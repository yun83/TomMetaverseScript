using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;
using UnityEngine.SceneManagement;

public class ControllerManager : MonoBehaviour
{
    [Header("Ui Canvase")]
    public VirtualJoystick LeftHand;
    public UiMessage MessgaeBox;

    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public Transform Axis;

    [Tooltip("카메라 회전 속도")]
    public float rotateSpeed = 0.65f;
    [Tooltip("카메라 확대 축소 속도")]
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

    public bool BoostKey = false;
    public bool JumpKey = false;

    [Header("케릭터")]
    public Transform PlayerObject;
    private CharacterController _controller;
    private CharacterManager _manager;
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

    private Animator mAnimator = null;
    //public AnimatorController[] AniType;

    public CinemachineVirtualCamera cvc;
    private Camera _camera;
    private UiButtonController UIController;

    [Header("상호작용")]
    public RandomRespawn RRSpawn;
    public WorldInteraction EventScripts;
    public int EventState = 0;
    /// <summary>
    /// 손에 아이템을 들었는지 판단
    /// </summary>
    private bool HandItem = false;
    public GameObject HandRelease;

    [Header("펫 시스템")]
    public GameObject[] PetObject;
    private GameObject insPetObj;
    private PetMoveController intPetScript;
    [Tooltip("펫이 앉아 있는 공간")]
    public Transform PetInTrans;

    [Header("PageLoding")]
    public GameObject PageLodingPopup;
    public Image progressBar = null;
    public Text ToolTipText;
    public string nextScene = "";
    /// <summary>
    /// 0 - Ect,
    /// 1 - MyRoom,
    /// </summary>
    public int RoomCheck = 0;

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
        string SceneName = SceneManager.GetActiveScene().name;
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

        //추후 NPC 대화를 통해서 획득
        CreatePetObject();

        if (SceneName.Equals("Room_A") || SceneName.Equals("Room_B"))
        {
            RoomCheck = 1;
        }
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
        insPetObj.name = "펫";
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
            //회전
            PlayerObject.eulerAngles = sumAngles;

            if (EventScripts != null)
            {
                EventScripts.OutInteraction();
            }
            EventState = 0;
        }

        transform.position = PlayerObject.position;
        if (EventState == 0)
        {
            //이동
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
                    EventScripts.OutInteraction();
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
                EventScripts.OnInteraction();
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
        //아이템 획득
        UIController.CallToastMassage("선물 획득 하였습니다. [" + getMoney[0] + "] Gold", 0.8f);
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
        op = SceneManager.LoadSceneAsync(nextScene);
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