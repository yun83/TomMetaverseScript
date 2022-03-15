using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InteractionCanvas : MonoBehaviour
{
    public Text NicText;
    public Button UseButton;
    private Transform MainCam;

    void Awake()
    {
        MainCam = Camera.main.transform;
    }
    private void FixedUpdate()
    {
        if(MainCam != null)
            transform.LookAt(MainCam.position);
    }
}
