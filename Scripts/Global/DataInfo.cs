using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInfo : Single<DataInfo>
{
    public bool LoginCheck = false;
    public bool LodingCheck = false;

    public float ScreenW = 1280;
    public float ScreenH = 720;

    //현재 위치하고 있는 곳에 따른 스테이트
    /// <summary>
    /// 0:케릭터 생성
    /// 1:마이룸
    /// 2:월드맵
    /// </summary>
    public int State = 0;

    //세이브 파일
    public string SaveData
    {
        get { return PlayerPrefs.GetString("CharacterSave", ""); }
        set { PlayerPrefs.SetString("CharacterSave", value); }
    }
    public string saveOption
    {
        get { return PlayerPrefs.GetString("OptionSave", ""); }
        set { PlayerPrefs.SetString("OptionSave", value); }
    }
    public string saveBuyItem
    {
        get { return PlayerPrefs.GetString("saveBuyItem", ""); }
        set { PlayerPrefs.SetString("saveBuyItem", value); }
    }

    public OptionData OptionInfo = new OptionData();

    //케릭터 선택 창에서 사용될 인벤 타입과 선택 된 아이템 번호 저장용
    public int ItemSelectIndex = -1;
    public int InvenNumber = -1;

    //월드에서 이동시에 키 중복 방지를 위해서
    public int TouchId = -1;
    public int RightTId = -1;

    public Animator MyPlayerAnimator;
    public Animator SubCharAnimator;

    //저장할 메인 케릭터 정보
    public Info_Char CharacterMain = new Info_Char();

    //케릭 선택 창에서 사용될 케릭터 정보
    public Info_Char CharacterSub = new Info_Char();
    public bool CharacterChangeCheck = false;

    public List<ButtonClass> OutRoomButton = new List<ButtonClass>();

    //아이템 리스트의 데이터 로딩
    public List<info_Costume>[] CoustumList = new List<info_Costume>[6];
    public List<info_Costume> EctItemData = new List<info_Costume>();

    //스크롤에 등록될 아이템 리스트 정리
    public List<info_Costume> CostumeScrollList = new List<info_Costume>();

    public List<int> BuyItemId = new List<int>(); //구매한 아이템 ID

    //삽에서 구입할 아이템 리스트 작성
    public List<info_Costume> BuyItemSaveList = new List<info_Costume>();
    public bool TotlaMoneySumCheck = false;


    private void Awake()
    {
        Application.targetFrameRate = 1000;

        #region Save Loding
        Debug.Log(" ------------- 세이브 로딩 --------------");
        if (!SaveData.Equals("") && SaveData != null)
        {
            Debug.Log("<color=yellow> Use Character Main Save Data </color>");
            CharacterMain = JsonUtility.FromJson<Info_Char>(SaveData);
        }
        Debug.Log("<color=yellow>Print Character Data</color>" + CharacterMain.printData());

        if (!saveOption.Equals("") && saveOption != null)
        {
            Debug.Log("<color=yellow> Use Option Save Data </color>");
            OptionInfo = JsonUtility.FromJson<OptionData>(saveOption);
        }

        BuyItemId.Clear();
        if (!saveBuyItem.Equals("") && saveBuyItem != null)
        {
            string[] stringList = saveBuyItem.Split(',');
            for (int i = 0; i < stringList.Length - 1; i++)
            {
                BuyItemId.Add(System.Convert.ToInt32(stringList[i]));
            }
        }
        #endregion

        #region Costum Item Data Loding

        List<Dictionary<string, object>> itemCsvDic = CSVReader.Read("Doc/CostumItem");

        //현재 아이템 타입이 6가지
        for (int i = 0; i < CoustumList.Length; i++)
        {
            CoustumList[i] = new List<info_Costume>();
            CoustumList[i].Clear();
        }
        EctItemData.Clear();

        for (var i = 0; i < itemCsvDic.Count; i++)
        {
            //print("ItemID " + data[i]["ItemID"] + " " + "Type " + data[i]["Type"] + " " + "State " + data[i]["State"] + " " + "Name " + data[i]["Name"] + " " +  "resourcesPath " + data[i]["resourcesPath"] );
            info_Costume temp = new info_Costume();

            temp.ItemID = System.Convert.ToInt32(itemCsvDic[i]["ItemID"]);
            temp.Type = System.Convert.ToInt32(itemCsvDic[i]["Type"]);
            temp.State = System.Convert.ToInt32(itemCsvDic[i]["State"]);
            temp.Sex = System.Convert.ToInt32(itemCsvDic[i]["Sex"]);
            temp.Name = System.Convert.ToString(itemCsvDic[i]["Name"]);
            temp.Description = System.Convert.ToString(itemCsvDic[i]["Description"]);
            temp.price = System.Convert.ToInt32(itemCsvDic[i]["price"]);
            temp.Path = System.Convert.ToInt32(itemCsvDic[i]["Path"]);
            temp.Suggestion = System.Convert.ToInt32(itemCsvDic[i]["Suggestion"]);
            //temp.Route = System.Convert.ToString(itemCsvDic[i]["Route"]);

            //구매된 아이템 체크
            for (int j = 0; j < BuyItemId.Count; j++)
            {
                if(temp.ItemID == BuyItemId[j])
                {//구매 되었음을 표기
                    temp.State = 1;
                }
            }

            if(temp.Type < 6)
                CoustumList[temp.Type].Add(temp);
            else
                EctItemData.Add(temp);
        }
        #endregion


        Com.ins.BgmSoundPlay(Resources.Load<AudioClip>("BGM/Progress"));
    }

    public info_Costume getItemData(int itemId)
    {
        for (int i = 0; i < CoustumList.Length; i++)
        {
            for(int j = 0; j < CoustumList[i].Count; j++)
            {
                if (CoustumList[i][j].ItemID == itemId)
                    return CoustumList[i][j];
            }
        }
        for(int i = 0; i < EctItemData.Count; i++)
        {
            if (EctItemData[i].ItemID == itemId)
                return EctItemData[i];
        }

        return null;
    }

    public void RoomOutButtonSetting()
    {
        UiButtonController ubc = GameObject.FindObjectOfType<UiButtonController>();
        OutRoomButton.Clear();

        ButtonClass item1 = new ButtonClass();
        item1.text = "World Map";
        item1.addEvent = (() => {
            LoadingPage.LoadScene("World_A");
        });

        ButtonClass item2 = new ButtonClass();
        item2.text = "Quit Game";
        item2.addEvent = (() => {
            ubc.OnClick_Exit();
        });

        OutRoomButton.Add(item1);
        OutRoomButton.Add(item2);
        ubc.OnClick_OutRoomPopup(OutRoomButton);

        ubc.OR_Popup.Title.text = "leave the Room";
    }

    public void WorldMapOutButtonSetting()
    {
        UiButtonController ubc = GameObject.FindObjectOfType<UiButtonController>();
        OutRoomButton.Clear();

        ButtonClass item1 = new ButtonClass();
        item1.text = "My Room";
        item1.addEvent = (() => {
            LoadingPage.LoadScene("Room_A");
        });

        ButtonClass item2 = new ButtonClass();
        item2.text = "Char Setting Page";
        item2.addEvent = (() => {
            LoadingPage.LoadScene("01_CharacterWindow");
        });

        ButtonClass item3 = new ButtonClass();
        item3.text = "Quit Game";
        item3.addEvent = (() => {
            ubc.OnClick_Exit();
        });

        OutRoomButton.Add(item1);
        OutRoomButton.Add(item2);
        OutRoomButton.Add(item3);
        ubc.OnClick_OutRoomPopup(OutRoomButton);

        ubc.OR_Popup.Title.text = "leave the Room";
    }
}
