using System;
using System.Collections.Generic;

[Serializable]
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
    /// <summary>
    /// 펫 종류
    /// </summary>
    public int PetID = -1;

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
    /// 퀘스트 아이디
    /// </summary>
    public int ID = -1;
    public int SceneID = 0;
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

[Serializable]
public class QuestDataCsv
{
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
    /// <summary>
    /// 퀘스트 아이디
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
    public int ID = -1;
    public string Name = "";
    public string Description = "";
    /// <summary>
    /// 진행 순위 낮을수록 먼저 실행
    /// </summary>
    public int Priority = 0;
    /// <summary>
    /// 보상
    /// </summary>
    public int Reward = 0;
    /// <summary>
    /// 0 - 기본상태
    /// 1 - 진행 가능 상태
    /// 2 - 다음 퀘스트로 진행 판단
    /// 3 - 진행 완료 상태
    /// 4 - 보상 완료 상태
    /// </summary>
    public int State = 0;

    public string PrintQuest()
    {
        string ret = "";
        ret = "ID [ " + ID + " ] :: " + Name + "\n" + Description + "\n 보상 : " + Reward + "\n 진행 상태 : " + State;
        return ret;
    }
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