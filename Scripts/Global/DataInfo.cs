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

    //���� ��ġ�ϰ� �ִ� ���� ���� ������Ʈ
    /// <summary>
    /// 0:�ɸ��� ����
    /// 1:���̷�
    /// 2:�����
    /// </summary>
    public int State = -1;
    public int OldState = -1;
    public string OldScneName = "";
    public bool MoneyChange = false;

    bool DataLodingCheck = false;

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

    [Header("����Ʈ �ý���")]
    //����Ʈ ������ �ε�
    public List<QuestCsv> QuestList = new List<QuestCsv>();
    public QuestCsv dailyQuest = new QuestCsv();
    public List<QuestDataCsv> QuestData = new List<QuestDataCsv>();

    public int QuestIdx = 0;
    public bool QuestWinCheck = false;
    /// <summary>
    /// 0 ��������Ʈ
    /// 1 �� ����Ʈ
    /// 2 ���� ����Ʈ
    /// 3 ī�� ����Ʈ
    /// </summary>
    public List<QuestVer2Data>[] QVer2 = new List<QuestVer2Data>[4];

    public int cWin_OpenBuyPopup = 0;

    public bool CallNpc = false;

    [Header("�ð��� �̺�Ʈ")]
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
        //Debug.Log(" ------------- ���̺� �ε� --------------");
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

        ////��ó�� ����Ʈ���� ���� ���ɻ��·� ����
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

    public void SortPriority(List<QuestVer2Data> data) //���� �������� �����ϴ� �Լ�
    {
        data.Sort(delegate (QuestVer2Data A, QuestVer2Data B)
        {
            if (A.Priority > B.Priority) return 1;
            else if (A.Priority < B.Priority) return -1;
            return 0; //������ ���� ���
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
        //Debug.Log(" ------------- ���̺� �ε� --------------");
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

        //����Ʈ üũ
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
                        //���� ��
                        break;
                    case 2:
                        {
                            //�Ϸ� �Ǵ�
                            int NextIdx = idx + 1;
                            if (NextIdx < QVer2[i].Count)
                            {//���� �ܰ� Ȱ��ȭ
                                if(QVer2[i][NextIdx].State == 0)
                                    QVer2[i][NextIdx].State = 1;
                            }
                            QVer2[i][idx].State = 3;
                            //Debug.Log(QVer2[i][idx].PrintQuest());
                        }
                        break;
                    case 3:
                        //����Ʈ �Ϸ� �˸� ������Ʈ
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
    /// 0	���̷뿡 ���ư�����
    /// 1	��������� ��������
    /// 2	���ڿ� �ɾƺ�����
    /// 3	�������ڸ� ȹ���ϼ���
    /// 4	��� ���� �ϼ���
    /// 5	�����ĸ� ���غ�����
    /// 6	ī�信 �湮�ϼ���
    /// 7	ħ�뿡 ���� ������
    /// 8	TV�� �� ������
    /// 9	Ŀ�Ǹ� �ֹ��� ������
    /// 10	���� �о� ��������
    /// 11	�귿�� ����������
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
                        //Debug.Log("����Ʈ ID : [" + QVer2[i][idx].ID + " : " + QuestId + "] Name [" + QVer2[i][idx].Name + "] ����");
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
