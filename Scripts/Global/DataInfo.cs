using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInfo : Single<DataInfo>
{
    public ControllerManager infoController;
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
    public bool MoneyChange = false;

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
    public List<CoustumItemCsv>[] CoustumList = new List<CoustumItemCsv>[6];
    public List<CoustumItemCsv> EctItemData = new List<CoustumItemCsv>();

    //스크롤에 등록될 아이템 리스트 정리
    public List<CoustumItemCsv> CostumeScrollList = new List<CoustumItemCsv>();

    public List<int> BuyItemId = new List<int>(); //구매한 아이템 ID

    //삽에서 구입할 아이템 리스트 작성
    public List<CoustumItemCsv> BuyItemSaveList = new List<CoustumItemCsv>();
    public CoustumItemCsv BuyItemSelect = new CoustumItemCsv();
    public bool TotlaMoneySumCheck = false;

    //퀘스트 데이터 로딩
    public List<QuestCsv> QuestList = new List<QuestCsv>();
    public QuestCsv dailyQuest = new QuestCsv();

    [Header("퀘스트 시스템")]
    public List<QuestDataCsv> QuestData = new List<QuestDataCsv>();
    /// <summary>
    /// 현재 진행중이 퀘스트 아이디
    /// 0 마이룸, 1 월드맵, 2 의자, 3 선물, 4 펫, 5 제스쳐,
    /// </summary>
    public int Now_QID = -1;
    public int Quest_WinState = 0;

    public int cWin_OpenBuyPopup = 0;
    private void Awake()
    {
        Application.targetFrameRate = 1000;
        Quest_WinState = 0;

        SaveDataLoding(); 
        ItemDataLoding();
        QuestDataLoding();

        Com.ins.BgmSoundPlay(Resources.Load<AudioClip>("BGM/Progress"));
    }

    private void SaveDataLoding()
    {
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
    }

    private void ItemDataLoding()
    {

        List<Dictionary<string, object>> itemCsvDic = CSVReader.Read("Doc/CostumItem");

        //현재 아이템 타입이 6가지
        for (int i = 0; i < CoustumList.Length; i++)
        {
            CoustumList[i] = new List<CoustumItemCsv>();
            CoustumList[i].Clear();
        }
        EctItemData.Clear();

        if (itemCsvDic != null)
        {
            for (var i = 0; i < itemCsvDic.Count; i++)
            {
                //print("ItemID " + data[i]["ItemID"] + " " + "Type " + data[i]["Type"] + " " + "State " + data[i]["State"] + " " + "Name " + data[i]["Name"] + " " +  "resourcesPath " + data[i]["resourcesPath"] );
                CoustumItemCsv temp = new CoustumItemCsv();

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
                    if (temp.ItemID == BuyItemId[j])
                    {//구매 되었음을 표기
                        temp.State = 1;
                    }
                }

                if (temp.Type < 6)
                    CoustumList[temp.Type].Add(temp);
                else
                    EctItemData.Add(temp);
            }
        }
    }

    private void QuestDataLoding()
    {
        List<Dictionary<string, object>> QuestGet = CSVReader.Read("Doc/Quest");
        List<Dictionary<string, object>> QuestDataGet = CSVReader.Read("Doc/QuestData");

        List<int> dailyCheck = new List<int>();

        QuestList.Clear();
        dailyCheck.Clear();
        QuestData.Clear();

        //중복 퀘스트 방지
        int[] indData = new int[QuestDataGet.Count];
        for (int i = 0; i < indData.Length; i++)
        {
            indData[i] = i;
        }


        if (QuestGet != null)
        {
            for (int i = 0; i < QuestGet.Count; i++)
            {
                QuestCsv temp = new QuestCsv();
                temp.ID = System.Convert.ToInt32(QuestGet[i]["ID"]); ;
                temp.QuestID = System.Convert.ToInt32(QuestGet[i]["QuestID"]);
                temp.State = System.Convert.ToInt32(QuestGet[i]["State"]);
                temp.NameID = System.Convert.ToInt32(QuestGet[i]["Name"]);

                switch (temp.NameID)
                {
                    case 0:
                        temp.nameText = "일일 퀘스트";
                        dailyCheck.Add(temp.ID);
                        break;
                }

                temp.GoldReward = System.Convert.ToInt32(QuestGet[i]["GoldReward"]);

                temp.Description = System.Convert.ToString(QuestGet[i]["Description"]);

                temp.QuiteListId.Clear();
                temp.QuiteListState.Clear();

                Com.ins.ShuffleArray(indData);

                for (int j = 0; j < temp.State; j++)
                {
                    temp.QuiteListId.Add(indData[j]);
                    temp.QuiteListState.Add(0);
                }

                QuestList.Add(temp);
            }
        }

        if (QuestDataGet.Count > 0)
        {
            for (int i = 0; i < QuestDataGet.Count; i++)
            {
                QuestDataCsv temp = new QuestDataCsv();

                temp.ID = System.Convert.ToInt32(QuestDataGet[i]["ID"]); ;
                temp.Name = System.Convert.ToString(QuestDataGet[i]["Name"]);
                temp.Description = System.Convert.ToString(QuestDataGet[i]["Description"]);
                temp.State = 0;

                QuestData.Add(temp);
            }
        }

        dailyQuest = QuestList[dailyCheck[Random.Range(0, dailyCheck.Count)]];
    }

    public CoustumItemCsv getItemData(int itemId)
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

    // Update is called once per frame
    void LateUpdate()
    {
        QuestCheckFunction();
    }

    void QuestCheckFunction()
    {
        //퀘스트 완료에 대한 판단
        if (Quest_WinState <= 0)
        {
            //순차적으로 완료 시켜야 한다
            int QSize = dailyQuest.QuiteListState.Count;
            int WinCheckCount = 0;

            Now_QID = -1;
            for (int i = 0; i < QSize; i++)
            {
                if (dailyQuest.QuiteListState[i] == 0)
                {
                    Now_QID = dailyQuest.QuiteListId[i];
                    break;
                }
            }

            for (int i = 0; i < QSize; i++)
            {
                int idx = dailyQuest.QuiteListId[i];
                dailyQuest.QuiteListState[i] = QuestData[idx].State;
                if(dailyQuest.QuiteListState[i] > 0)
                    WinCheckCount++;
                //Debug.Log(idx + " : 퀘스트 결과 : " + DataInfo.ins.dailyQuest.QuiteListState[i]);
            }

            if (WinCheckCount >= QSize)
            {
                Quest_WinState = 1;
            }
        }
    }
    public void AddMoney(int value)
    {
        CharacterMain.Money += value;
        SaveData = JsonUtility.ToJson(CharacterMain);
        MoneyChange = true;
    }

    public void RoomOutButtonSetting()
    {
        UiButtonController ubc = GameObject.FindObjectOfType<UiButtonController>();
        OutRoomButton.Clear();

        ButtonClass item1 = new ButtonClass();
        item1.text = "World Map";
        item1.addEvent = (() => {
            infoController.LoadScene("World_A");
        });

        //ButtonClass item2 = new ButtonClass();
        //item2.text = "Char Setting Page";
        //item2.addEvent = (() => {
        //    LoadingPage.LoadScene("01_CharacterWindow");
        //});

        ButtonClass item3 = new ButtonClass();
        item3.text = "Quit Game";
        item3.addEvent = (() => {
            ubc.OnClick_Exit();
        });

        OutRoomButton.Add(item1);
        //OutRoomButton.Add(item2);
        OutRoomButton.Add(item3);
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
            infoController.LoadScene("Room_A");
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
}
