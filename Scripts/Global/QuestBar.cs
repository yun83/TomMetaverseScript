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

    public int Id;

    public void ScrollCellIndex(int idx)
    {
        //DataInfo.ins.dailyQuest.QuiteListId
        int id = DataInfo.ins.dailyQuest.QuiteListId[idx];
        Title.text = System.Convert.ToString(DataInfo.ins.QuestName[id]);
    }

    public void OnClick_Evenet()
    {
    }
}