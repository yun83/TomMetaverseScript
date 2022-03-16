using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;
using UnityEditor.Animations;

public class ControllerManager : MonoBehaviour
{
    [Header("Ui Canvase")]
    public VirtualJoystick LeftHand;
    public UiMessage MessgaeBox;

    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public Transform Axis;

    [Tooltip("카메라 따라오는 속도")]
    public float manMoveSpeed = 1f;     // 플레이어를 따라오는 카메라 맨의 스피드.
    [Tooltip("카메라 회전 속도")]
    [Range(0.1f, 5f)]
    public float rotateSpeed = 0.65f;
    [Tooltip("카메라 확대 축소 속도")]
    [Range(0.01f, 2f)]
    public float ZoomSpeed = 0.5f;            // 줌 스피드.
    [Tooltip("카메라와의 거리")]
    [Range(25f, 50f)]
    public float Distance = 40f;          // 카메라와의 거리.
    [Tooltip("최소 거리")]
    [Range(25f, 40f)]
    public float DisMinSize = 25f;
    [Tooltip("최대 거리")]
    [Range(35f, 60f)]
    public float DisMaxSize = 30;
    [Tooltip("카메라 각도 최소 제한")]
    [Range(-40, 30)]
    public float RotationDisMin = -30;
    [Tooltip("카메라 각도 최대 제한")]
    [Range(0, 80)]
    public float RotationDisMax = 70;

    [Header("Player")]
    public Transform PlayerObject;
    private CharacterController _controller;
    [Tooltip("프레임당 점프 높이")]
    public float JumpDis = 0.02f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -1.5f;
    [Tooltip("케릭터 이동 속도")]
    [Range(1f, 5f)]
    public float moveSpeed = 2;
    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    private Vector2 AxisRtt;
    private int MoveAniState = -1;
    private float moveSpeedSum = 0;
    private float deltaMagnitudeDiff = 0;
    private float _verticalVelocity = 0;

    public bool BoostKey = false;
    public bool JumpKey = false;

    private Animator mAnimator = null;
    public AnimatorController[] AniType;

    public CinemachineVirtualCamera cvc;
    private Camera _camera;
    private WorldInteraction EvnetTrans;
    private int EventState = 0;

    public RandomRespawn RRSpawn;

    private void Awake()
    {
        JumpKey = false;
        BoostKey = false;
        DataInfo.ins.RightTId = -1;

        _camera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        initController();
    }

    void initController()
    {
        if (PlayerObject == null)
            PlayerObject = GameObject.FindGameObjectWithTag("Player").transform;

        PlayerObject.GetComponent<CharacterManager>().itemEquipment(DataInfo.ins.CharacterMain);
        _controller = PlayerObject.GetComponent<CharacterController>();

        if (cvc == null)
            cvc = GetComponentInChildren<CinemachineVirtualCamera>();
        if (cvc.Follow == null)
            cvc.Follow = Axis;

        cvc.LookAt = PlayerObject;

        if (mAnimator == null)
            mAnimator = PlayerObject.GetComponentInChildren<Animator>();

        if (DataInfo.ins.CharacterMain.Sex == 0)
            mAnimator.runtimeAnimatorController = AniType[0];
        else
            mAnimator.runtimeAnimatorController = AniType[1];

        RRSpawn = GameObject.FindObjectOfType<RandomRespawn>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveUpdate();
        GroundedCheck();
        RayCastEventLogic();
    }

    private void FixedUpdate()
    {
    }

    void MoveUpdate()
    {
        float moveH = 0;
        float moveV = 0;
        int mtempState = 0;
        deltaMagnitudeDiff = 0;

#if UNITY_EDITOR
        moveH = Input.GetAxis("Horizontal");
        moveV = Input.GetAxis("Vertical");

        if (Input.GetMouseButtonDown(0))
        {
            if (RayCastEvent(Input.mousePosition))
            {
                return;
            }
        }
        if (Input.GetMouseButton(1))
        {
            AxisRtt.y += Input.GetAxis("Mouse X"); // 마우스의 좌우 이동량을 xmove 에 누적합니다.
            AxisRtt.x -= Input.GetAxis("Mouse Y"); // 마우스의 상하 이동량을 ymove 에 누적합니다.
        }
        deltaMagnitudeDiff = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnClick_Jump();
        }
#endif
        if (LeftHand == null)
            return;

        if (LeftHand.MoveFlag)
        {
            moveH = LeftHand.JoyVec.x;
            moveV = LeftHand.JoyVec.y;
            //Debug.Log(moveH + ":::" + moveV);
        }

