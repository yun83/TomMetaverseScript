using System;
using System.Collections.Generic;

[Serializable]
public class Info_Char
{
    public int Money = 2000;
    /// <summary>
    /// �г���
    /// </summary>
    public string NicName = "";
    /// <summary>
    /// ����
    /// </summary>
    public int Sex = 1; //���� �⺻ ����
    /// <summary>
    /// �Ӹ�ī��
    /// </summary>
    public int Hair = 5; 
    /// <summary>
    /// ������
    /// </summary>
    public int Shirt = 10;
    /// <summary>
    /// ����
    /// </summary>
    public int Pants = 15;
    /// <summary>
    /// �Ź�
    /// </summary>
    public int Shoes = 19;
    /// <summary>
    /// �Ǽ��縮
    /// </summary>
    public int Accessory = -1;
    /// <summary>
    /// �� ����
    /// </summary>
    public int PetID = -1;

    public void iniEqtData(string mNic = "")
    {
        //���� ������ �ʱ�ȭ
        Hair = 5;
        Shirt = 10;
        Pants = 15;
        Shoes = 19;
        Accessory = -1;
        NicName = mNic;
    }

    public string printData()
    {
        string ret =
            "\n NicName [" + NicName + "] " +
            "\n Sex [" + Sex + "] " +
            "\n Hair [" + Hair + "] " +
            "\n Shirt [" + Shirt + "] " +
            "\n Pants [" + Pants + "] " +
            "\n Shoes [" + Shoes + "] " +
            "\n Accessory [" + Accessory + "] " +
            "\n Money [" + Money + "] " +
            "\n PetID [" + PetID + "] "
            ;
        return ret;
    }
}

[Serializable]
public class CoustumItemCsv
{
    public int ItemID = 0;

    /// <summary>
    /// 0 - �Ӹ�
    /// 1 - ����
    /// 2 - ����
    /// 3 - �Ź�
    /// 4 - ���ǽ�
    /// 5 - �Ǽ��縮
    /// </summary>
    public int Type = 0;

    /// <summary>
    /// 0 - �̺���
    /// 1 - ����
    /// </summary>
    public int State = 0;

    /// <summary>
    /// 0 - ����
    /// 1 - ����
    /// 2 - ����
    /// </summary>
    public int Sex = 0;
    
    /// <summary>
    /// ������ �̸�
    /// </summary>
    public string Name = "";

    /// <summary>
    /// ����
    /// </summary>
    public string Description = "";

    /// <summary>
    /// ����
    /// </summary>
    public int price = 0;

    /// <summary>
    /// ���Ŀ��� ��η� �ϴ��� �ڽİ�ü ����
    /// </summary>
    public int Path = 0;

    /// <summary>
    /// ��õ ��ǰ
    /// </summary>
    public int Suggestion = 0;

    /// <summary>
    /// �ΰ��ӿ��� ���� üũ�� ����
    /// </summary>
    public int inGameUse = 0;
}

[Serializable] 
public class OptionData
{
    public bool EffectSound = true;
    public bool BgmSound = true;
    public bool NicNameOpen = true;
}

[Serializable]
public class ButtonClass
{
    public string text;
    public delegate void ClickEvent();
    public ClickEvent addEvent;
}

[Serializable] 
public class QuestCsv
{
    /// <summary>
    /// ����Ʈ ���̵�
    /// </summary>
    public int ID = -1;
    public int SceneID = 0;
    /// <summary>
    /// ����Ʈ ����
    /// </summary>
    public int State = 0;
    /// <summary>
    /// ����Ʈ �̸� Id 0:��������Ʈ
    /// </summary>
    public int NameID = 0;
    public string nameText = "";
    /// <summary>
    /// ��庸��
    /// </summary>
    public int GoldReward = 0;
    /// <summary>
    /// ����Ʈ �ǰ�
    /// </summary>
    public string Description = "";

    /// <summary>
    /// ����Ʈ ����
    /// </summary>
    public List<int> QuiteListId = new List<int>();
    /// <summary>
    /// ������ ���� ���� ����
    /// </summary>
    public List<int> QuiteListState = new List<int>();
}

[Serializable]
public class QuestDataCsv{
    public int ID = -1;
    public string Name = "";
    public string Description = "";
    public int Reward = 0;
    public int SceneId = 0;
    public int Priority = 0;
    public int State = 0;
}

[Serializable]
public class QuestVer2Data
{
    public int ID = -1;
    public string Name = "";
    public string Description = "";
    public int Priority = 0;
    public int Reward = 0;
    public int State = 0;
}

[Serializable] 
public class DayEventData{
    public int RouletteState;
    public string RouletteDay;

    public string printData()
    {
        string ret =
            "\n Roulette "+
            "\n State [" + RouletteState + "] " +
            "\n Day [" + RouletteDay + "] "
            ;
        return ret;
    }
}