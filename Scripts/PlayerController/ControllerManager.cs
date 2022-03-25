using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;

public class ControllerManager : MonoBehaviour
{
    [Header("Ui Canvase")]
    public VirtualJoystick LeftHand;
    public UiMessage MessgaeBox;

    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public Transform Axis;

    [Tooltip("ī�޶� ȸ�� �ӵ�")]
    public float rotateSpeed = 0.65f;
    [Tooltip("ī�޶� Ȯ�� ��� �ӵ�")]
    public float ZoomSpeed = 0.5f;            // �� ���ǵ�.
    [Tooltip("ī�޶���� �Ÿ�")]
    [Range(25f, 50f)]
    public float Distance = 40f;          // ī�޶���� �Ÿ�.
    [Tooltip("�ּ� �Ÿ�")]
    [Range(25f, 40f)]
    public float DisMinSize = 25f;
    [Tooltip("�ִ� �Ÿ�")]
    [Range(35f, 60f)]
    public float DisMaxSize = 30;
    [Tooltip("ī�޶� ���� �ּ� ����")]
    [Range(-40, 30)]
    public float RotationDisMin = -30;
    [Tooltip("ī�޶� ���� �ִ� ����")]
    [Range(0, 80)]
    public float RotationDisMax = 70;

    public bool BoostKey = false;
    public bool JumpKey = false;

    [Header("�ɸ���")]
    public Transform PlayerObject;
    private CharacterController _controller;
    private CharacterManager _manager;
    [Tooltip("�����Ӵ� ���� ����")]
    public float JumpDis = 0.02f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -1.5f;
    [Tooltip("�ɸ��� �̵� �ӵ�")]
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

    private Animator mAnimator = null;
    //public AnimatorController[] AniType;

    public CinemachineVirtualCamera cvc;
    private Camera _camera;
    private UiButtonController UIController;

    [Header("��ȣ�ۿ�")]
    public RandomRespawn RRSpawn;
    public WorldInteraction EventScripts;
    public int EventState = 0;
    /// <summary>
    /// �տ� �������� ������� �Ǵ�
    /// </summary>
    private bool HandItem = false;

    [Header("�� �ý���")]
    public GameObject[] PetObject;
    private GameObject insPetObj;
    private PetMoveController intPetScript;

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

        _controller = PlayerObject.GetComponent<CharacterController>();
        _manager = PlayerObject.GetComponent<CharacterManager>();
        _manager.itemEquipment(DataInfo.ins.CharacterMain);

        if (cvc == null)
            cvc = GetComponentInChildren<CinemachineVirtualCamera>();
        if (cvc.Follow == null)
            cvc.Follow = Axis;

        cvc.LookAt = PlayerObject;

        if (mAnimator == null)
            mAnimator = PlayerObject.GetComponentInChildren<Animator>();

        //if (DataInfo.ins.CharacterMain.Sex == 0)
        //    mAnimator.runtimeAnimatorController = AniType[0];
        //else
        //    mAnimator.runtimeAnimatorController = AniType[1];

        RRSpawn = GameObject.FindObjectOfType<RandomRespawn>();
        UIController = GetComponentInChildren<UiButtonController>();

        insPetObj = Instantiate(PetObject[0]);
        intPetScript = insPetObj.AddComponent<PetMoveController>();
        intPetScript.myPlayerTrans = transform;
        intPetScript.PlayerMoveSpeed = moveSpeed;
        insPetObj.name = "��";
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
            AxisRtt.y += Input.GetAxis("Mouse X"); // ���콺�� �¿� �̵����� xmove �� �����մϴ�.
            AxisRtt.x -= Input.GetAxis("Mouse Y"); // ���콺�� ���� �̵����� ymove �� �����մϴ�.
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
            Touch touchZero = Input.GetTouch(0); //ù��° �հ��� ��ġ�� ����
            Touch touchOne = Input.GetTouch(1); //�ι�° �հ��� ��ġ�� ����

