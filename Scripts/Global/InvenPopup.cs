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
    public GameObject SubButton;
    public GameObject[] SelectDetailedMenu;
    private RectTransform scrollRect;

    void Start()
    {
        InvenButton[0].onClick.RemoveAllListeners();
        InvenButton[0].onClick.AddListener(OnClick_Item);
        InvenButton[1].onClick.RemoveAllListeners();
        InvenButton[1].onClick.AddListener(OnClick_Gesture);
    }
    private void OnEnable()
    {
        scrollRect = ScrollView.GetComponent<RectTransform>();

        CharacterSetting();
        scrollViewSetting();
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

        InvenChar.itemEquipment(DataInfo.ins.CharacterSub);
        DataInfo.ins.SubCharAnimator = InvenChar.GetComponentInChildren<Animator>();
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

        SubButton.SetActive(true);
        ClickCheckClose(0);
        StartCoroutine(ScrollViewSetting());
        scrollRect.offsetMax = (new Vector2(0, -150));
    }

    void OnClick_Item()
    {
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
        scrollViewSetting();
    }
    void OnClick_Gesture()
    {
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
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
        SubButton.SetActive(false);
        StartCoroutine(ScrollViewSetting());
        scrollRect.offsetMax = (new Vector2(0, -90));
    }

    #region detailed Menu Button Setting
    void ClickCheckClose(int idx)
    {
        for (int i = 0; i < SelectDetailedMenu.Length; i++)
        {
            SelectDetailedMenu[i].SetActive(false);
        }
        SelectDetailedMenu[idx].SetActive(true);
    }

    public void OnClick_detailedMenu_All()
    {
        ClickCheckClose(0);
        //아이템 리스트 셋팅
        DataInfo.ins.CostumeScrollList.Clear();
        for (int i = 0; i < DataInfo.ins.CoustumList.Length; i++)
        {
            for (int k = 0; k < DataInfo.ins.CoustumList[i].Count; k++)
            {
                if (DataInfo.ins.CoustumList[i][k].State == 1)
                {
                    //성별 검사 하여 현재 나에게 맞는 성멸의 아이템만 출력
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
    public void OnClick_detailedMenu_Hair()
    {
        ClickCheckClose(1);
        //아이템 리스트 셋팅
        DataInfo.ins.CostumeScrollList.Clear();
        for (int k = 0; k < DataInfo.ins.CoustumList[0].Count; k++)
        {
            if (DataInfo.ins.CoustumList[0][k].State == 1)
            {
                //성별 검사 하여 현재 나에게 맞는 성멸의 아이템만 출력
                if (DataInfo.ins.CoustumList[0][k].Sex == DataInfo.ins.CharacterSub.Sex
                || DataInfo.ins.CoustumList[0][k].Sex == 2)
                {
                    DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[0][k]);
                }
            }
        }

        StartCoroutine(ScrollViewSetting());
    }
    public void OnClick_detailedMenu_Shirt()
    {
        ClickCheckClose(2);
        //아이템 리스트 셋팅
        DataInfo.ins.CostumeScrollList.Clear();
        for (int k = 0; k < DataInfo.ins.CoustumList[1].Count; k++)
        {
            if (DataInfo.ins.CoustumList[1][k].State == 1)
            {
                //성별 검사 하여 현재 나에게 맞는 성멸의 아이템만 출력
                if (DataInfo.ins.CoustumList[1][k].Sex == DataInfo.ins.CharacterSub.Sex
                || DataInfo.ins.CoustumList[1][k].Sex == 2)
                {
                    DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[1][k]);
                }
            }
        }

        StartCoroutine(ScrollViewSetting());
    }
    public void OnClick_detailedMenu_Pants()
    {
        ClickCheckClose(3);
        //아이템 리스트 셋팅
        DataInfo.ins.CostumeScrollList.Clear();
        for (int k = 0; k < DataInfo.ins.CoustumList[2].Count; k++)
        {
            if (DataInfo.ins.CoustumList[2][k].State == 1)
            {
                //성별 검사 하여 현재 나에게 맞는 성멸의 아이템만 출력
                if (DataInfo.ins.CoustumList[2][k].Sex == DataInfo.ins.CharacterSub.Sex
                || DataInfo.ins.CoustumList[2][k].Sex == 2)
                {
                    DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[2][k]);
                }
            }
        }

        StartCoroutine(ScrollViewSetting());
    }
    public void OnClick_detailedMenu_Shoes()
    {
        ClickCheckClose(4);
        //아이템 리스트 셋팅
        DataInfo.ins.CostumeScrollList.Clear();
        for (int k = 0; k < DataInfo.ins.CoustumList[3].Count; k++)
        {
            if (DataInfo.ins.CoustumList[3][k].State == 1)
            {
                //성별 검사 하여 현재 나에게 맞는 성멸의 아이템만 출력
                if (DataInfo.ins.CoustumList[3][k].Sex == DataInfo.ins.CharacterSub.Sex
                || DataInfo.ins.CoustumList[3][k].Sex == 2)
                {
                    DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[3][k]);
                }
            }
        }

        StartCoroutine(ScrollViewSetting());
    }
    public void OnClick_detailedMenu_Set()
    {
        ClickCheckClose(5);
        //아이템 리스트 셋팅
        DataInfo.ins.CostumeScrollList.Clear();
        for (int k = 0; k < DataInfo.ins.CoustumList[4].Count; k++)
        {
            if (DataInfo.ins.CoustumList[4][k].State == 1)
            {
                //성별 검사 하여 현재 나에게 맞는 성멸의 아이템만 출력
                if (DataInfo.ins.CoustumList[4][k].Sex == DataInfo.ins.CharacterSub.Sex
                || DataInfo.ins.CoustumList[4][k].Sex == 2)
                {
                    DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[4][k]);
                }
            }
        }

        StartCoroutine(ScrollViewSetting());
    }
    public void OnClick_detailedMenu_Accessory()
    {
        ClickCheckClose(6);
        //아이템 리스트 셋팅
        DataInfo.ins.CostumeScrollList.Clear();
        for (int k = 0; k < DataInfo.ins.CoustumList[5].Count; k++)
        {
            if (DataInfo.ins.CoustumList[5][k].State == 1)
            {
                //성별 검사 하여 현재 나에게 맞는 성멸의 아이템만 출력
                if (DataInfo.ins.CoustumList[5][k].Sex == DataInfo.ins.CharacterSub.Sex
                || DataInfo.ins.CoustumList[5][k].Sex == 2)
                {
                    DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[5][k]);
                }
            }
        }

        StartCoroutine(ScrollViewSetting());
    }
    #endregion
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
