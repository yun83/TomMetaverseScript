using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttArea : MonoBehaviour
{
    public bool HitCheck = false;
    // Start is called before the first frame update
    void Start()
    {
        HitCheck = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            HitCheck = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            HitCheck = false;
        }
    }
}
