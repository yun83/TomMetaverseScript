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

    //���� ��ġ�ϰ� �ִ� ���� ���� ������Ʈ
    /// <summary>
    /// 0:�ɸ��� ����
    /// 1:���̷�
    /// 2:�����
    /// </summary>
    public int State = 0;
    public bool MoneyChange = false;

    //���̺� ����
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

    //�ɸ��� ���� â���� ���� �κ� Ÿ�԰� ���� �� ������ ��ȣ �����
    public int ItemSelectIndex = -1;
    public int InvenNumber = -1;

    //���忡�� �̵��ÿ� Ű �ߺ� ������ ���ؼ�
    public int TouchId = -1;
    public int RightTId = -1;

    public Animator MyPlayerAnimator;
    public Animator SubCharAnimator;

    //������ ���� �ɸ��� ����
    public Info_Char CharacterMain = new Info_Char();

    //�ɸ� ���� â���� ���� �ɸ��� ����
    public Info_Char CharacterSub = new Info_Char();
    public bool CharacterChangeCheck = false;

    public List<ButtonClass> OutRoomButton = new List<ButtonClass>();

    //������ ����Ʈ�� ������ �ε�
    public List<CoustumItemCsv>[] CoustumList = new List<CoustumItemCsv>[6];
    public List<CoustumItemCsv> EctItemData = new List<CoustumItemCsv>();

    //��ũ�ѿ� ��ϵ� ������ ����Ʈ ����
    public List<CoustumItemCsv> CostumeScrollList = new List<CoustumItemCsv>();

    public List<int> BuyItemId = new List<int>(); //������ ������ ID

    //�𿡼� ������ ������ ����Ʈ �ۼ�
    public List<CoustumItemCsv> BuyItemSaveList = new List<CoustumItemCsv>();
    public CoustumItemCsv BuyItemSelect = new CoustumItemCsv();
    public bool TotlaMoneySumCheck = false;

    //����Ʈ ������ �ε�
    public List<QuestCsv> QuestList = new List<QuestCsv>();
    public QuestCsv dailyQuest = new QuestCsv();

    [Header("����Ʈ �ý���")]
    public List<QuestDataCsv> QuestData = new List<QuestDataCsv>();
    /// <summary>
    /// ���� �������� ����Ʈ ���̵�
    /// 0 ���̷�, 1 �����, 2 ����, 3 ����, 4 ��, 5 ������,
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
        Debug.Log(" ------------- ���̺� �ε� --------------");
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

        //���� ������ Ÿ���� 6����
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

                //���ŵ� ������ üũ
                for (int j = 0; j < BuyItemId.Count; j++)
                {
                    if (temp.ItemID == BuyItemId[j])
                    {//���� �Ǿ����� ǥ��
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

        //�ߺ� ����Ʈ ����
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
                        temp.nameText = "���� ����Ʈ";
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
        //����Ʈ �Ϸῡ ���� �Ǵ�
        if (Quest_WinState <= 0)
        {
            //���������� �Ϸ� ���Ѿ� �Ѵ�
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
                //Debug.Log(idx + " : ����Ʈ ��� : " + DataInfo.ins.dailyQuest.QuiteListState[i]);
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
