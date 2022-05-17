using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterWindow : MonoBehaviour
{
    public GameObject InvenScrollView;
    public Transform ItemOpenTrans;
    public Color SelectColor = new Color(0.6f, 1, 1, 1);
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

    private Vector2 nowPos, prePos;
    private Vector2 movePosDiff;


    public NicNamePopup NicPopup;
    [Header("옵션")]
    public Button OptionButton;
    public GameObject Popup_Option;

    [Header("설정완료")]
    public GameObject ExitPopupObj;
    public Text ExitPopupText;
    public Button ExitPopupYes;
    public Button ExitPopupNo;

    public string NectSceneName = "97_moveScene";
    /// <summary>
    /// 0 - 팝업 없음
    /// 1 - 저장 및 나가기 팝업
    /// 2 - 닉네임 잘못 됨을 알리는 팝업
    /// 3 - 게임 종료 팝업
    /// 4 - 아이템 구매 팝업
     /// </summary>
    private int PopupOpenCheck = 0;

    private CharacterManager playerManger = new CharacterManager();

    private int CheckInvenNumber = -1;
    private int CheckInvenIndex = -1;

    [Header("로딩화면")]
    public GameObject PageLodingPopup;
    public Image progressBar = null;
    public Text ToolTipText;
    public string nextScene = "";

    [Header("아이템 구매")]
    public ItemBuyPopup BuyPopup;
    public GameObject MyRoomSelectPopup;

    void Start()
    {
        playerManger = mPlayer.GetComponent<CharacterManager>();

        for (int i = 0; i < ItemIconButton.Length; i++)
        {
            int tempIdx = i;
            ItemIconButton[i].onClick.RemoveAllListeners();
            ItemIconButton[tempIdx].onClick.AddListener(()=> {
                Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
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
            //Debug.Log(" ------------- CharacterWindow 로딩 ------------- ");
            DataInfo.ins.CharacterSub = JsonUtility.FromJson<Info_Char>(DataInfo.ins.SaveData);
        }

        if (DataInfo.ins.CharacterSub.NicName == null || DataInfo.ins.CharacterSub.NicName == "")
        {
            NicNameObj.text = "닉네임을 설정하세요";
        }
        else
        {
            NicNameObj.text = DataInfo.ins.CharacterSub.NicName;
        }

        ChangedNicName = NicNameSetter.GetComponentInChildren<InputField>();

        SetterEndButton = SetterEndObj.GetComponent<Button>();
        SetterEndButton.onClick.RemoveAllListeners();
        SetterEndButton.onClick.AddListener(OpenPopup);

        ExitPopupObj.SetActive(false);
        BuyPopup.gameObject.SetActive(false);
        MyRoomSelectPopup.SetActive(false);
        Popup_Option.SetActive(false);

        playerManger.itemEquipment(DataInfo.ins.CharacterSub);

        StopAllCoroutines();

        if (DataInfo.ins.CharacterSub.Sex == 1)
            OnClick_SexMale();
        else
            OnClick_SexFemale();

        OnClick_IndexButton(6);
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
                case 4:
                    OnClick_BuyPopupClose();
                    break;
                case 10:
                    Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
                    PopupOpenCheck = 0;
                    Popup_Option.SetActive(false);
                    break;
            }
        }

        if (DataInfo.ins.cWin_OpenBuyPopup == 1)
        {
            OnClick_BuyPopupOpen();
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
            case 6: PrintStr += "구매한 아이템"; break;
        }

        if( DataInfo.ins.InvenNumber >= 0)
        {
            Image OldButImage = ItemIconButton[DataInfo.ins.InvenNumber].GetComponent<Image>();
            OldButImage.color = Color.white;
        }

        StartCoroutine(ClickEvnetCall(idx));

        Image tempImage = ItemIconButton[idx].GetComponent<Image>();
        tempImage.color = SelectColor;

        DataInfo.ins.ItemSelectIndex = -1;

        //Debug.Log(PrintStr);
    }

    IEnumerator ClickEvnetCall(int idx)
    {
        //ReSetScrollView();

        yield return null;

        DataInfo.ins.InvenNumber = idx;

        if (setScroll == null)
            setScroll = InvenScrollView.GetComponent<InitScroll>();

        DataInfo.ins.CostumeScrollList.Clear();

        if (idx == 6) {
            //구매한 아이템버튼 추가
            for(int parts = 0; parts < DataInfo.ins.CoustumList.Length; parts++)
            {
                for(int cnt = 0; cnt < DataInfo.ins.CoustumList[parts].Count; cnt++)
                {
                    if (DataInfo.ins.CoustumList[parts][cnt].State == 1)
                    {
                        if (DataInfo.ins.CoustumList[parts][cnt].Sex == DataInfo.ins.CharacterSub.Sex || DataInfo.ins.CoustumList[parts][cnt].Sex == 2)
                            DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[parts][cnt]);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < DataInfo.ins.CoustumList[idx].Count; i++)
            {
                //성별이 공통이거나 같을때에 리스트에 추가
                if (DataInfo.ins.CoustumList[idx][i].Sex == DataInfo.ins.CharacterSub.Sex || DataInfo.ins.CoustumList[idx][i].Sex == 2)
                {
                    DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[idx][i]);
                }
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
        //NicNameSetter.SetActive(true);
        //SetterEndObj.SetActive(false);
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

        //NicNameSetter.SetActive(false);
        //SetterEndObj.SetActive(true);

        //info 동기화
        //DataInfo.ins.CharacterMain.NicName = DataInfo.ins.CharacterSub.NicName;
    }

    public void OnClick_ItemRestButton() {
        //Debug.Log("착용 아이템 초기화");
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));

        if (DataInfo.ins.SaveData != "" && DataInfo.ins.SaveData != null)
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
            Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
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
            Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
            DataInfo.ins.CharacterSub.Sex = 0;
            DataInfo.ins.CharacterSub.Hair = 0;

            playerManger.itemEquipment(DataInfo.ins.CharacterSub);
            OnClick_IndexButton(DataInfo.ins.InvenNumber);
        }
    }

    void OpenPopup()
    {
        if (DataInfo.ins.CharacterSub.NicName == null || DataInfo.ins.CharacterSub.NicName == "")
        {
            ClosePopup();
            OpenCharPopupData("알림", "케릭명을 설정해 주세요");
            return;
        }

        PopupOpenCheck = 1;
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Pop Up"));
        ExitPopupObj.SetActive(true);

        ExitPopupYes.onClick.RemoveAllListeners();
        ExitPopupNo.onClick.RemoveAllListeners();
        if (DataInfo.ins.BuyItemSaveList.Count <= 0)
        {
            ExitPopupText.text = "현재의 상태를\n저장 하고 나가 시겠습니까?";
            ExitPopupNo.onClick.AddListener(ClosePopup);
            ExitPopupYes.onClick.AddListener(SaveExitCharSettingPage);
        }
        else
        {
            ExitPopupText.text = "구매 되지 않은 아이템은\n 착용 해제 됩니다.\n해제된 아이템 구매는\n상점에서 가능합니다.\n게임화면으로 나가 시겠습니까?";
            ExitPopupNo.onClick.AddListener(ClosePopup);
            ExitPopupYes.onClick.AddListener(CharCostumSettingOut);
        }
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

    public void OnClick_QuitPopupOk()
    {
        Application.Quit();
    }

    public void SaveExitCharSettingPage() {
        
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Select"));
        Debug.Log("---------- Save Exit CharSetting Page --------");
        DataInfo.ins.CharacterSub.Money = DataInfo.ins.CharacterMain.Money;
        DataInfo.ins.SaveData = JsonUtility.ToJson(DataInfo.ins.CharacterSub);

        Debug.Log(DataInfo.ins.SaveData);
        DataInfo.ins.CharacterMain = DataInfo.ins.CharacterSub;

        //LoadScene(NectSceneName);
        MyRoomSelectPopup.SetActive(true);
    }

    void CharCostumSettingOut()
    {
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Select"));

        for(int i = 0; i < DataInfo.ins.BuyItemSaveList.Count; i++)
        {
            if (DataInfo.ins.BuyItemSaveList[i].State != 0)
                continue;

            switch (DataInfo.ins.BuyItemSaveList[i].Type)
            {
                case 0: //머리
                        //기본 남녀의 헤어가 다르기 때문에 분기   
                    if (DataInfo.ins.CharacterSub.Sex == 1)
                    {
                        DataInfo.ins.CharacterSub.Hair = 5;
                    }
                    else
                    {
                        DataInfo.ins.CharacterSub.Hair = 0;
                    }
                    break;
                case 1: //셔츠
                case 4: //세트
                    if(DataInfo.ins.BuyItemSaveList[i].Type == 4)
                    {//바지 자동 기본 설정
                        DataInfo.ins.CharacterSub.Pants = 15;
                    }
                    DataInfo.ins.CharacterSub.Shirt = 10;
                    break;
                case 2: //바지
                    DataInfo.ins.CharacterSub.Pants = 15;
                    break;
                case 3: //신발
                    DataInfo.ins.CharacterSub.Shoes = 19;
                    break;
                case 5: // 악세사리
                    DataInfo.ins.CharacterSub.Accessory = -1;
                    break;
            }
        }

        Debug.Log("---------- Char Costum Setting Out --------");
        DataInfo.ins.CharacterSub.Money = DataInfo.ins.CharacterMain.Money;
        DataInfo.ins.SaveData = JsonUtility.ToJson(DataInfo.ins.CharacterSub);
        DataInfo.ins.CharacterMain = DataInfo.ins.CharacterSub;

        Debug.Log(DataInfo.ins.SaveData);

        //LoadScene(NectSceneName);
        MyRoomSelectPopup.SetActive(true);
    }

    public void LoadScene(string sceneName)
    {
        progressBar.fillAmount = 0;
        PageLodingPopup.SetActive(true);

        DataInfo.ins.OldScneName = SceneManager.GetActiveScene().name;
        nextScene = sceneName;

        DataInfo.ins.SceneNameCheck();

        StartCoroutine(coroutineLoadScene());
    }

    IEnumerator coroutineLoadScene()
    {
        yield return null;

        AsyncOperation op;
        op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;

        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);

                if (progressBar.fillAmount >= 0.99f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    public void OnClick_BuyPopupOpen()
    {
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Pop Up"));
        PopupOpenCheck = 4;
        DataInfo.ins.cWin_OpenBuyPopup = 2;
        BuyPopup.gameObject.SetActive(true);
        BuyPopup.ButtonSetting(ReflashScroll, OnClick_BuyPopupClose, NoMoneyPopupCall);
    }

    public void OnClick_BuyPopupClose()
    {
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
        PopupOpenCheck = 0;
        DataInfo.ins.cWin_OpenBuyPopup = 0;
        BuyPopup.gameObject.SetActive(false);
    }

    void ReflashScroll()
    {
        StartCoroutine(ClickEvnetCall(DataInfo.ins.InvenNumber));
    }

    void NoMoneyPopupCall()
    {
        OpenCharPopupData("알림", "보유금액이 부족합니다.");
    }

    public void OnClick_RoomSelect(int idx)
    {
        switch (idx)
        {
            case 0:
                DataInfo.ins.MyRoomName = "Room_A";
                break;
            case 1:
                DataInfo.ins.MyRoomName = "Room_B";
                break;
        }
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
        LoadScene(DataInfo.ins.MyRoomName);
    }

    public void OnClick_OptionOpen()
    {
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Pop Up"));
        PopupOpenCheck = 10;
        Popup_Option.SetActive(true);
    }
}
