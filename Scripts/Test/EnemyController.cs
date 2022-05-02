using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float runSpeed = 4;
    public Transform UserTrans;
    public NavMeshAgent nav;
    public Animator anim;

    public int eState = 0;
    // Start is called before the first frame update
    void Start()
    {
        nav = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();

        UserTrans = DataInfo.ins.myPlayer.PetLookPos;

        //Vector3 StartPos = myPlayerTrans.position;
        //StartPos.y = 0.08f;
        //StartPos.z += 1f;
        //transform.position = StartPos;

        eState = 0;
        TargetSearch();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TargetSearch()
    {
        nav.SetDestination(UserTrans.position);

        if (eState == 0)
        {
            Invoke("TargetSearch", 1f);
        }
    }
}
