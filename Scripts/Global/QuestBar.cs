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

    int QuestIdx;
    public int Q_ID;

    QuestVer2Data qv2Data;

    public void ScrollCellIndex(int idx)
    {
        //Q_ID = DataInfo.ins.dailyQuest.QuiteListId[idx];
        //Title.text =DataInfo.ins.QuestData[Q_ID].Name;
        //Description.text = DataInfo.ins.QuestData[Q_ID].Description;

        //if(DataInfo.ins.dailyQuest.QuiteListState[idx] <= 0)
        //    CheckMark.SetActive(false);
        //else
        //    CheckMark.SetActive(true);

        QuestIdx = DataInfo.ins.QuestIdx;
        qv2Data = DataInfo.ins.QVer2[QuestIdx][idx];

        Title.text = qv2Data.Name;
        Description.text = qv2Data.Description;
        Reward.text = qv2Data.Reward + "Gold";

        if(qv2Data.State <= 0)
            CheckMark.SetActive(false);
        else
            CheckMark.SetActive(true);
    }

    public void OnClick_Evenet()
    {
    }
}