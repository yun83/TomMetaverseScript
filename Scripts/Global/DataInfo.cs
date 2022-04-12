using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInfo : Single<DataInfo>
{
    public ControllerManager infoController;
    public UiButtonController GameUI;
    public PetMoveController PetController;
    public CharacterManager myPlayer;
    public Animator myAnimator;

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
    public List<QuestVer2Data>[] QVer2 = new List<QuestVer2Data>[4];
    /// <summary>
    /// ���� �������� ����Ʈ ���̵�
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
    public int Now_QID = -1;
    public int Quest_WinState = 0;

    public int cWin_OpenBuyPopup = 0;

    public bool CallNpc = false;

    [Header("�ð��� �̺�Ʈ")]
    public DayEventData deData = new DayEventData();

    private void Awake()
    {
        Application.targetFrameRate = 1000;
        Quest_WinState = 0;

        SaveDataLoding(); 
        ItemDataLoding();
        QuestDataLoding();
        TimeEventLoding();

        Com.ins.BgmSoundPlay(Resources.Load<AudioClip>("BGM/Progress"));
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
            //QVer2[i].Sort();
            SortPriority(QVer2[i]);
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
            //for (int i = 0; i < QSize; i++)
            //{
            //    if (dailyQuest.QuiteListState[i] == 0)
            //    {
            //        Now_QID = dailyQuest.QuiteListId[i];
            //        break;
            //    }
            //}

            //for (int i = 0; i < QSize; i++)
            //{
            //    int idx = dailyQuest.QuiteListId[i];
            //    dailyQuest.QuiteListState[i] = QuestData[idx].State;
            //    if(dailyQuest.QuiteListState[i] > 0)
            //        WinCheckCount++;
            //    //Debug.Log(idx + " : ����Ʈ ��� : " + DataInfo.ins.dailyQuest.QuiteListState[i]);
            //}

            //if (WinCheckCount >= QSize)
            //{
            //    Quest_WinState = 1;
            //}
        }
    }

    public void AddMoney(int value)
    {
        CharacterMain.Money += value;
        SaveData = JsonUtility.ToJson(CharacterMain);
        MoneyChange = true;
    }

}
