using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Text Title;
    public Text Description;
    public GameObject CheckMark;

    public int Q_ID;

    public void ScrollCellIndex(int idx)
    {
        //DataInfo.ins.dailyQuest.QuiteListId
        Q_ID = DataInfo.ins.dailyQuest.QuiteListId[idx];
        Title.text =DataInfo.ins.QuestData[Q_ID].Name;
        Description.text = DataInfo.ins.QuestData[Q_ID].Description;

        if(DataInfo.ins.dailyQuest.QuiteListState[idx] <= 0)
            CheckMark.SetActive(false);
        else
            CheckMark.SetActive(true);
    }

    public void OnClick_Evenet()
    {
    }
}