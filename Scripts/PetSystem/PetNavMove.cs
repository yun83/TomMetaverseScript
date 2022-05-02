using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PetNavMove : MonoBehaviour
{
    public float runSpeed = 4;
    public Transform myPlayerTrans;
    public NavMeshAgent nav;
    public Animator anim;
    //float saveTime = 0;

    private void Start()
    {
        nav = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();

        myPlayerTrans = DataInfo.ins.myPlayer.PetLookPos;
        Vector3 StartPos = myPlayerTrans.position;
        StartPos.y = 0.08f;
        StartPos.z += 1f;
        transform.position = StartPos;
    }

    private void FixedUpdate()
    {
        PetMoveLogic();
    }

    void PetMoveLogic()
    {
        //nav.destination = myPlayerTrans.position;
        //if (Time.time > saveTime)
        {
            nav.SetDestination(myPlayerTrans.position);
            //saveTime = Time.time + 1f;
        }
        float temp = nav.velocity.sqrMagnitude;
        float temp2 = nav.velocity.magnitude;
        Debug.Log(temp + " ::: " + temp2);

        anim.SetFloat("Speed", temp2);
    }
}
