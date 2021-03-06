using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPopupVer2 : MonoBehaviour
{
    public Button EscButton;

    public InitScroll MainQuestScroll;
    public InitScroll SubQuestScroll;

    List<Button> ScrollButton = new List<Button>();
    // Start is called before the first frame update
    void Start()
    {
        EscButton.onClick.RemoveAllListeners();
        EscButton.onClick.AddListener(() => {
            DataInfo.ins.GameUI.OnClick_CloseAllPopup();
            Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
        });

        DataInfo.ins.QuestIdx = 0;
        switch (DataInfo.ins.State)
        {
            default:
                Click_CallBack(0);
                break;
            case 1:
                Click_CallBack(1);
                break;
            case 2:
                Click_CallBack(2);
                break;
            case 3:
                Click_CallBack(3);
                break;
        }
    }

    private void OnEnable()
    {
        MainQuestScroll.totalCount = 4;
        MainQuestScroll.InitScrollCall();

        SubQuestScroll.totalCount = DataInfo.ins.QVer2[DataInfo.ins.QuestIdx].Count;
        SubQuestScroll.InitScrollCall();
    }

    public void Click_CallBack(int value)
    {
        DataInfo.ins.QuestIdx = value;
        SubQuestScroll.totalCount = DataInfo.ins.QVer2[value].Count;
        SubQuestScroll.InitScrollCall();

        MainQuestScroll.totalCount = 4;
        MainQuestScroll.InitScrollCall();
    }
}
