using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInfo : Single<DataInfo>
{
    public bool LoginCheck = false;
    public bool LodingCheck = false;

    public float ScreenW = 1280;
    public float ScreenH = 720;

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
    public OptionData OptionInfo = new OptionData();

    //�ɸ��� ���� â���� ���� �κ� Ÿ�԰� ���� �� ������ ��ȣ �����
    public int ItemSelectIndex = -1;
    public int InvenNumber = -1;

    //���忡�� �̵��ÿ� Ű �ߺ� ������ ���ؼ�
    public int TouchId = -1;
    public int RightTId = -1;

    public Animator MyPlayerAnimator;

    //������ ���� �ɸ��� ����
    public Info_Char CharacterMain = new Info_Char();

    //�ɸ� ���� â���� ���� �ɸ��� ����
    public Info_Char CharacterSub = new Info_Char();

    //������ ����Ʈ�� ������ �ε�
    public List<info_Costume>[] CoustumList = new List<info_Costume>[6];

    //��ũ�ѿ� ��ϵ� ������ ����Ʈ ����
    public List<info_Costume> CostumeScrollList = new List<info_Costume>();

    //�̸��
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
