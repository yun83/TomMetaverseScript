using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureButtonItem : MonoBehaviour
{
    public int ButtonId = -1;
    public Image IconImage;
    Button NowButton;
    Text getText;

    CoustumItemCsv ItemInfo;
    public void ScrollCellIndex(int idx)
    {
        string iconName;

        ButtonId = idx;
        
        ItemInfo = DataInfo.ins.CostumeScrollList[ButtonId];

        NowButton = GetComponent<Button>();
        getText = GetComponentInChildren<Text>();

        NowButton.onClick.RemoveAllListeners();
        NowButton.onClick.AddListener(OnClick_Evenet);

        getText.text = ItemInfo.Name;

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
    
    // 이 함수는 목록 내용을 업데이트하기 위해 `CircularScrollingList`에 의해 호출됩니다.
    // 콘텐츠의 유형은 `IntListBank`에서 `object`로 변환됩니다. (나중에 정의됨)
    // 따라서 사용하려면 다시 자체 유형으로 변환해야 합니다.
    // 콘텐츠의 원래 유형은 'int'입니다.
    //protected override void UpdateDisplayContent(object content)
    //{
    //    //_contentText.text = ((int)content).ToString();
    //    ScrollCellIndex(((int)content));
    //}
}
