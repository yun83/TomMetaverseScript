using System.Collections.Generic;

[System.Serializable]
public class Info_Char
{
    public int Money = 2000;
    /// <summary>
    /// 닉네임
    /// </summary>
    public string NicName = "";
    /// <summary>
    /// 성별
    /// </summary>
    public int Sex = 1; //최초 기본 남자
    /// <summary>
    /// 머리카락
    /// </summary>
    public int Hair = 5; 
    /// <summary>
    /// 윗도리
    /// </summary>
    public int Shirt = 10;
    /// <summary>
    /// 바지
    /// </summary>
    public int Pants = 15;
    /// <summary>
    /// 신발
    /// </summary>
    public int Shoes = 19;
    /// <summary>
    /// 악세사리
    /// </summary>
    public int Accessory = -1;

    public void iniEqtData(string mNic = "")
    {
        //장착 아이템 초기화
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
            "\n Money [" + Money + "] "
            ;
        return ret;
    }
}

[System.Serializable]
public class info_Costume
{
    public int ItemID = 0;

    /// <summary>
    /// 0 - 머리
    /// 1 - 상의
    /// 2 - 하의
    /// 3 - 신발
    /// 4 - 원피스
    /// 5 - 악세사리
    /// </summary>
    public int Type = 0;

    /// <summary>
    /// 0 - 미보유
    /// 1 - 보유
    /// </summary>
    public int State = 0;

    /// <summary>
    /// 0 - 여자
    /// 1 - 남자
    /// 2 - 공용
    /// </summary>
    public int Sex = 0;
    
    /// <summary>
    /// 아이템 이름
    /// </summary>
    public string Name = "";

    /// <summary>
    /// 설명
    /// </summary>
    public string Description = "";

    /// <summary>
    /// 가격
    /// </summary>
    public int price = 0;

    /// <summary>
    /// 추후에는 경로로 일단은 자식객체 순서
    /// </summary>
    public int Path = 0;

    /// <summary>
    /// 추천 상품
    /// </summary>
    public int Suggestion = 0;

    /// <summary>
    /// 인게임에서 사용될 체크용 변수
    /// </summary>
    public int inGameUse = 0;
}

[System.Serializable] 
public class OptionData
{
    public bool EffectSound = true;
    public bool BgmSound = true;
    public bool NicNameOpen = true;
}


[System.Serializable] 
public class QuestData
{
    /// <summary>
    /// 퀘스트 아이디
    /// </summary>
    public int ID = -1;
    /// <summary>
    /// 퀘스트 종류
    /// </summary>
    public int QuestID = 0;
    /// <summary>
    /// 퀘스트 갯수
    /// </summary>
    public int State = 0;
    /// <summary>
    /// 퀘스트 이름 Id 0:일일퀘스트
    /// </summary>
    public int NameID = 0;
    public string nameText = "";
    /// <summary>
    /// 골드보상
    /// </summary>
    public int GoldReward = 0;
    /// <summary>
    /// 퀘스트 의견
    /// </summary>
    public string Description = "";

    /// <summary>
    /// 퀘스트 종류
    /// </summary>
    public List<int> QuiteListId = new List<int>();
    /// <summary>
    /// 종류에 따른 진행 사항
    /// </summary>
    public List<int> QuiteListState = new List<int>();
}