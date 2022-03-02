using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureButtonItem : MonoBehaviour
{
    public int ButtonId = -1;
    Button NowButton;
    Text getText;
    public void ScrollCellIndex(int idx)
    {
        ButtonId = idx;
        NowButton = GetComponent<Button>();
        getText = GetComponentInChildren<Text>();

        NowButton.onClick.RemoveAllListeners();
        NowButton.onClick.AddListener(OnClick_Evenet);

        getText.text = DataInfo.ins.TriggerName[ButtonId];
    }

    public void OnClick_Evenet()
    {
        //Debug.Log(" ButtonClickEvnet " + ButtonId);
        DataInfo.ins.MyPlayerAnimator.SetInteger("Emotion", ButtonId + 1);
    }
}
