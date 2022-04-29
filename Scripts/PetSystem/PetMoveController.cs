using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetMoveController : MonoBehaviour
{
    public LayerMask GroundLayers = -1;
    public Transform myPlayerTrans;
    public Transform Look;
    public Animator PetAni;
    public Rigidbody rigid;
    public GameObject InteractionObj = null;

    public int aniMoveState = 0;
    public float PlayerDis;
    public float PlayerMoveSpeed = 2;
    private float SumSpeed = 0;
    public int AniSetIntIndx = 0;
    Vector3 movePos;

    public bool ShowObject = false;
    float NonAniTime = 0;

    float CheckTime = 0;
    public bool MoveStop = false;
    // Start is called before the first frame update
    void Start()
    {
        PetAni = GetComponent<Animator>();

        GameObject temp = new GameObject("타겟검색");
        temp.transform.parent = transform;

        gameObject.tag = "Pet";
        gameObject.layer = LayerMask.NameToLayer("TouchLayer");
        Look = temp.transform;

        myPlayerTrans = DataInfo.ins.myPlayer.PetLookPos;
        Vector3 StartPos = myPlayerTrans.position;
        StartPos.y = 0.08f;
        StartPos.z += 1f;
        transform.position = StartPos;

        InteractionObj = Instantiate(Resources.Load<GameObject>("Prefabs/PetInteraction"));
        InteractionObj.transform.parent = transform;
        InteractionObj.transform.localPosition = new Vector3(0, 0.8f, 0);
        InteractionObj.SetActive(false);

        movePos = transform.position;
        aniMoveState = 0;

        DataInfo.ins.PetController = this;
    }

    //private void FixedUpdate(){ }
    private void Update()
    {
        if (!MoveStop)
        {
            PetMoveVer3();
        }
    }

    private void PetMoveVer3()
    {
        Vector3 StartPos = myPlayerTrans.position;
        StartPos.y = 0.08f;
        PlayerDis = Vector3.Distance(transform.position, StartPos);

        if (PlayerDis > 0.7f)
            LookPlayer();

        switch (aniMoveState)
        {
            case 0:
                SumSpeed = 0;
                if (PlayerDis > 0.8f)
                    aniMoveState = 1;
                else if (PlayerDis > 1.5f)
                    aniMoveState = 2;


                if (!ShowObject)
                    InteractionObj.SetActive(false);
                else
                    InteractionObj.SetActive(true);
                break;
            case 1:
                ShowObject = false;
                InteractionObj.SetActive(false);
                if (AniSetIntIndx != 1)
                {
                    AniSetIntIndx = 1;
                    Com.ins.AniSetInt(PetAni, "Move", AniSetIntIndx);
                }

                SumSpeed = PlayerMoveSpeed;
                if (PlayerDis > 1.5f)
                    aniMoveState = 2;
                break;
            case 2:
                if (AniSetIntIndx != 2)
                {
                    AniSetIntIndx = 2;
                    Com.ins.AniSetInt(PetAni, "Move", AniSetIntIndx);
                }

                SumSpeed = PlayerMoveSpeed + PlayerMoveSpeed;
                if (PlayerDis > 2)
                    aniMoveState = 3;
                else if (PlayerDis <= 1f)
                    aniMoveState = 1;
                break;
            case 3:
                if (AniSetIntIndx != 2)
                {
                    AniSetIntIndx = 2;
                    Com.ins.AniSetInt(PetAni, "Move", AniSetIntIndx);
                }

                SumSpeed = PlayerMoveSpeed + PlayerMoveSpeed + PlayerMoveSpeed + 1f;
                if (PlayerDis <= 1.5f)
                    aniMoveState = 2;
                break;
        }

        if (aniMoveState != 0 && PlayerDis < 0.6f)
        {
            if (AniSetIntIndx != 0)
            {
                AniSetIntIndx = 0;
                NonAniTime = Time.time;
                Com.ins.AniSetInt(PetAni, "Move", AniSetIntIndx);
            }
            aniMoveState = 0;
        }

        if (aniMoveState != 0)
        {
            float offset = Time.deltaTime * SumSpeed;
            movePos += transform.forward * offset;
            movePos.y = 0.08f;
            transform.position = movePos;
        }
    }

    void PetMoveVer2()
    {
        int getAniState = DataInfo.ins.infoController.PlayerMoveAniState;
        transform.rotation = DataInfo.ins.infoController.PlayerObject.rotation;

        myPlayerTrans = DataInfo.ins.infoController.PlayerObject;
        Vector3 StartPos = myPlayerTrans.position;
        StartPos.y = 0.08f;
        StartPos.z += 1f;
        transform.position = StartPos;

        if (getAniState == 3)
            getAniState = 2;
        Com.ins.AniSetInt(PetAni, "Move", getAniState);
    }

    void LookPlayer()
    {
        Vector3 StartPos = myPlayerTrans.position;
        StartPos.y = 0.08f;
        //플레이어 바라보기
        Look.LookAt(StartPos);

        Vector3 tempAngles = new Vector3();
        tempAngles.x = 0;
        tempAngles.y = Look.eulerAngles.y;
        tempAngles.z = Look.eulerAngles.z;
        //이동
        transform.eulerAngles = tempAngles;
    }

    public void PetInteraction()
    {
        ShowObject = (ShowObject == false) ? true : false;
    }

    void GroundCheck()
    {
        if (CheckTime > Time.time)
            return;
        
        RaycastHit rayHit;

        float distance = 50f;

        if (Physics.Raycast(transform.position, Vector3.down, out rayHit, distance, GroundLayers))
        {
            Debug.Log(rayHit.transform.name + " :: " + rayHit.point);

        }
        CheckTime = Time.time + 1;
    }

    //Bulldog give paw(손? 하이파이브?)
    //Bulldog idle happy
    //Bulldog idle lay down(앉아?)

    public void OnClick_Evnet_0()
    {
        Com.ins.AniSetInt(PetAni, "Touch", 1);
        DataInfo.ins.WinQuest(4);
    }
    public void OnClick_Evnet_1()
    {
        Com.ins.AniSetInt(PetAni, "Touch", 2);
        DataInfo.ins.WinQuest(4);
    }
    public void OnClick_Evnet_2()
    {
        Com.ins.AniSetInt(PetAni, "Touch", 12);
        DataInfo.ins.WinQuest(4);
    }
}
