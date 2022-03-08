using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiButtonController : MonoBehaviour
{
    [Header("케릭터")]
    public CharacterManager myPlayer;
    private Animator myAnimator;

    [Header("버튼 설정")]
    public Button[] UiButton;

    [Header("게임Ui")]
    public GameObject CharObj;
    public GameObject GestureScroll;
    public GameObject OptionPopup;
    RectTransform rtGestureScroll;
    InitScroll setGestureScroll;
    public GameObject ShopPopup;
    public PopupController popupController;
    public UiMessage ToastMes;

    private int UiPopupState = 0;

    void Awake()
    {
        if (myPlayer == null)
            myPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterManager>();

        myAnimator = myPlayer.GetComponentInChildren<Animator>();
        DataInfo.ins.MyPlayerAnimator = myAnimator;

        if (ToastMes == null)
        ToastMes = transform.GetComponentInChildren<UiMessage>();

        rtGestureScroll = GestureScroll.GetComponent<LoopVerticalScrollRect>().content;
        setGestureScroll = GestureScroll.GetComponent<InitScroll>();
        if(popupController == null)
            popupController = transform.GetComponentInChildren<PopupController>();

        //버튼 클릭 반응 삽입
        for (int i = 0; i < UiButton.Length; i++)
            UiButton[i].onClick.RemoveAllListeners();

        UiButton[0].onClick.AddListener(OnClick_Gesture);
        UiButton[1].onClick.AddListener(OnClick_Inven);
        UiButton[2].onClick.AddListener(OnClick_Shop);
        UiButton[3].onClick.AddListener(OnClick_Quest);
        UiButton[4].onClick.AddListener(OnClick_Option);
        UiButton[5].onClick.AddListener(OnClick_Exit);

        //all popup clase
        OnClick_CloseAllPopup();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
            OnClick_CloseAllPopup();
        }
    }

    public void OnClick_CloseAllPopup()
    {
        switch (UiPopupState)
        {//종료시의 분기 설정
            case 1:
                //아이템 샵에서의 팝업 종료시 케릭터의 의상 변경
                StartCoroutine(ShopItemSetting());
                break;
        }

        UiPopupState = 0;
        //all popup clase
        GestureScroll.SetActive(false);
        OptionPopup.SetActive(false);
        popupController.gameObject.SetActive(false);
        ShopPopup.SetActive(false);
        CharObj.SetActive(false);
    }

    #region UI Button Click Setting
    public void OnClick_Gesture() {

        if (GestureScroll.activeSelf)
        {
            Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
            GestureScroll.SetActive(false);
        }
        else
        {
            OnClick_CloseAllPopup();

            Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Pop Up"));
            GestureScroll.SetActive(true);
            StartCoroutine(GestureScrollSetting());
        }
    }
    public void OnClick_Inven()
    {
        OnClick_CloseAllPopup();
        CharObj.SetActive(true);
    }
    public void OnClick_Shop()
    {
        OnClick_CloseAllPopup();
        ShopPopup.SetActive(true);
        CharObj.SetActive(true);

        UiPopupState = 1;

        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Pop Up"));
    }
    public void OnClick_Quest() { }
    public void OnClick_Option()
    {
        OnClick_CloseAllPopup();

        OptionPopup.SetActive(true);
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Pop Up"));
    }
    public void OnClick_Exit()
    {
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Pop Up"));

        OnClick_CloseAllPopup();

        //현재 위치에 관련하여 분기 하여야 한다. 마이룸일 경우는 종료
        popupController.gameObject.SetActive(true);
        popupController.PopupYesNo("알림", "게임을 종료 하시겠습니까?", new Vector2(400, 300),
            () =>
            {
                Application.Quit();
            },
            () =>
            {
                Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
                popupController.gameObject.SetActive(false);
            });
    }

    #endregion


    IEnumerator GestureScrollSetting()
    {
        int Size = rtGestureScroll.childCount;

        for (int i = Size - 1; i >= 0; i--)
        {
            Destroy(rtGestureScroll.GetChild(i).gameObject);
        }

        yield return new WaitForEndOfFrame();
        //yield return new WaitForSeconds(1.0f);
        setGestureScroll.totalCount = DataInfo.ins.TriggerName.Length;

        setGestureScroll.InitScrollCall();
    }

    IEnumerator ShopItemSetting()
    {
        bool SetItemCheck = false;

        //헬멧 ID로 Item찾기
        for (int i = 0; i < DataInfo.ins.CoustumList[0].Count; i++)
        {
            if (DataInfo.ins.CoustumList[0][i].ItemID == DataInfo.ins.CharacterSub.Hair)
            {
                if(DataInfo.ins.CoustumList[0][i].State == 1)
                {// 아이템을 보유 중일 경우 해당 아이템을 케릭터에 대입한다
                    DataInfo.ins.CharacterMain.Hair = DataInfo.ins.CoustumList[0][i].ItemID;
                }
                break;
            }
        }

        yield return null;

        //셔츠 검사
        for (int i = 0; i < DataInfo.ins.CoustumList[1].Count; i++)
        {
            //셔츠의 아이템 아이디가 검색시 없을경우 세트 템으로
            SetItemCheck = true;

            if (DataInfo.ins.CoustumList[1][i].ItemID == DataInfo.ins.CharacterSub.Shirt)
            {
                SetItemCheck = false;
                if (DataInfo.ins.CoustumList[1][i].State == 1) { 
                    DataInfo.ins.CharacterMain.Shirt = DataInfo.ins.CoustumList[1][i].ItemID;
                }
                break;
            }
        }
        if (SetItemCheck)
        {
            //세트 템검사
            for (int i = 0; i < DataInfo.ins.CoustumList[4].Count; i++)
            {
                if (DataInfo.ins.CoustumList[4][i].ItemID == DataInfo.ins.CharacterSub.Shirt)
                {
                    if (DataInfo.ins.CoustumList[4][i].State == 1)
                    {
                        DataInfo.ins.CharacterMain.Shirt = DataInfo.ins.CoustumList[4][i].ItemID;
                    }
                    break;
                }
            }
        }

        yield return null;

        //바지 검사
        for (int i = 0; i < DataInfo.ins.CoustumList[2].Count; i++)
        {
            if (DataInfo.ins.CoustumList[2][i].ItemID == DataInfo.ins.CharacterSub.Pants)
            {
                if (DataInfo.ins.CoustumList[2][i].State == 1)
                {
                    DataInfo.ins.CharacterMain.Pants = DataInfo.ins.CoustumList[2][i].ItemID;
                }
                break;
            }
        }

        yield return null;

        //신발 검사
        for (int i = 0; i < DataInfo.ins.CoustumList[3].Count; i++)
        {
            if (DataInfo.ins.CoustumList[3][i].ItemID == DataInfo.ins.CharacterSub.Shoes)
            {
                if (DataInfo.ins.CoustumList[3][i].State == 1)
                {
                    DataInfo.ins.CharacterMain.Shoes = DataInfo.ins.CoustumList[3][i].ItemID;
                }
                break;
            }
        }

        //악세서리 착용시.
        if (DataInfo.ins.CharacterSub.Accessory > 0)
        {
            for (int i = 0; i < DataInfo.ins.CoustumList[4].Count; i++)
            {
                if (DataInfo.ins.CoustumList[5][i].ItemID == DataInfo.ins.CharacterSub.Accessory)
                {

                    if (DataInfo.ins.CoustumList[5][i].State == 1)
                    {
                        DataInfo.ins.CharacterMain.Accessory = DataInfo.ins.CoustumList[5][i].ItemID;
                    }
                    break;
                }
            }
        }

        yield return null;

        DataInfo.ins.SaveData = JsonUtility.ToJson(DataInfo.ins.CharacterMain);

        yield return null;

        myPlayer.itemEquipment(DataInfo.ins.CharacterMain);

    }
}
