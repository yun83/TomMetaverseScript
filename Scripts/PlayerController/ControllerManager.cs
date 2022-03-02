using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControllerManager : MonoBehaviour
{
    [Header("Ui Canvase")]
    public VirtualJoystick LeftHand;
    public UiMessage MessgaeBox;

    public Button JumpButton;
    public EventTrigger BoostButton;

    [Header("Camera")]
    public CameraMan cameraMan;
    [Tooltip("카메라 따라오는 속도")]
    public float manMoveSpeed = 1f;     // 플레이어를 따라오는 카메라 맨의 스피드.
    [Tooltip("카메라 회전 속도")]
    [Range(0.1f, 5f)]
    public float rotateSpeed = 0.65f;
    [Tooltip("카메라 확대 축소 속도")]
    [Range(0.01f, 2f)]
    public float ZoomSpeed = 0.25f;            // 줌 스피드.
    [Tooltip("카메라와의 거리")]
    [Range(0.5f, 9f)]
    public float Distance = 1.5f;          // 카메라와의 거리.
    public float H_Distance = 0.5f;          // 카메라와의 높이.
    [Tooltip("최소 거리")]
    [Range(0.5f, 4f)]
    public float DisMinSize = 0.5f;
    [Tooltip("최대 거리")]
    [Range(4f, 10f)]
    public float DisMaxSize = 10f;
    [Tooltip("카메라 각도 최소 제한")]
    [Range(-30, 30)]
    public float RotationDisMin = 0;
    [Tooltip("카메라 각도 최대 제한")]
    [Range(0, 60)]
    public float RotationDisMax = 55;
    [Tooltip("카메라 각도 최대 제한")]
    [Range(0, 600)]
    public float jumpPower = 100;

    [Header("Player")]
    public Transform PlayerObject;
    [Tooltip("케릭터 이동 속도")]
    [Range(1f, 5f)]
    public float moveSpeed = 2;

    private Vector2 AxisRtt;
    private Vector3 AxisVec;
    private Animator mAnimator = null;
    private int MoveAniState = -1;
    private float moveSpeedSum = 0;
    private Vector3 CameraManPos;        // 카메라 맨의 위치.
    private int WallHitState = 0;
    private float oldDistance = 3.5f;
    private float ExChangedDis = 0;
    private float deltaMagnitudeDiff = 0;

    public bool BoostKey = false;

    private void Awake()
    {
        Application.targetFrameRate = 1000;
        
        Debug.Log("------------- JoiSick Init Start ---------------");
        if (cameraMan == null)
        {
            Debug.LogError("CameraMan Null Point");
        }

        //Debug.Log("------------- JoiSick Init End ---------------");
        if (mAnimator == null)
            mAnimator = PlayerObject.GetComponentInChildren<Animator>();

        BoostKey = false;
        DataInfo.ins.RightTId = -1;

        Com.ins.BgmSoundPlay(Resources.Load<AudioClip>("BGM/Progress"));
        PlayerObject.GetComponent<CharacterManager>().itemEquipment(DataInfo.ins.CharacterMain);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MoveUpdate();
    }

    void MoveUpdate()
    {
        float moveH = Input.GetAxis("Horizontal");
        float moveV = Input.GetAxis("Vertical");
        int mtempState = 0;
        deltaMagnitudeDiff = 0;

#if UNITY_EDITOR
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
        }

        //거리에 따른  에니메이션 지정

        if (LeftHand.MoveFlag)
        {
            if (Input.GetKey(KeyCode.LeftShift) || BoostKey)
                mtempState = 3;
            else if(LeftHand.Run)
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

        if (WallHitState == 1)
            WallHitState = 2;
        if (WallHitState == 3)
        {
            if (oldDistance > Distance)
                deltaMagnitudeDiff = -ZoomSpeed;
            else
                WallHitState = 0;
        }

        RaycastHit hitinfo;
        if (Physics.Raycast(cameraMan.CameraMain.position, cameraMan.CameraMain.forward, out hitinfo, 100))
        {
            if (hitinfo.transform.tag == "Wall")
            {
                //Debug.Log(" ------------ " + hitinfo.transform.name + " -------------- ");
                if (WallHitState == 0)
                {
                    oldDistance = Distance;
                }
                deltaMagnitudeDiff = Vector2.Distance(hitinfo.point, cameraMan.CameraMain.position);
                deltaMagnitudeDiff += 0.2f;
                WallHitState = 1;
                ExChangedDis = Time.time + 1;
            }
        }
        if (WallHitState == 2)
        {
            if (Physics.Raycast(cameraMan.CameraMain.position, -cameraMan.CameraMain.forward, out hitinfo, 100))
            {
                if (hitinfo.transform.tag == "Wall")
                {
                    float dir = Vector3.Distance(cameraMan.Axis.position, hitinfo.point);
                    //Debug.Log(dir + " [ " + Distance + " ] => [ " + oldDistance + " ]" );

                    if (dir > Distance)
                    {
                        WallHitState = 3;
                    }
                }
            }
        }

        //에니메이션 셋팅
        MoveAniStateSetting(mtempState);


        //카메라 축소 확대
        Distance += deltaMagnitudeDiff * -1;
        Distance = Mathf.Clamp(Distance, DisMinSize, DisMaxSize);

        AxisVec = cameraMan.Axis.forward * -1;
        AxisVec *= Distance;
        AxisVec.y += H_Distance;
        cameraMan.CameraMain.position = cameraMan.Axis.position + AxisVec;

        //카메라의 회전
        Quaternion v3Rotation = new Quaternion();
        float minRttX = Mathf.Clamp(AxisRtt.x, RotationDisMin, RotationDisMax);
        AxisRtt.x = minRttX;
        v3Rotation.eulerAngles = new Vector3(minRttX, AxisRtt.y, v3Rotation.z);
        cameraMan.Axis.rotation = v3Rotation;
        cameraMan.CameraMain.transform.LookAt(cameraMan.Axis.position);

        //카메라맨의 위치
        CameraManPos = cameraMan.transform.position;
        cameraMan.transform.position += (PlayerObject.position - CameraManPos) * manMoveSpeed;

        //카메라 방향에 따른 케릭터 방향
        float offset = Time.deltaTime * moveSpeedSum;
        if (moveH != 0 || moveV != 0)
        {
            Vector3 tempAngles = cameraMan.Axis.eulerAngles;
            tempAngles.x = offset;
            Vector3 plusAngles = new Vector3(0, Mathf.Atan2(moveH, moveV) * Mathf.Rad2Deg, 0);
            Vector3 sumAngles = (tempAngles + plusAngles);

            //이동
            PlayerObject.eulerAngles = sumAngles;
        }
        PlayerObject.position += PlayerObject.forward * offset;
    }

    RaycastHit rayHit;
    void ObjectRaycast()
    {
        float distance = 50f;
        int layerMask = 1 << LayerMask.NameToLayer("FindObject");  // FindObject 레이어만 충돌 체크함

        if (Physics.Raycast(cameraMan.CameraMain.position, cameraMan.CameraMain.forward, out rayHit, distance, layerMask))
        {
            GameObject temp = rayHit.transform.gameObject;
        }
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
        mAnimator.SetTrigger("Jump");
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
}
