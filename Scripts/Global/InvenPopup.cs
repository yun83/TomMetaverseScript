using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenPopup : MonoBehaviour
{
    public CharacterManager InvenChar;
    public InitScroll ScrollView;

    public Button[] InvenButton;

    void Start()
    {
        InvenButton[0].onClick.RemoveAllListeners();
        InvenButton[0].onClick.AddListener(OnClick_Item);
        InvenButton[1].onClick.RemoveAllListeners();
        InvenButton[1].onClick.AddListener(OnClick_Gesture);
    }
    private void OnEnable()
    {
        scrollViewSetting();
        CharacterSetting();
    }

    void FixedUpdate()
    {
        if (DataInfo.ins.CharacterChangeCheck)
        {
            InvenChar.itemEquipment(DataInfo.ins.CharacterSub);
            DataInfo.ins.CharacterChangeCheck = false;
        }
    }

    private void CharacterSetting()
    {
        if (!DataInfo.ins.SaveData.Equals("") && DataInfo.ins.SaveData != null)
        {
            DataInfo.ins.CharacterSub = JsonUtility.FromJson<Info_Char>(DataInfo.ins.SaveData);
        }

        //NicName.text = DataInfo.ins.CharacterSub.NicName;
        InvenChar.itemEquipment(DataInfo.ins.CharacterSub);
        DataInfo.ins.SubCharAnimator = InvenChar.GetComponentInChildren<Animator>();
        //MoneyText.text = DataInfo.ins.CharacterMain.Money.ToString();
    }

    void scrollViewSetting()
    {
        //아이템 리스트 셋팅
        DataInfo.ins.CostumeScrollList.Clear();
        for (int i = 0; i < DataInfo.ins.CoustumList.Length; i++)
        {
            for (int k = 0; k < DataInfo.ins.CoustumList[i].Count; k++)
            {
                //아이템 데이터에 추천 항목 축가 하여 추천 검사
                if (DataInfo.ins.CoustumList[i][k].State == 1)
                {
                    if (DataInfo.ins.CoustumList[i][k].Sex == DataInfo.ins.CharacterSub.Sex
                        || DataInfo.ins.CoustumList[i][k].Sex == 2)
                    {
                        DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[i][k]);
                    }
                }
            }
        }

        StartCoroutine(ScrollViewSetting());
    }

    void OnClick_Item() {
        scrollViewSetting();
    }
    void OnClick_Gesture()
    {
        //아이템 리스트 셋팅
        DataInfo.ins.CostumeScrollList.Clear();
        for (int i = 0; i < DataInfo.ins.EctItemData.Count; i++)
        {
            //아이템 데이터에 추천 항목 축가 하여 추천 검사
            if (DataInfo.ins.EctItemData[i].State == 1)
            {
                DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.EctItemData[i]);
            }
        }

        StartCoroutine(ScrollViewSetting());
    }

    IEnumerator ScrollViewSetting()
    {
        yield return null;

        ScrollView.gameObject.SetActive(true);
        DataInfo.ins.ItemSelectIndex = -1;

        yield return null;

        ScrollView.totalCount = DataInfo.ins.CostumeScrollList.Count;
        ScrollView.InitScrollCall();
    }
}
