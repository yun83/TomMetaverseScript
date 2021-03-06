using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player3DUi : MonoBehaviour
{
    public Text NicName;
    private Transform MainCam;

    void OnEnable()
    {
        MainCam = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if(MainCam != null)
            transform.LookAt(MainCam.position);
    }
}
