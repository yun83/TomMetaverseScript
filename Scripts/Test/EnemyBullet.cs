using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public bool Move = false;
    public float Speed = 0.1f;

    public List<Vector3> MovePoint = new List<Vector3>();
    public int moveIndex = 0;

    public GameObject childObject;
    // Start is called before the first frame update
    void Start()
    {
    }

    //// Update is called once per frame
    //void Update()
    //{
    //}

    private void FixedUpdate()
    {
        if (Move)
        {
            //transform.position = Vector3.MoveTowards(transform.position, MovePoint[0], Speed);

            if(MovePoint.Count > 0)
            {
                float dis = Vector3.Distance(MovePoint[moveIndex], transform.position);

                if(dis <= 0.05f)
                {
                    moveIndex++;
                }

                if (moveIndex >= MovePoint.Count)
                {
                    Invoke("DeleteObject", 1f);
                    Move = false;
                }
                else
                {
                    transform.LookAt(MovePoint[moveIndex]);
                    transform.position = Vector3.MoveTowards(transform.position, MovePoint[moveIndex], Speed);
                }
            }
        }
    }


    void DeleteObject()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("Player hit " + other.gameObject.name);
        }
    }
}
