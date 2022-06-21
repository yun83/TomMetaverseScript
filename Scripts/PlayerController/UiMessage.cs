using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiMessage : MonoBehaviour
{
    public Image PopupBackGround;
    public Text Msg;
    bool AlphaAniCheck = false;
    Color SetColor = new Color(1, 1, 1, 1);
    
    public void OnMessage(string getMsg , float mTime = 1f)
    {
        Msg.text = getMsg;
        gameObject.SetActive(true);
        AlphaAniCheck = false;
        SetColor.a = 1;
        PopupBackGround.color = SetColor;

        mTime -= 0.5f;
        Invoke("PopupOff", mTime);
    }

    void PopupOff()
    {
        if (!AlphaAniCheck)
            StartCoroutine(OffAni());
    }

    IEnumerator OffAni()
    {
        var callTime = new WaitForSeconds(0.1f);

        AlphaAniCheck = true;
        yield return null;

        while (SetColor.a > 0)
        {
            SetColor.a -= 0.2f;
            PopupBackGround.color = SetColor;
            yield return callTime;
        }

        yield return null;

        AlphaAniCheck = false;
        gameObject.SetActive(false);
    }
}
