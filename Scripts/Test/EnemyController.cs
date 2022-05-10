using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum MobType
    {
        근거리 = 0,
        원거리,
        마법형,
    };

    public MobType EnemyType = MobType.근거리;
    public float AttSpeed = 3.25f;

    [Header("세팅되는 오브젝트")]
    public Transform UserTrans;
    public NavMeshAgent nav;
    public Animator anim;

    [Header("진행사항")]
    public int eState = 0;
    private float SumTime = 0;

    [Header("공격체크")]
    public EnemyAttArea Area_Short;
    public Vector3 PlayerPos;
    public List<Vector3> LinePos = new List<Vector3>();
    public GameObject Bullet;


    private Vector3 PointStart;
    private Vector3 PointOne;
    private Vector3 PointTwo;
    private Vector3 PointEnd;
    //private LineRenderer lineRender;

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
            case MobType.근거리:
                break;
        }
        eState = 0;

        LinePos.Clear();

        //if(lineRender == null)
        //{
        //    lineRender = new GameObject("Line").AddComponent<LineRenderer>();
        //    lineRender.SetWidth(0.1f, 0.1f);
        //    lineRender.SetColors(Color.black, Color.white);
        //    lineRender.useWorldSpace = true;
        //}

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
            case MobType.근거리:
                if (dis < 1.2f)
                    nectStateCheck = true;
                break;
            case MobType.원거리:
                if (dis < 8f)
                    nectStateCheck = true;
                break;
            case MobType.마법형:
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
            case 10: 
                AttMotionSetting();
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

    void AttMotionSetting()
    {
        transform.LookAt(UserTrans);

        PlayerPos = UserTrans.position;

        BezierCurvePosSetting();

        anim.SetTrigger("Att");
        //Att time 1.1f
        if (AttSpeed < 1.2f)
            AttSpeed = 1.2f;

        switch (EnemyType)
        {
            case MobType.근거리:
                Area_Short.gameObject.SetActive(true);
                break;
            case MobType.원거리:
                break;
            case MobType.마법형:
                break;
        }
        SumTime = Time.time + AttSpeed;
        eState = 99;
    }

    void BezierCurvePosSetting()
    {
        float dis = Vector3.Distance(UserTrans.position, transform.position);

        PointStart = transform.position;

        PointOne = Vector3.Lerp(transform.position, UserTrans.position, 0.2f);
        PointTwo = Vector3.Lerp(transform.position, UserTrans.position, 0.8f);
        PointEnd = PlayerPos;
        if (dis < 3) { PointOne.y += 1.5f; PointTwo.y += 1.5f; }
        else if (dis < 5) { PointOne.y += 2; PointTwo.y += 2; }
        else { PointOne.y = 3; PointTwo.y += 3; }

        LinePos.Clear();
        for (float i = 0; i < 1; i += 0.05f)
        {
            LinePos.Add(Com.ins.GetPointOnBezierCurve(PointStart, PointOne, PointTwo, PointEnd, i));
        }

        //lineRender.SetVertexCount(LinePos.Count);
        //lineRender.SetPosition(0, PointStart);
        //for (int i = 1; i < LinePos.Count; i++)
        //{
        //    lineRender.SetPosition(i, LinePos[i]);
        //}

        if (EnemyType == MobType.원거리)
        {
            EnemyBullet eb = Instantiate(Bullet).GetComponent<EnemyBullet>();

            eb.transform.position = PointStart;
            eb.MovePoint = LinePos;
            eb.moveIndex = 0;
            eb.Move = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        switch (EnemyType)
        {
            case MobType.근거리:
                break;
            case MobType.원거리:
                {
                    if (LinePos.Count > 1)
                    {
                        Gizmos.DrawSphere(PointStart, 0.05f);
                        Gizmos.DrawSphere(PointOne, 0.05f);
                        Gizmos.DrawSphere(PointTwo, 0.05f);
                        Gizmos.DrawSphere(PointEnd, 0.05f);

                        for (int i = 0; i < LinePos.Count; i++)
                        {
                            Debug.DrawLine(PointStart, LinePos[i], Color.yellow);
                        }

                        Debug.DrawLine(PointStart, PointOne, Color.red);
                        Debug.DrawLine(PointOne, PointTwo, Color.red);
                        Debug.DrawLine(PointTwo, PointEnd, Color.red);
                    }
                }
                break;
            case MobType.마법형:
                break;
        }
#endif
    }
}
