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
    [Tooltip("ī�޶� ������� �ӵ�")]
    public float manMoveSpeed = 1f;     // �÷��̾ ������� ī�޶� ���� ���ǵ�.
    [Tooltip("ī�޶� ȸ�� �ӵ�")]
    [Range(0.1f, 5f)]
    public float rotateSpeed = 0.65f;
    [Tooltip("ī�޶� Ȯ�� ��� �ӵ�")]
    [Range(0.01f, 2f)]
    public float ZoomSpeed = 0.25f;            // �� ���ǵ�.
    [Tooltip("ī�޶���� �Ÿ�")]
    [Range(0.5f, 9f)]
    public float Distance = 1.5f;          // ī�޶���� �Ÿ�.
    public float H_Distance = 0.5f;          // ī�޶���� ����.
    [Tooltip("�ּ� �Ÿ�")]
    [Range(0.5f, 4f)]
    public float DisMinSize = 0.5f;
    [Tooltip("�ִ� �Ÿ�")]
    [Range(4f, 10f)]
    public float DisMaxSize = 10f;
    [Tooltip("ī�޶� ���� �ּ� ����")]
    [Range(-30, 30)]
    public float RotationDisMin = 0;
    [Tooltip("ī�޶� ���� �ִ� ����")]
    [Range(0, 60)]
    public float RotationDisMax = 55;
    [Tooltip("ī�޶� ���� �ִ� ����")]
    [Range(0, 600)]
    public float jumpPower = 100;

    [Header("Player")]
    public Transform PlayerObject;
    [Tooltip("�ɸ��� �̵� �ӵ�")]
    [Range(1f, 5f)]
    public float moveSpeed = 2;

    private Vector2 AxisRtt;
    private Vector3 AxisVec;
    private Animator mAnimator = null;
    private int MoveAniState = -1;
    private float moveSpeedSum = 0;
    private Vector3 CameraManPos;        // ī�޶� ���� ��ġ.
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
        }

        //�Ÿ��� ����  ���ϸ��̼� ����

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

        //���ϸ��̼� ����
        MoveAniStateSetting(mtempState);


        //ī�޶� ��� Ȯ��
        Distance += deltaMagnitudeDiff * -1;
        Distance = Mathf.Clamp(Distance, DisMinSize, DisMaxSize);

        AxisVec = cameraMan.Axis.forward * -1;
        AxisVec *= Distance;
        AxisVec.y += H_Distance;
        cameraMan.CameraMain.position = cameraMan.Axis.position + AxisVec;

        //ī�޶��� ȸ��
        Quaternion v3Rotation = new Quaternion();
        float minRttX = Mathf.Clamp(AxisRtt.x, RotationDisMin, RotationDisMax);
        AxisRtt.x = minRttX;
        v3Rotation.eulerAngles = new Vector3(minRttX, AxisRtt.y, v3Rotation.z);
        cameraMan.Axis.rotation = v3Rotation;
        cameraMan.CameraMain.transform.LookAt(cameraMan.Axis.position);

        //ī�޶���� ��ġ
        CameraManPos = cameraMan.transform.position;
        cameraMan.transform.position += (PlayerObject.position - CameraManPos) * manMoveSpeed;

        //ī�޶� ���⿡ ���� �ɸ��� ����
        float offset = Time.deltaTime * moveSpeedSum;
        if (moveH != 0 || moveV != 0)
        {
            Vector3 tempAngles = cameraMan.Axis.eulerAngles;
            tempAngles.x = offset;
            Vector3 plusAngles = new Vector3(0, Mathf.Atan2(moveH, moveV) * Mathf.Rad2Deg, 0);
            Vector3 sumAngles = (tempAngles + plusAngles);

            //�̵�
            PlayerObject.eulerAngles = sumAngles;
        }
        PlayerObject.position += PlayerObject.forward * offset;
    }

    RaycastHit rayHit;
    void ObjectRaycast()
    {
        float distance = 50f;
        int layerMask = 1 << LayerMask.NameToLayer("FindObject");  // FindObject ���̾ �浹 üũ��

        if (Physics.Raycast(cameraMan.CameraMain.position, cameraMan.CameraMain.forward, out rayHit, distance, layerMask))
        {
            GameObject temp = rayHit.transform.gameObject;
        }
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
        mAnimator.SetTrigger("Jump");
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
