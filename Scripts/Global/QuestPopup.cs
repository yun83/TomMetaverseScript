using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPopup : MonoBehaviour
{
    public Text Title;
    public InitScroll setScroll;

    private void OnEnable()
    {
        Title.text = DataInfo.ins.dailyQuest.nameText;

        StartCoroutine(QuestSetting());
    }

    IEnumerator QuestSetting()
    {
        yield return null;

        if (setScroll == null)
            setScroll = transform.GetComponentInChildren<InitScroll>();

        yield return new WaitForEndOfFrame();

        setScroll.totalCount = DataInfo.ins.dailyQuest.QuiteListId.Count;
        setScroll.InitScrollCall();
    }
}
