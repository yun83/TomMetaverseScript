using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureButtonItem : MonoBehaviour
{
    public int ButtonId = -1;
    Button NowButton;
    Text getText;

    info_Costume ItemInfo;
    public void ScrollCellIndex(int idx)
    {
        ButtonId = idx;
        
        ItemInfo = DataInfo.ins.CostumeScrollList[ButtonId];

        NowButton = GetComponent<Button>();
        getText = GetComponentInChildren<Text>();

        NowButton.onClick.RemoveAllListeners();
        NowButton.onClick.AddListener(OnClick_Evenet);

        getText.text = ItemInfo.Name + "\n" + ItemInfo.Description;
    }

    public void OnClick_Evenet()
    {
        //Debug.Log(" ButtonClickEvnet " + ButtonId);
        DataInfo.ins.MyPlayerAnimator.SetInteger("Emotion", ItemInfo.Path);
    }
}
