using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Text Title;
    public Text Description;
    public Text Reward;
    public GameObject CheckMarkBox;
    public GameObject CheckMark;

    Image BackGround;
    Button ClickEvent;

    public Color NonStart = new Color(0.85f, 0.85f, 0.85f);
    public Color Start = Color.white;
    public Color Win = new Color(0.65f, 0.9f, 1f);
    public Color End = new Color(0.5f, 0.5f, 0.5f);

    public Color textColor_Non = new Color(0.2f, 0.2f, 0.2f, 1f);
    public Color textColor_End = new Color(0.2f, 0.2f, 0.2f, 0.5f);

    int QuestIdx;
    public int Q_ID;

    QuestVer2Data qv2Data;

    public void ScrollCellIndex(int idx)
    {
        BackGround = GetComponent<Image>();
        ClickEvent = GetComponent<Button>();

        QuestIdx = DataInfo.ins.QuestIdx;
        qv2Data = DataInfo.ins.QVer2[QuestIdx][idx];

        Title.text = qv2Data.Name;
        Description.text = qv2Data.Description;
        Reward.text = qv2Data.Reward + "Gold";

        Title.color = textColor_Non;
        Description.color = textColor_Non;
        Reward.color = textColor_Non;

        ClickEvent.interactable = true;
        switch (qv2Data.State)
        {
            case 0:
                BackGround.color = NonStart;
                break;
            case 1:
                BackGround.color = Start;
                break;
            case 2:
                break;
            case 3:
                BackGround.color = Win;
                break;
            case 4:
                break;
            case 5:
                BackGround.color = End;
                Title.color = textColor_End;
                Description.color = textColor_End;
                Reward.color = textColor_End;
                ClickEvent.interactable = false;
                break;
        }

        if (qv2Data.State < 2)
            CheckMark.SetActive(false);
        else
            CheckMark.SetActive(true);

        ClickEvent.onClick.RemoveAllListeners();
        ClickEvent.onClick.AddListener(OnClick_Evenet);
    }

    public void OnClick_Evenet()
    {
        if (qv2Data.State == 3)
        {
            //Debug.Log("¸®¿öµå È¹µæ :: " + qv2Data.Reward);
            qv2Data.State = 4;

            DataInfo.ins.AddMoney(qv2Data.Reward);
            BackGround.color = new Color(0.5f, 0.5f, 0.5f);
            Invoke("RefillCells", 0.1f);
        }
    }

    void RefillCells()
    {
        DataInfo.ins.GameUI.QuestPopup.Click_CallBack(DataInfo.ins.QuestIdx);
    }
}