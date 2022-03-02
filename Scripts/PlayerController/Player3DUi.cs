using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player3DUi : MonoBehaviour
{
    public Text NicName;
    private void FixedUpdate()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
