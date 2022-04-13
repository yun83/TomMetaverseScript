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
    public GameObject CheckMark;
    Image BackGround;
    Button ClickEvent;

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

        switch(qv2Data.State)
        {
            case 0:
                BackGround.color = new Color(0.8f, 0.8f, 0.8f);
                break;
            case 1:
            case 2:
            case 3:
                BackGround.color = Color.white;
                break;
            case 4:
                BackGround.color = new Color(0.5f, 0.5f, 0.5f);
                break;
        }

        if (qv2Data.State < 2)
            CheckMark.SetActive(false);
        else
        {
            CheckMark.SetActive(true);
        }

        ClickEvent.onClick.RemoveAllListeners();
        ClickEvent.onClick.AddListener(OnClick_Evenet);
    }

    public void OnClick_Evenet()
    {
        if (qv2Data.State == 3)
        {
            Debug.Log("¸®¿öµå È¹µæ");
            qv2Data.State = 4;

            BackGround.color = new Color(0.5f, 0.5f, 0.5f);
        }
    }
}