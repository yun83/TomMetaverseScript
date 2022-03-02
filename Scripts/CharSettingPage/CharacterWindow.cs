using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterWindow : MonoBehaviour
{
    public GameObject InvenScrollView;
    public Transform ItemOpenTrans;
    public Button[] ItemIconButton;
    public Button[] SexSelectButton;
    InitScroll setScroll;

    public Transform mainCam;
    public Transform mPlayer;

    public GameObject NicNameSetter;
    private InputField ChangedNicName;
    public Text NicNameObj;
    public GameObject SetterEndObj;
    private Button SetterEndButton;
    public Button CloseButton;

    private Vector2 nowPos, prePos;
    private Vector2 movePosDiff;

    public NicNamePopup NicPopup;
    public GameObject ExitPopupObj;
    public GameObject QuitPopupObj;

    public string NectSceneName = "97_moveScene";
    /// <summary>
    /// 0 - 팝업 없음
    /// 1 - 저장 및 나가기 팝업
    /// 2 - 닉네임 잘못 됨을 알리는 팝업
    /// </summary>
    private int PopupOpenCheck = 0;

    private CharacterManager playerManger = new CharacterManager();

    private int CheckInvenNumber = -1;
    private int CheckInvenIndex = -1;

    void Start()
    {
        playerManger = mPlayer.GetComponent<CharacterManager>();

        for (int i = 0; i < ItemIconButton.Length; i++)
        {
            int tempIdx = i;
            ItemIconButton[i].onClick.RemoveAllListeners();
            ItemIconButton[tempIdx].onClick.AddListener(()=> {
                OnClick_IndexButton(tempIdx);
            });
        }

        for(int i = 0; i < SexSelectButton.Length; i++)
        {
            SexSelectButton[i].onClick.RemoveAllListeners();
        }
        SexSelectButton[0].onClick.AddListener(OnClick_SexMale);
        SexSelectButton[1].onClick.AddListener(OnClick_SexFemale);

        if (!DataInfo.ins.SaveData.Equals("") && DataInfo.ins.SaveData != null)
        {
            Debug.Log(" ------------- CharacterWindow 로딩 ------------- ");
            DataInfo.ins.CharacterSub = JsonUtility.FromJson<Info_Char>(DataInfo.ins.SaveData);
        }

        if (DataInfo.ins.CharacterSub.NicName == null || DataInfo.ins.CharacterSub.NicName == "")
        {
            NicNameObj.text = "닉네임을 설정하세요";
            NicNameSetter.SetActive(true);
            SetterEndObj.SetActive(false);
        }
        else
        {
            NicNameObj.text = DataInfo.ins.CharacterSub.NicName;
            NicNameSetter.SetActive(false);
            SetterEndObj.SetActive(true);
        }

        ChangedNicName = NicNameSetter.GetComponentInChildren<InputField>();

        SetterEndButton = SetterEndObj.GetComponent<Button>();
        SetterEndButton.onClick.RemoveAllListeners();
        SetterEndButton.onClick.AddListener(OpenPopup);

        CloseButton.onClick.RemoveAllListeners();
        CloseButton.onClick.AddListener(OpenQuitPopupObj);

        ExitPopupObj.SetActive(false);
        QuitPopupObj.SetActive(false);

        playerManger.itemEquipment(DataInfo.ins.CharacterSub);

        StopAllCoroutines();

        if (DataInfo.ins.CharacterSub.Sex == 1)
            OnClick_SexMale();
        else
            OnClick_SexFemale();

        OnClick_IndexButton(1);
    }

    void FixedUpdate()
    {
        //CharacterRotation();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //팝업 닫는 부분에서 사운드가 중복으로 나는걸 방지 하기 위해서 분기 해주었다
            switch (PopupOpenCheck)
            {
                default:
                    OpenPopup();
                    break;
                case 1:
                    ClosePopup();
                    break;
                case 2:
                    closeCharPopup();
                    break;
                case 3:
                    OnClick_ClseQuitPopupObj();
                    break;
            }
        }
        ChangeCostumeCheck();
    }

    void ChangeCostumeCheck()
    {
        bool tempCheck = false;
        if (CheckInvenNumber != DataInfo.ins.InvenNumber)
        {
            CheckInvenNumber = DataInfo.ins.InvenNumber;
            tempCheck = true;
        }
        if (CheckInvenIndex != DataInfo.ins.ItemSelectIndex)
        {
            CheckInvenIndex = DataInfo.ins.ItemSelectIndex;
            tempCheck = true;
        }

        if (tempCheck)
        {
            //playerManger.SetCharInfo(CheckInvenNumber, CheckInvenIndex);
            playerManger.itemEquipment(DataInfo.ins.CharacterSub);
        }
    }

    public void Evnet_PointDown()
    {
        prePos = Input.mousePosition;
    }

    public void Event_PointUp()
    {
    }

    public void Event_Drag()
    {
        movePosDiff = Vector3.zero;

        nowPos = Input.mousePosition;
        movePosDiff = (Vector2)((prePos - nowPos) * Time.deltaTime);

        if (movePosDiff.x < 0)
        {
            mPlayer.Rotate(0, movePosDiff.x, 0);
        }
        else if (movePosDiff.x > 0)
        {
            mPlayer.Rotate(0, movePosDiff.x, 0);
        }
    }

    private void CharacterRotation()
    {
        movePosDiff = Vector3.zero;

        if (Input.GetMouseButtonDown(0))
        {
            prePos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
        }
        else if (Input.GetMouseButton(0))
        {
            nowPos = Input.mousePosition;
            movePosDiff = (Vector2)((prePos - nowPos) * Time.deltaTime);

            if (movePosDiff.x < 0)
            {
                mPlayer.Rotate(0, movePosDiff.x, 0);
            }
            else if (movePosDiff.x > 0)
            {
                mPlayer.Rotate(0, movePosDiff.x, 0);
            }
        }
    }

    public void OnClick_IndexButton(int idx)
    {
        string PrintStr = "";
        PrintStr = "click Event [" + idx + "] : ";
        switch (idx)
        {
            case 0: PrintStr += "모자&머리 인벤"; break;
            case 1: PrintStr += "셔츠 인벤"; break;
            case 2: PrintStr += "바지 인벤"; break;
            case 3: PrintStr += "신발 인벤"; break;
            case 4: PrintStr += "세트 인벤"; break;
            case 5: PrintStr += "악세서리 인벤"; break;
        }

        if( DataInfo.ins.InvenNumber >= 0)
        {
            Image OldButImage = ItemIconButton[DataInfo.ins.InvenNumber].GetComponent<Image>();
            OldButImage.color = Color.white;
        }

        StartCoroutine(ClickEvnetCall(idx));

        Image tempImage = ItemIconButton[idx].GetComponent<Image>();
        tempImage.color = Color.blue;

        DataInfo.ins.ItemSelectIndex = -1;

        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));

        //Debug.Log(PrintStr);
    }

    IEnumerator ClickEvnetCall(int idx)
    {
        ReSetScrollView();

        yield return null;

        DataInfo.ins.InvenNumber = idx;

        if (setScroll == null)
            setScroll = InvenScrollView.GetComponent<InitScroll>();

        DataInfo.ins.CostumeScrollList.Clear();

        for (int i = 0; i < DataInfo.ins.CoustumList[idx].Count; i++) {
            //성별이 공통이거나 같을때에 리스트에 추가
            if (DataInfo.ins.CoustumList[idx][i].Sex == DataInfo.ins.CharacterSub.Sex || DataInfo.ins.CoustumList[idx][i].Sex == 2)
            {
                DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[idx][i]);
            }
        }

        yield return new WaitForEndOfFrame();
        //yield return new WaitForSeconds(1.0f);
        setScroll.totalCount = DataInfo.ins.CostumeScrollList.Count;

        setScroll.InitScrollCall();
    }

    public void OnClick_InvenOff()
    {
        OpenPopup();
    }

    void ReSetScrollView()
    {
        RectTransform ContentRect = InvenScrollView.GetComponent<LoopVerticalScrollRect>().content;
        int Size = ContentRect.childCount;

        if (Size <= 0)
            return;

        for(int i = Size - 1; i >= 0; i--)
        {
            Destroy(ContentRect.GetChild(i).gameObject);
        }

    }

    public void OnChangedText_NicName()
    {
        string tempName = ChangedNicName.text;
        NicNameObj.text = tempName;
    }

    public void OnClick_NicNameSet()
    {
        //Debug.Log("닉네임 변경");
        NicNameSetter.SetActive(true);
        SetterEndObj.SetActive(false);
    }

    public void OnClick_SaveButton()
    {
        DataInfo.ins.CharacterSub.printData();

        if(ChangedNicName.text == null || ChangedNicName.text.Equals(""))
        {
            PopupOpenCheck = 2;
            return;
        }

        //Debug.Log("닉네임 저장 버튼");
        string tempName = ChangedNicName.text;
        DataInfo.ins.CharacterSub.NicName = tempName;
        NicNameObj.text = DataInfo.ins.CharacterSub.NicName;

        NicNameSetter.SetActive(false);
        SetterEndObj.SetActive(true);

        //info 동기화
        //DataInfo.ins.CharacterMain.NicName = DataInfo.ins.CharacterSub.NicName;
    }

    public void OnClick_ItemRestButton() {
        //Debug.Log("착용 아이템 초기화");
        
        if(DataInfo.ins.SaveData != "" && DataInfo.ins.SaveData != null)
            DataInfo.ins.CharacterSub = JsonUtility.FromJson<Info_Char>(DataInfo.ins.SaveData);
        else
            DataInfo.ins.CharacterSub.iniEqtData();

        if (DataInfo.ins.CharacterSub.Sex == 1)
            OnClick_SexMale();
        else
            OnClick_SexFemale();

        //의상 목록 초기화
        playerManger.itemEquipment(DataInfo.ins.CharacterSub);
    }

    public void OnClick_SexMale()
    {
        //Debug.Log("남자 버튼");
        Image t1 = SexSelectButton[0].GetComponent<Image>();
        Image t2 = SexSelectButton[1].GetComponent<Image>();

        t1.color = Color.yellow;
        t2.color = Color.white;

        if (DataInfo.ins.CharacterSub.Sex == 0)
        {
            DataInfo.ins.CharacterSub.Sex = 1;
            DataInfo.ins.CharacterSub.Hair = 5;

            playerManger.itemEquipment(DataInfo.ins.CharacterSub);
            OnClick_IndexButton(DataInfo.ins.InvenNumber);
        }
    }

    public void OnClick_SexFemale()
    {
        //Debug.Log("여자 버튼");
        Image t1 = SexSelectButton[0].GetComponent<Image>();
        Image t2 = SexSelectButton[1].GetComponent<Image>();

        t1.color = Color.white;
        t2.color = Color.yellow;

        if (DataInfo.ins.CharacterSub.Sex == 1)
        {
            DataInfo.ins.CharacterSub.Sex = 0;
            DataInfo.ins.CharacterSub.Hair = 0;

            playerManger.itemEquipment(DataInfo.ins.CharacterSub);
            OnClick_IndexButton(DataInfo.ins.InvenNumber);
        }
    }

    void OpenPopup()
    {
        PopupOpenCheck = 1;
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Pop Up"));
        ExitPopupObj.SetActive(true);
    }

    public void ClosePopup()
    {
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
        PopupOpenCheck = 0;
        ExitPopupObj.SetActive(false);
    }

    void OpenCharPopupData(string mTitle = "알림", string mDescription = "팝업 텍스트")
    {
        NicPopup.gameObject.SetActive(true);

        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Pop Up"));
        PopupOpenCheck = 2;
        NicPopup.Title.text = mTitle;
        NicPopup.Description.text = mDescription;
        NicPopup.OK_Button.onClick.RemoveAllListeners();
        NicPopup.OK_Button.onClick.AddListener(closeCharPopup);
    }

    void closeCharPopup()
    {
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
        PopupOpenCheck = 0;
        NicPopup.gameObject.SetActive(false);
    }

    void OpenQuitPopupObj()
    {
        QuitPopupObj.SetActive(true);
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Pop Up"));
        PopupOpenCheck = 3;
    }

    public void OnClick_ClseQuitPopupObj()
    {
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
        PopupOpenCheck = 0;
        QuitPopupObj.SetActive(false);
    }

    public void OnClick_QuitPopupOk()
    {
        Application.Quit();
    }

    public void SaveExitCharSettingPage() {
        if(DataInfo.ins.CharacterSub.NicName == null || DataInfo.ins.CharacterSub.NicName == "")
        {
            Debug.Log("케릭명 설정이 안되었음을 알리는 팝업");
            NicPopup.gameObject.SetActive(false);
            OpenCharPopupData("알림", "케릭명을 설정해 주세요");
            return;
        }
        
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Select"));

        DataInfo.ins.SaveData = JsonUtility.ToJson(DataInfo.ins.CharacterSub);

        Debug.Log(DataInfo.ins.SaveData);
        DataInfo.ins.CharacterMain = DataInfo.ins.CharacterSub;

        //UnityEngine.SceneManagement.SceneManager.LoadScene(NectSceneName);
        LoadingPage.LoadScene(NectSceneName);
    }
}
