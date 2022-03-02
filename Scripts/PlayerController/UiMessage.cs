using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiMessage : MonoBehaviour
{
    public Text Msg;

    public float useTime = 0.1f;
    float startTime;

    private void OnEnable()
    {
        startTime = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
        if(startTime + useTime < Time.time)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnMessage(string getMsg , float mTime = 1f)
    {
        Msg.text = getMsg;
        useTime = mTime;
        gameObject.SetActive(true);
    }
}
