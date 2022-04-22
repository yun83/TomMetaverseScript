using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureButtonItem : MonoBehaviour
{
    public int ButtonId = -1;
    public Image IconImage;
    Button NowButton;
    //Text getText;

    CoustumItemCsv ItemInfo;
    public void ScrollCellIndex(int idx)
    {
        string iconName;

        ButtonId = idx;
        
        ItemInfo = DataInfo.ins.CostumeScrollList[ButtonId];

        NowButton = GetComponent<Button>();
        //getText = GetComponentInChildren<Text>();

        NowButton.onClick.RemoveAllListeners();
        NowButton.onClick.AddListener(OnClick_Evenet);

        //getText.text = ItemInfo.Name;

        iconName = "Emotion/" + ItemInfo.Path + "_" + ItemInfo.Description;
        IconImage.sprite = Resources.Load<Sprite>(iconName);
    }

    public void OnClick_Evenet()
    {
        //Debug.Log(" ButtonClickEvnet " + ButtonId);
        //DataInfo.ins.MyPlayerAnimator.SetInteger("Emotion", ItemInfo.Path);
        if(DataInfo.ins.CharacterMain.Sex == 1)
            Com.ins.AniSetInt(DataInfo.ins.MyPlayerAnimator, "Emotion", ItemInfo.Path + 100);
        else
            Com.ins.AniSetInt(DataInfo.ins.MyPlayerAnimator, "Emotion", ItemInfo.Path);

        DataInfo.ins.WinQuest(5);
    }
}
