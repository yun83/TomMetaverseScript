using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInfo : Single<DataInfo>
{
    public ControllerManager infoController;
    public UiButtonController GameUI;
    public PetMoveController PetController;
    public CharacterManager myPlayer;

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
    public int State = -1;
    public int OldState = -1;
    public string OldScneName = "";
    public bool MoneyChange = false;

    bool DataLodingCheck = false;

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
    public string DayEvent
    {
        get { return PlayerPrefs.GetString("DayEvent", ""); }
        set { PlayerPrefs.SetString("DayEvent", value); }
    }
    public string MyRoomName
    {
        get { return PlayerPrefs.GetString("MyRoomName", ""); }
        set { PlayerPrefs.SetString("MyRoomName", value); }
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

    [Header("퀘스트 시스템")]
    //퀘스트 데이터 로딩
    public List<QuestCsv> QuestList = new List<QuestCsv>();
    public QuestCsv dailyQuest = new QuestCsv();
    public List<QuestDataCsv> QuestData = new List<QuestDataCsv>();

    public int QuestIdx = 0;
    public bool QuestWinCheck = false;
    /// <summary>
    /// 0 일일퀘스트
    /// 1 룸 퀘스트
    /// 2 월드 퀘스트
    /// 3 카페 퀘스트
    /// </summary>
    public List<QuestVer2Data>[] QVer2 = new List<QuestVer2Data>[4];

    public int cWin_OpenBuyPopup = 0;

    public bool CallNpc = false;

    [Header("시간형 이벤트")]
    public DayEventData deData = new DayEventData();

    private void Awake()
    {
        Application.targetFrameRate = 1000;

        DataLodingCheck = false;

        SaveDataLoding();
        SceneNameCheck();
        ItemDataLoding();
        QuestDataLoding();
        TimeEventLoding();

    }

    void SceneNameCheck()
    {
        DataLodingCheck = true;
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            default:
                State = -1;
                break;
            case "Room_A":
            case "Room_B":
                State = 1;
                break;
            case "World_A":
                State = 2;
                break;
            case "CoffeeShop":
                State = 3;
                break;
        }
        Com.ins.BgmSoundPlay(Resources.Load<AudioClip>("BGM/Progress"));
    }

    // Update is called once per frame
    void LateUpdate()
    {
        QuestCheckFunction();
    }

    private void SaveDataLoding()
    {
        //Debug.Log(" ------------- 세이브 로딩 --------------");
        if (!SaveData.Equals("") && SaveData != null)
        {
            CharacterMain = JsonUtility.FromJson<Info_Char>(SaveData);
        }
        Debug.Log("<color=yellow>Print Character Data</color>" + CharacterMain.printData());

        if (!saveOption.Equals("") && saveOption != null)
        {
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

        QuestList.Clear();
        QuestData.Clear();

        if (QuestDataGet.Count > 0)
        {
            for (int i = 0; i < QuestDataGet.Count; i++)
            {
                QuestDataCsv temp = new QuestDataCsv();

                temp.ID = System.Convert.ToInt32(QuestDataGet[i]["ID"]);
                temp.Name = System.Convert.ToString(QuestDataGet[i]["Name"]);
                temp.Description = System.Convert.ToString(QuestDataGet[i]["Description"]);

                temp.Reward = System.Convert.ToInt32(QuestDataGet[i]["Reward"]);
                temp.SceneId = System.Convert.ToInt32(QuestDataGet[i]["SceneID"]);
                temp.Priority = System.Convert.ToInt32(QuestDataGet[i]["Priority"]);

                temp.State = 0;

                QuestData.Add(temp);
            }
        }

        List<int> QuestIdList = new List<int>();
        QuestIdList.Clear();

        if (QuestGet != null)
        {
            for (int i = 0; i < QuestGet.Count; i++)
            {
                QuestCsv temp = new QuestCsv();
                temp.ID = System.Convert.ToInt32(QuestGet[i]["ID"]); ;
                temp.SceneID = System.Convert.ToInt32(QuestGet[i]["SceneID"]);
                temp.State = System.Convert.ToInt32(QuestGet[i]["State"]);
                temp.nameText = System.Convert.ToString(QuestGet[i]["Name"]);
                temp.GoldReward = System.Convert.ToInt32(QuestGet[i]["GoldReward"]);
                temp.Description = System.Convert.ToString(QuestGet[i]["Description"]);

                temp.QuiteListId.Clear();
                temp.QuiteListState.Clear();
                for (int cnt = 0; cnt < QuestData.Count; cnt++)
                {
                    if (temp.SceneID == QuestData[cnt].SceneId)
                    {
                        temp.QuiteListId.Add(QuestData[cnt].ID);
                        temp.QuiteListState.Add(0);
                    }
                }
                QuestList.Add(temp);
            }
        }

        for (int i = 0; i < QVer2.Length; i++)
        {
            QVer2[i] = new List<QuestVer2Data>();
            QVer2[i].Clear();
        }

        for (int i = 0; i < QVer2.Length; i++)
        {
            for (int idx = 0; idx < QuestData.Count; idx++)
            {
                if (QuestData[idx].SceneId == i)
                {
                    QuestVer2Data tem = new QuestVer2Data();
                    tem.ID = QuestData[idx].ID;
                    tem.Name = QuestData[idx].Name;
                    tem.Description = QuestData[idx].Description;
                    tem.Reward = QuestData[idx].Reward;
                    tem.State = 0;
                    tem.Priority = QuestData[idx].Priority;

                    QVer2[i].Add(tem);
                }
            }
        }

        for (int i = 0; i < QVer2.Length; i++)
        {
            SortPriority(QVer2[i]);
        }

        ////맨처음 퀘스트들의 시작 가능상태로 변경
        //for (int i = 0; i < QVer2.Length; i++)
        //{
        //    QVer2[i][0].State = 1;
        //}
        for (int i = 0; i < QVer2.Length; i++)
        {
            for (int idx = 0; idx < QVer2[i].Count; idx++)
                QVer2[i][idx].State = 1;
        }
    }

    public void SortPriority(List<QuestVer2Data> data) //오름 차순으로 정렬하는 함수
    {
        data.Sort(delegate (QuestVer2Data A, QuestVer2Data B)
        {
            if (A.Priority > B.Priority) return 1;
            else if (A.Priority < B.Priority) return -1;
            return 0; //동일한 값일 경우
        });
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

    void TimeEventLoding()
    {
        //Debug.Log(" ------------- 세이브 로딩 --------------");
        if (!DayEvent.Equals("") && DayEvent != null)
        {
            deData = JsonUtility.FromJson<DayEventData>(DayEvent);
            //DateTime a = System.DateTime.ParseExact(deData.RouletteDay, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
        }
        Debug.Log("<color=yellow>Time Event Print</color>" + deData.printData());
    }

    void QuestCheckFunction()
    {
        if (!DataLodingCheck)
            return;

        bool ShowQuestWin = false;

        //퀘스트 체크
        for (int i = 0; i < QVer2.Length; i++)
        {
            if (QVer2[i] == null)
                return;
            for (int idx = 0; idx < QVer2[i].Count; idx++)
            {
                switch (QVer2[i][idx].State)
                {
                    case 0:
                        break;
                    case 1:
                        //진행 중
                        break;
                    case 2:
                        {
                            //완료 판단
                            int NextIdx = idx + 1;
                            if (NextIdx < QVer2[i].Count)
                            {//다음 단계 활성화
                                if(QVer2[i][NextIdx].State == 0)
                                    QVer2[i][NextIdx].State = 1;
                            }
                            QVer2[i][idx].State = 3;
                            //Debug.Log(QVer2[i][idx].PrintQuest());
                        }
                        break;
                    case 3:
                        //퀘스트 완료 알림 스테이트
                        ShowQuestWin = true;
                        break;
                    case 4:
                        {
                            QVer2[i][idx].State = 5;
                            QuestVer2Data temp = QVer2[i][idx];
                            QVer2[i].RemoveAt(idx);
                            QVer2[i].Add(temp);
                        }
                        break;
                }
            }
        }

        if (ShowQuestWin)
        {
            QuestWinCheck = ShowQuestWin;
            GameUI.QuestSuccess.SetActive(true);
        }

        if(QuestWinCheck != ShowQuestWin)
        {
            QuestWinCheck = ShowQuestWin;
            GameUI.QuestSuccess.SetActive(false);
        }
    }
    /// <summary>
    /// 0	마이룸에 돌아가세요
    /// 1	월드맵으로 나가세요
    /// 2	의자에 앉아보세요
    /// 3	선물상자를 획득하세요
    /// 4	펫과 교감 하세요
    /// 5	제스쳐를 취해보세요
    /// 6	카페에 방문하세요
    /// 7	침대에 누워 보세요
    /// 8	TV를 켜 보세요
    /// 9	커피를 주문해 보세요
    /// 10	펫을 분양 받으세요
    /// 11	룰렛을 돌려보세요
    /// </summary>
    public void WinQuest(int QuestId)
    {
        if (!DataLodingCheck)
            return;

        for (int i = 0; i < QVer2.Length; i++)
        {
            if (QVer2[i] == null)
                return;
            for (int idx = 0; idx < QVer2[i].Count; idx++)
            {
                if(QVer2[i][idx].ID == QuestId)
                {
                    if (QVer2[i][idx].State == 1)
                    {
                        //Debug.Log("퀘스트 ID : [" + QVer2[i][idx].ID + " : " + QuestId + "] Name [" + QVer2[i][idx].Name + "] 성공");
                        QVer2[i][idx].State = 2;
                    }
                }
            }
        }
    }

    public void AddMoney(int value)
    {
        CharacterMain.Money += value;
        SaveData = JsonUtility.ToJson(CharacterMain);
        MoneyChange = true;
    }
    public string GetThousandCommaText(int data)
    {
        return string.Format("{0:#,###}", data);
    }
}
