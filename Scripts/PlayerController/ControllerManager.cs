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
    public GameObject HandRelease;

    [Header("�� �ý���")]
    public GameObject[] PetObject;
    private GameObject insPetObj;
    private PetMoveController intPetScript;

    [Header("PageLoding")]
    public GameObject PageLodingPopup;
    public Image progressBar = null;
    public Text ToolTipText;
    public string nextScene = "";

    private void Awake()
    {
        JumpKey = false;
        BoostKey = false;
        DataInfo.ins.RightTId = -1;

        _camera = Camera.main;

        //���� NPC ��ȭ�� ���ؼ� ȹ��
        CreatePetObject();
    }

    // Start is called before the first frame update
    void Start()
    {
        initController();
    }

    void initController()
    {
        DataInfo.ins.infoController = this;
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

        HandRelease.SetActive(false);
        PageLodingPopup.SetActive(false);
        progressBar.fillAmount = 0;
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

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }

    void CreatePetObject()
    {
        if (insPetObj != null)
        {
            Destroy(insPetObj);
            insPetObj = null;
        }

        insPetObj = Instantiate(PetObject[0]);
        intPetScript = insPetObj.AddComponent<PetMoveController>();
        intPetScript.myPlayerTrans = transform;
        intPetScript.PlayerMoveSpeed = moveSpeed;
        insPetObj.name = "��";
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
                if(Truk.parent != null)
                    temp = Truk.parent.GetComponent<WorldInteraction>();
                else
                    temp = Truk.GetComponent<WorldInteraction>();

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
                    if (EventState == 1)
                    {
                        DataInfo.ins.RoomOutButtonSetting();
                    }
                    break;
                case InteractionType.WorldMapOut:
                    if (EventState == 1)
                    {
                        DataInfo.ins.WorldMapOutButtonSetting();
                    }
                    break;
                case InteractionType.OnChair:
                    if (EventState == 1)
                    {
                        if(DataInfo.ins.CharacterMain.Sex == 1)
                            Com.ins.AniSetInt(mAnimator, "Interaction", 101);
                        else
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
                        if (DataInfo.ins.CharacterMain.Sex == 1)
                            Com.ins.AniSetInt(mAnimator, "Interaction", 102);
                        else
                            Com.ins.AniSetInt(mAnimator, "Interaction", 2);
                    }
                    PlayerObject.position = EventScripts.PlayerPos;
                    break;
                case InteractionType.Gift:
                    if (EventState == 1)
                    {
                        GiftGet();
                    }
                    PlayerObject.position = EventScripts.PlayerPos;
                    break;
                case InteractionType.Pickup:
                    if (EventState == 1)
                    {
                        StartCoroutine(HandObjectSetting());
                    }
                    break;
                case InteractionType.NPC_PetMaster:
                    if (EventState == 1)
                    {
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

        HandRelease.SetActive(true);
        HandItem = true;
    }

    void GiftGet()
    {
        int[] getMoney = { 100, 150, 200, 250, 300 };
        Com.ins.ShuffleArray(getMoney);

        if (DataInfo.ins.CharacterMain.Sex == 1)
            Com.ins.AniSetInt(mAnimator, "Interaction", 103);
        else
            Com.ins.AniSetInt(mAnimator, "Interaction", 3);

        RRSpawn.ItemDelet(EventScripts);

        if (DataInfo.ins.Now_QID == 3)
        {
            DataInfo.ins.QuestData[3].State = 1;
        }

        DataInfo.ins.AddMoney(getMoney[0]);

        DataInfo.ins.SaveData = JsonUtility.ToJson(DataInfo.ins.CharacterMain);
        //������ ȹ��
        UIController.CallToastMassage("���� ȹ�� �Ͽ����ϴ�. [" + getMoney[0] + "] Gold", 0.8f);
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
            if (DataInfo.ins.CharacterMain.Sex == 1)
                mAnimator.SetTrigger("ManJump");
            else
                mAnimator.SetTrigger("Jump");
            //_verticalVelocity = Mathf.Sqrt(JumpDis * 2f * Gravity);
        }
    }

    public void OnClick_HandRelease()
    {
        //Hand Item Release
        if (HandItem)
        {
            if (_manager.PickupTrans.childCount > 0)
            {
                for (int i = _manager.PickupTrans.childCount - 1; i >= 0; i--)
                {
                    Destroy(_manager.PickupTrans.GetChild(i).gameObject);
                }
            }
        }

        HandItem = false;
        HandRelease.SetActive(false);
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
                    if (DataInfo.ins.CharacterMain.Sex == 1)
                        Com.ins.AniSetInt(mAnimator, "MoveState", 100);
                    else
                        Com.ins.AniSetInt(mAnimator, "MoveState", 0);
                    break;
                case 1:
                    moveSpeedSum = moveSpeed;
                    if (DataInfo.ins.CharacterMain.Sex == 1)
                        Com.ins.AniSetInt(mAnimator, "MoveState", 101);
                    else
                        Com.ins.AniSetInt(mAnimator, "MoveState", 1);
                    break;
                case 2:
                    moveSpeedSum = moveSpeed + moveSpeed;
                    if (DataInfo.ins.CharacterMain.Sex == 1)
                        Com.ins.AniSetInt(mAnimator, "MoveState", 102);
                    else
                        Com.ins.AniSetInt(mAnimator, "MoveState", 2);
                    break;
                case 3:
                    moveSpeedSum = moveSpeed + moveSpeed * moveSpeed;
                    if (DataInfo.ins.CharacterMain.Sex == 1)
                        Com.ins.AniSetInt(mAnimator, "MoveState", 103);
                    else
                        Com.ins.AniSetInt(mAnimator, "MoveState", 3);
                    break;
            }
        }
    }


    public void LoadScene(string sceneName)
    {
        progressBar.fillAmount = 0;
        PageLodingPopup.SetActive(true);
        nextScene = sceneName;

        StartCoroutine(coroutineSceneLoding());

        if (DataInfo.ins.Now_QID == 0 && sceneName.Equals("Room_A"))
        {
            DataInfo.ins.QuestData[0].State = 1;
        }
        if (DataInfo.ins.Now_QID == 1 && sceneName.Equals("World_A"))
        {
            DataInfo.ins.QuestData[1].State = 1;
        }
    }

    IEnumerator coroutineSceneLoding()
    {
        yield return null;

        AsyncOperation op;
        op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;

        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);

                if (progressBar.fillAmount >= 0.99f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}