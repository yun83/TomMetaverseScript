using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInfo : Single<DataInfo>
{
    public bool LoginCheck = false;
    public bool LodingCheck = false;

    public float ScreenW = 1280;
    public float ScreenH = 720;

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
    public OptionData OptionInfo = new OptionData();

    //케릭터 선택 창에서 사용될 인벤 타입과 선택 된 아이템 번호 저장용
    public int ItemSelectIndex = -1;
    public int InvenNumber = -1;

    //월드에서 이동시에 키 중복 방지를 위해서
    public int TouchId = -1;
    public int RightTId = -1;

    public Animator MyPlayerAnimator;

    //저장할 메인 케릭터 정보
    public Info_Char CharacterMain = new Info_Char();

    //케릭 선택 창에서 사용될 케릭터 정보
    public Info_Char CharacterSub = new Info_Char();

    //아이템 리스트의 데이터 로딩
    public List<info_Costume>[] CoustumList = new List<info_Costume>[6];

    //스크롤에 등록될 아이템 리스트 정리
    public List<info_Costume> CostumeScrollList = new List<info_Costume>();

    //이모션
    public string[] TriggerName =
    {
        "Dance",
        "Angry",
        "Cry",
        "Happy",
        "Laugh",
        "Greet",
        "handshake",
        "Pant",
        "SayNo",
        "SayYes",
    };
}