        if (LeftHand.MoveFlag == false && Input.touchCount >= 2)
        {
            Touch touchZero = Input.GetTouch(0); //첫번째 손가락 터치를 저장
            Touch touchOne = Input.GetTouch(1); //두번째 손가락 터치를 저장

            //터치에 대한 이전 위치값을 각각 저장함
            //처음 터치한 위치(touchZero.position)에서 이전 프레임에서의 터치 위치와 이번 프로임에서 터치 위치의 차이를 뺌
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition; //deltaPosition는 이동방향 추적할 때 사용
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // 각 프레임에서 터치 사이의 벡터 거리 구함
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude; //magnitude는 두 점간의 거리 비교(벡터)
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // 거리 차이 구함(거리가 이전보다 크면(마이너스가 나오면)손가락을 벌린 상태_줌인 상태)
            if (prevTouchDeltaMag - touchDeltaMag > 0)
                deltaMagnitudeDiff = ZoomSpeed;
            else
                deltaMagnitudeDiff = -ZoomSpeed;
        }
        else if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch tempTouch = Input.GetTouch(i);

                if (tempTouch.fingerId != DataInfo.ins.TouchId)
                {
                    switch (tempTouch.phase)
                    {
                        case TouchPhase.Began:
                            if (RayCastEvent(tempTouch.position))
                            {
                                return;
                            }
                            if (DataInfo.ins.RightTId == -1)
                            {
                                DataInfo.ins.RightTId = tempTouch.fingerId;
                            }
                            break;
                        case TouchPhase.Moved:
                            if (!EventSystem.current.IsPointerOverGameObject(tempTouch.fingerId))
                            {
                                if (tempTouch.fingerId == DataInfo.ins.RightTId)
                                {
                                    AxisRtt.y += tempTouch.deltaPosition.x * rotateSpeed;
                                    AxisRtt.x -= tempTouch.deltaPosition.y * rotateSpeed;
                                }
                            }
                            break;
                        case TouchPhase.Canceled:
                        case TouchPhase.Ended:
                            if (tempTouch.fingerId == DataInfo.ins.RightTId)
                            {
                                DataInfo.ins.RightTId = -1;
                            }
                            break;
                    }
                }
            }
        }
        //거리에 따른  에니메이션 지정
        if (LeftHand.MoveFlag)
        {
            if (Input.GetKey(KeyCode.LeftShift) || BoostKey)
                mtempState = 3;
            else if (LeftHand.Run)
                mtempState = 2;
            else
                mtempState = 1;
        }
        else
        {
            float Dis = Vector3.Distance(Vector2.zero, new Vector2(moveH, moveV));
            if (Dis != 0)
            {
                if (Input.GetKey(KeyCode.LeftShift) || BoostKey)
                    mtempState = 3;
                else if (Dis < 1f)
                    mtempState = 1;
                else
                    mtempState = 2;
            }
        }
        //에니메이션 셋팅
        MoveAniStateSetting(mtempState);

        //카메라 축소 확대
        Distance += deltaMagnitudeDiff * -1;
        Distance = Mathf.Clamp(Distance, DisMinSize, DisMaxSize);
        cvc.m_Lens.FieldOfView = Distance;

        //카메라의 회전
        Quaternion v3Rotation = new Quaternion();
        float minRttX = Mathf.Clamp(AxisRtt.x, RotationDisMin, RotationDisMax);
        AxisRtt.x = minRttX;
        v3Rotation.eulerAngles = new Vector3(minRttX, AxisRtt.y, 0.0f);
        Axis.rotation = v3Rotation;

        //카메라 방향에 따른 케릭터 방향
        float offset = Time.deltaTime * moveSpeedSum;
        if (moveH != 0 || moveV != 0)
        {
            Vector3 tempAngles = new Vector3();
            tempAngles.x = offset;
            tempAngles.y = Axis.eulerAngles.y;
            tempAngles.z = Axis.eulerAngles.z;
            Vector3 plusAngles = new Vector3(0, Mathf.Atan2(moveH, moveV) * Mathf.Rad2Deg, 0);
            Vector3 sumAngles = (tempAngles + plusAngles);
            //이동
            PlayerObject.eulerAngles = sumAngles;

            if (EvnetTrans != null)
            {
                EvnetTrans.EventObj.SetActive(true);
                if (EvnetTrans.mCollider != null)
                {
                    EvnetTrans.mCollider.isTrigger = false;
                }
            }
            EventState = 0;
        }

        transform.position = PlayerObject.position;
        if (EventState == 0)
        {
            //PlayerObject.position += PlayerObject.forward * offset;
            // move the player
            Vector3 MoveVec3 = PlayerObject.forward * offset;
            MoveVec3.y = _verticalVelocity;
            _controller.Move(MoveVec3);
            Axis.position = PlayerObject.position;
        }
    }

    private void GroundedCheck()
    {
        if (EventState == 0)
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(PlayerObject.position.x, PlayerObject.position.y - GroundedOffset, PlayerObject.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (Grounded)
            {
                JumpKey = false;
            }
            // 중력의 영향을 받아 아래쪽으로 하강합니다.
            _verticalVelocity -= Gravity * Time.deltaTime;
        }
    }

    RaycastHit rayHit;
    bool RayCastEvent(Vector3 inPos)
    {
        bool ret = false;
        float distance = 50f;
        int layerMask = 1 << LayerMask.NameToLayer("TouchLayer");  // FindObject 레이어만 충돌 체크함
        Ray ray = _camera.ScreenPointToRay(inPos);


        if (Physics.Raycast(ray, out rayHit, distance, layerMask))
        {
            if (rayHit.transform.parent.TryGetComponent<WorldInteraction>(out var temp))
            {
                if (EvnetTrans != null)
                {
                    EvnetTrans.EventObj.SetActive(true);
                    if (EvnetTrans.mCollider != null)
                    {
                        EvnetTrans.mCollider.isTrigger = false;
                    }
                }
                Debug.Log("Ray Cast Event Trigger [<color=blue>" + temp.name + "</color>] Tag [<color=yellow>" + temp.nowType.ToString() + "</color>]");
                EvnetTrans = temp;
                EventState = 1;
            }
            ret = true;
        }
        return ret;
    }

    void RayCastEventLogic()
    {
        if (EventState > 0 && EventState < 3)
        {
            if (EvnetTrans.mCollider != null)
            {
                EvnetTrans.mCollider.isTrigger = true;
            }

            switch (EvnetTrans.nowType)
            {
                case InteractionType.OutRoom:
                    DataInfo.ins.RoomOutButtonSetting();
                    break;
                case InteractionType.WorldMapOut:
                    DataInfo.ins.WorldMapOutButtonSetting();
                    break;
                case InteractionType.OnChair:
                    if (EventState == 1)
                    {
                        mAnimator.SetInteger("Interaction", 1);
                    }
                    PlayerObject.position = EvnetTrans.PlayerPos;
                    PlayerObject.eulerAngles = EvnetTrans.PlayerRotation;
                    break;
                case InteractionType.Meditate:
                    if (EventState == 1)
                    {
                        mAnimator.SetInteger("Interaction", 2);
                    }
                    PlayerObject.position = EvnetTrans.PlayerPos;
                    break;
                case InteractionType.Gift:
                    if (EventState == 1)
                    {
                        mAnimator.SetInteger("Interaction", 3);
                        RRSpawn.ItemDelet(EvnetTrans);
                    }
                    PlayerObject.position = EvnetTrans.PlayerPos;
                    break;
            }
            EventState ++;
            if (EventState < 3)
            {
                EvnetTrans.EventObj.SetActive(false);
            }
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    void OnClick_SeveClipBoard(string getText)
    {
        Debug.Log(getText);
        UniClipboard.SetText(getText);

        MessgaeBox.OnMessage("'" + getText + "'\n클립보드에 복사 되었습니다.");
    }

    public void PointDown_Boost()
    {
        BoostKey = true;
    }
    public void PointUp_Boost()
    {
        BoostKey = false;
    }

    public void OnClick_Jump()
    {
        if (!JumpKey)
        {
            JumpKey = true;
            mAnimator.SetTrigger("Jump");
            _verticalVelocity = Mathf.Sqrt(JumpDis * 2f * Gravity);
        }
    }


    void MoveAniStateSetting(int mState)
    {
        if (mAnimator == null)
        {
            Debug.LogError("에니메이션 오브젝트가 잡히지 않았습니다.");
            return;
        }

        if (mState != MoveAniState)
        {
            MoveAniState = mState;

            switch (MoveAniState)
            {
                case 0:
                    moveSpeedSum = 0;
                    mAnimator.SetInteger("MoveState", 0);
                    break;
                case 1:
                    moveSpeedSum = moveSpeed;
                    mAnimator.SetInteger("MoveState", 1);
                    break;
                case 2:
                    moveSpeedSum = moveSpeed + moveSpeed;
                    mAnimator.SetInteger("MoveState", 2);
                    break;
                case 3:
                    moveSpeedSum = moveSpeed + moveSpeed * moveSpeed;
                    mAnimator.SetInteger("MoveState", 3);
                    break;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }
}
