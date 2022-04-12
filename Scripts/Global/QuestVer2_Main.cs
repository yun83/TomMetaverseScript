using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestVer2_Main : MonoBehaviour
{
    public Text Title;
    string QName = "";
    Image outlineImg;
    Button NowButton;
    int Index = -1;

    public void ScrollCellIndex(int idx)
    {
        outlineImg = GetComponent<Image>();

        switch (idx)
        {
            case 0: QName = "ÀÏÀÏÄù½ºÆ®"; break;
            case 1: QName = "¸¶ÀÌ·ë Äù½ºÆ®"; break;
            case 2: QName = "¿ùµå Äù½ºÆ®"; break;
            case 3: QName = "Ä«Æä Äù½ºÆ®"; break;
        }
        Index = idx;
        Title.text = QName;

        NowButton = GetComponent<Button>();

        NowButton.onClick.RemoveAllListeners();
        NowButton.onClick.AddListener(OnClick_Evenet);

        if (DataInfo.ins.QuestIdx == Index)
            outlineImg.color = Color.blue;
        else
            outlineImg.color = Color.white;
    }

    public void OnClick_Evenet()
    {
        Debug.Log(QName);
        DataInfo.ins.GameUI.QuestPopup.Click_CallBack(Index);
    }
}
