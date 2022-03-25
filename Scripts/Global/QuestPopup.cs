using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPopup : MonoBehaviour
{
    public Text Title;
    public InitScroll setScroll;
    public Button SuccesButton;
    public Text SuccesText;

    private void OnEnable()
    {
        Title.text = DataInfo.ins.dailyQuest.nameText;
        SuccesText.text = "º¸»ó : "+ DataInfo.ins.dailyQuest.GoldReward + "Gold";

        if (DataInfo.ins.Quest_WinState == 1)
        {
            SuccesButton.interactable = true;
        }
        else 
        {
            SuccesButton.interactable = false;
        }

        SuccesButton.onClick.RemoveAllListeners();
        SuccesButton.onClick.AddListener(RewardGet);

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

    void RewardGet()
    {
        if(DataInfo.ins.Quest_WinState == 1)
        {
            DataInfo.ins.CharacterMain.Money += DataInfo.ins.dailyQuest.GoldReward;
            DataInfo.ins.Quest_WinState = 2;

            DataInfo.ins.SaveData = JsonUtility.ToJson(DataInfo.ins.CharacterMain);
            SuccesButton.interactable = false;
        }
    }
}
