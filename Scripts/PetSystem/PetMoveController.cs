using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetMoveController : MonoBehaviour
{
    public Transform myPlayerTrans;
    public Transform Look;
    public Animator PetAni;
    public Rigidbody rigid;

    public int aniMoveState = 0;
    public float PlayerDis;
    public float PlayerMoveSpeed = 2;
    private float SumSpeed = 0;
    private GameObject InteractionObj = null;

    public bool ShowObject  = false;
    // Start is called before the first frame update
    void Start()
    {
        PetAni = GetComponent<Animator>();

        GameObject temp = new GameObject("타겟검색");
        temp.transform.parent = transform;

        gameObject.tag = "Pet";
        gameObject.layer = LayerMask.NameToLayer("TouchLayer");
        Look = temp.transform;

        InteractionObj = Instantiate(Resources.Load<GameObject>("Prefabs/PetInteraction"));
        InteractionObj.transform.parent = transform;
        InteractionObj.transform.position = new Vector3(0, 0.8f, 0);
        InteractionObj.SetActive(false);

        aniMoveState = 0;
    }

    private void FixedUpdate()
    {
        if (myPlayerTrans == null)
        {
            return;
        }
        PlayerDis = Vector3.Distance(transform.position, myPlayerTrans.position);

        PetMove();
    }

    private void PetMove()
    {
        if (PlayerDis > 0.7f)
            LookPlayer();

        switch (aniMoveState)
        {
            case 0:
                SumSpeed = 0;
                if (PlayerDis > 3)
                    aniMoveState = 1;
                else if (PlayerDis > 6)
                    aniMoveState = 2;


                if (!ShowObject)
                    InteractionObj.SetActive(false);
                else
                    InteractionObj.SetActive(true);
                break;
            case 1:
                InteractionObj.SetActive(false);
                Com.ins.AniSetInt(PetAni, "Move", 1);

                SumSpeed = PlayerMoveSpeed;
                if (PlayerDis > 5)
                    aniMoveState = 2;
                break;
            case 2:
                Com.ins.AniSetInt(PetAni, "Move", 2);

                SumSpeed = PlayerMoveSpeed + PlayerMoveSpeed;
                if (PlayerDis > 7)
                    aniMoveState = 3;
                break;
            case 3:
                Com.ins.AniSetInt(PetAni, "Move", 2);

                SumSpeed = PlayerMoveSpeed + PlayerMoveSpeed + PlayerMoveSpeed + 1f;
                if (PlayerDis < 5)
                    aniMoveState = 2;
                break;
        }

        if (aniMoveState != 0 && PlayerDis < 2f)
        {
            PetAni.SetInteger("Move", 0);
            aniMoveState = 0;
        }

        if (aniMoveState != 0)
        {
            float offset = Time.deltaTime * SumSpeed;
            transform.position += transform.forward * offset;
        }
    }

    void LookPlayer()
    {
        //플레이어 바라보기
        Look.LookAt(myPlayerTrans);

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

    public void OnClick_Evnet_0() {
    }
    public void OnClick_Evnet_1()
    {
    }
    public void OnClick_Evnet_2()
    {
    }
}