            //��ġ�� ���� ���� ��ġ���� ���� ������
            //ó�� ��ġ�� ��ġ(touchZero.position)���� ���� �����ӿ����� ��ġ ��ġ�� �̹� �����ӿ��� ��ġ ��ġ�� ���̸� ��
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition; //deltaPosition�� �̵����� ������ �� ���
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // �� �����ӿ��� ��ġ ������ ���� �Ÿ� ����
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude; //magnitude�� �� ������ �Ÿ� ��(����)
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // �Ÿ� ���� ����(�Ÿ��� �������� ũ��(���̳ʽ��� ������)�հ����� ���� ����_���� ����)
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
                            if (!EventSystem.current.IsPointerOverGameObject(tempTouch.fingerId))
                            {
                                if (RayCastEvent(tempTouch.position))
                                {
                                    return;
                                }
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
        //�Ÿ��� ����  ���ϸ��̼� ����
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
        //���ϸ��̼� ����
        MoveAniStateSetting(mtempState);

        //ī�޶� ��� Ȯ��
        Distance += deltaMagnitudeDiff * -1;
        Distance = Mathf.Clamp(Distance, DisMinSize, DisMaxSize);
        cvc.m_Lens.FieldOfView = Distance;

        //ī�޶��� ȸ��
        Quaternion v3Rotation = new Quaternion();
        float minRttX = Mathf.Clamp(AxisRtt.x, RotationDisMin, RotationDisMax);
        AxisRtt.x = minRttX;
        v3Rotation.eulerAngles = new Vector3(minRttX, AxisRtt.y, 0.0f);
        Axis.rotation = v3Rotation;

        //ī�޶� ���⿡ ���� �ɸ��� ����
        float offset = Time.deltaTime * moveSpeedSum;
        if (moveH != 0 || moveV != 0)
        {
            Vector3 tempAngles = new Vector3();
            tempAngles.x = offset;
            tempAngles.y = Axis.eulerAngles.y;
            tempAngles.z = Axis.eulerAngles.z;
            Vector3 plusAngles = new Vector3(0, Mathf.Atan2(moveH, moveV) * Mathf.Rad2Deg, 0);
            Vector3 sumAngles = (tempAngles + plusAngles);
            //ȸ��
            PlayerObject.eulerAngles = sumAngles;

            if (EventScripts != null)
            {
                EventScripts.UseState = 0;
                //EventScripts.EventObj.SetActive(true);
                if (EventScripts.mCollider != null)
                {
                    EventScripts.mCollider.isTrigger = false;
                }
            }
            EventState = 0;
        }

        transform.position = PlayerObject.position;
        if (EventState == 0)
        {
            //�̵�
            //PlayerObject.position += PlayerObject.forward * offset;
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
            // �߷��� ������ �޾� �Ʒ������� �ϰ��մϴ�.
            _verticalVelocity -= Gravity * Time.deltaTime;
        }
    }

