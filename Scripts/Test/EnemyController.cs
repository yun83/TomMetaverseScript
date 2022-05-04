using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum MobType
    {
        �ٰŸ� = 0,
        ���Ÿ�,
        ������,
    };

    public MobType EnemyType = MobType.�ٰŸ�;
    public float AttSpeed = 3.25f;

    [Header("���õǴ� ������Ʈ")]
    public Transform UserTrans;
    public NavMeshAgent nav;
    public Animator anim;

    [Header("�������")]
    public int eState = 0;
    private float SumTime = 0;

    [Header("�ٰŸ� ����üũ")]
    public EnemyAttArea Area_Short;

    // Start is called before the first frame update
    void Start()
    {
        nav = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();

        UserTrans = DataInfo.ins.myPlayer.transform;

        Area_Short = transform.GetComponentInChildren<EnemyAttArea>();

        Area_Short.gameObject.SetActive(false);
        switch (EnemyType)
        {
            case MobType.�ٰŸ�:
                Area_Short.gameObject.SetActive(true);
                break;
        }
        eState = 0;
        TargetSearch();
    }

    // Update is called once per frame
    void Update()
    {
        eStateLogic();
    }

    void TargetSearch()
    {
        bool nectStateCheck = false;
        float dis = Vector3.Distance(UserTrans.position, transform.position);
        switch (EnemyType)
        {
            case MobType.�ٰŸ�:
                if (dis < 1.2f)
                    nectStateCheck = true;
                break;
            case MobType.���Ÿ�:
                if (dis < 8f)
                    nectStateCheck = true;
                break;
            case MobType.������:
                if (dis < 6f)
                    nectStateCheck = true;
                break;
        }

        if (nectStateCheck)
        {
            NavMoveStop();
            eState = 10;
        }
        else
        {
            nav.destination = UserTrans.position;
        }
    }

    void eStateLogic()
    {
        switch (eState)
        {
            case 0:
                float temp = nav.velocity.magnitude;
                anim.SetFloat("Speed", temp);
                TargetSearch();
                break;
            case 10: //Att time 1.1f
                transform.LookAt(UserTrans);
                anim.SetTrigger("Att");
                eState = 99;
                if (AttSpeed < 1.2f)
                    AttSpeed = 1.2f;
                SumTime = Time.time + AttSpeed;
                break;
            case 20: //Demage time 0.3f
                anim.SetTrigger("Hit");
                eState = 99;
                SumTime = Time.time + 1.0f;
                break;
            case 99:
                if (SumTime < Time.time)
                {
                    NavStartMove();
                    TargetSearch();
                    eState = 0;
                }
                break;
        }
    }

    void NavMoveStop()
    {
        nav.ResetPath();
        nav.isStopped = true;
        nav.updatePosition = false;
        nav.updateRotation = false;
        nav.velocity = Vector3.zero;
        anim.SetFloat("Speed", 0);
    }

    void NavStartMove()
    {
        nav.isStopped = false;
        nav.updatePosition = true;
        nav.updateRotation = true;
    }
}
