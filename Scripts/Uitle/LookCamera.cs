using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCamera : MonoBehaviour
{
    private Transform MainCam;
    // Start is called before the first frame update
    void Start()
    {
        MainCam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (MainCam != null)
            transform.LookAt(MainCam.position);
    }
}