    RaycastHit rayHit;
    bool RayCastEvent(Vector3 inPos)
    {
        bool ret = false;
        float distance = 50f;
        int layerMask = 1 << LayerMask.NameToLayer("TouchLayer");  // FindObject ���̾ �浹 üũ��
        Ray ray = _camera.ScreenPointToRay(inPos);


        if (Physics.Raycast(ray, out rayHit, distance, layerMask))
        {
            WorldInteraction temp = null;
            Transform Truk = rayHit.transform;

            Debug.Log("Ray Cast Event Trigger [<color=blue> Name : " + Truk.name + "</color> ] [<color=yellow> Tag : " + Truk.tag + "</color> ]");

            try
            {
                temp = Truk.parent.GetComponent<WorldInteraction>();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.ToString());
            }

            if (temp != null)
            {
                if (EventScripts != null)
                {
                    //EventScripts.EventObj.SetActive(true);
                    if (EventScripts.mCollider != null)
                    {
                        EventScripts.mCollider.isTrigger = false;
                    }
                }
                EventScripts = temp;
                EventState = 1;
            }
            else
            {
                if (Truk.tag == "Pet")
                {
                    intPetScript.PetInteraction();
                }
                else
                {
                    switch (Truk.name)
                    {
                        case "PI_0":
                            intPetScript.OnClick_Evnet_0();
                            if (DataInfo.ins.Now_QID == 4)
                            {
                                DataInfo.ins.QuestData[4].State = 1;
                            }
                            break;
                        case "PI_1":
                            intPetScript.OnClick_Evnet_1();
                            if (DataInfo.ins.Now_QID == 4)
                            {
                                DataInfo.ins.QuestData[4].State = 1;
                            }
                            break;
                        case "PI_2":
                            intPetScript.OnClick_Evnet_2();
                            if (DataInfo.ins.Now_QID == 4)
                            {
                                DataInfo.ins.QuestData[4].State = 1;
                            }
                            break;
                    }
                }
            }

            ret = true;
        }
        return ret;
    }

    public void RayCastEventLogic()
    {
        if (EventState > 0 && EventState < 3)
        {
            if (EventScripts.mCollider != null)
            {
                EventScripts.mCollider.isTrigger = true;
            }

            switch (EventScripts.nowType)
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
                        Com.ins.AniSetInt(mAnimator, "Interaction", 1);

                        if (DataInfo.ins.Now_QID == 2)
                        {
                            DataInfo.ins.QuestData[2].State = 1;
                        }
                    }
                    PlayerObject.position = EventScripts.PlayerPos;
                    PlayerObject.eulerAngles = EventScripts.PlayerRotation;
                    break;
                case InteractionType.Meditate:
                    if (EventState == 1)
                    {
                        Com.ins.AniSetInt(mAnimator, "Interaction", 2);
                    }
                    PlayerObject.position = EventScripts.PlayerPos;
                    break;
                case InteractionType.Gift:
                    if (EventState == 1)
                    {
                        Com.ins.AniSetInt(mAnimator, "Interaction", 3);
                        RRSpawn.ItemDelet(EventScripts);
                        //UIController.OnClick_Roulette();
                        if (DataInfo.ins.Now_QID == 3)
                        {
                            DataInfo.ins.QuestData[3].State = 1;
                        }
                    }
                    PlayerObject.position = EventScripts.PlayerPos;
                    break;
                case InteractionType.Pickup:
                    if (EventState == 1)
                    {
                        StartCoroutine(HandObjectSetting());
                    }
                    break;
            }
            EventState++;
            EventScripts.UseState = 1;
        }
    }

    IEnumerator HandObjectSetting()
    {
        if (_manager.PickupTrans.childCount > 0)
        {
            for (int i = _manager.PickupTrans.childCount - 1; i >= 0; i--)
            {
                Destroy(_manager.PickupTrans.GetChild(i).gameObject);
            }
        }
        yield return null;

        GameObject HandObject = Instantiate(EventScripts.gameObject);
        yield return null;

        WorldInteraction temp = HandObject.GetComponent<WorldInteraction>();
        Destroy(temp.EventObj);
        Destroy(temp);
        yield return null;

        HandObject.transform.parent = _manager.PickupTrans;
        HandObject.transform.localPosition = Vector3.zero;

        HandItem = true;
    }

    public void RouletteEndEvent()
    {
        UIController.OnClick_CloseAllPopup();
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

        MessgaeBox.OnMessage("'" + getText + "'\nŬ�����忡 ���� �Ǿ����ϴ�.");
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
            //_verticalVelocity = Mathf.Sqrt(JumpDis * 2f * Gravity);
        }
    }


    void MoveAniStateSetting(int mState)
    {
        if (mAnimator == null)
        {
            Debug.LogError("���ϸ��̼� ������Ʈ�� ������ �ʾҽ��ϴ�.");
            return;
        }

        if (mState != MoveAniState)
        {
            MoveAniState = mState;

            switch (MoveAniState)
            {
                case 0:
                    moveSpeedSum = 0;
                    Com.ins.AniSetInt(mAnimator, "MoveState", 0);
                    break;
                case 1:
                    moveSpeedSum = moveSpeed;
                    Com.ins.AniSetInt(mAnimator, "MoveState", 1);
                    break;
                case 2:
                    moveSpeedSum = moveSpeed + moveSpeed;
                    Com.ins.AniSetInt(mAnimator, "MoveState", 2);
                    break;
                case 3:
                    moveSpeedSum = moveSpeed + moveSpeed * moveSpeed;
                    Com.ins.AniSetInt(mAnimator, "MoveState", 3);
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