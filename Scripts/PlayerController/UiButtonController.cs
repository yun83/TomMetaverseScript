using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiButtonController : MonoBehaviour
{
    public delegate void Callback();

    [Header("�ɸ���")]
    public CharacterManager myPlayer;
    private Animator myAnimator;

    [Header("��ư ����")]
    public Button[] UiButton;
    public GameObject QuestSuccess;
    private bool SuccesssOn = true;

    [Header("����Ui")]
    public Text MoneyText;
    public GameObject CharObj;
    public GameObject GestureScroll;
    public GameObject OptionPopup;
    RectTransform rtGestureScroll;
    InitScroll setGestureScroll;
    public GameObject ShopPopup;
    public GameObject InvenPopup;
    public GameObject QuestPopup;
    public GameObject RoulettePopup;
    public OutRoomPopup OR_Popup;
    public PopupController popupController;
    public UiMessage ToastMes;

    private int UiPopupState = 0;

    public List<CoustumItemCsv> GestureItem = new List<CoustumItemCsv>();

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

        //��ư Ŭ�� ���� ����
        for (int i = 0; i < UiButton.Length; i++)
            UiButton[i].onClick.RemoveAllListeners();

        UiButton[0].onClick.AddListener(OnClick_Gesture);
        UiButton[1].onClick.AddListener(OnClick_Inven);
        UiButton[2].onClick.AddListener(OnClick_Shop);
        UiButton[3].onClick.AddListener(OnClick_Quest);
        UiButton[4].onClick.AddListener(OnClick_Option);
        UiButton[5].onClick.AddListener(() =>
        {
            OnClick_CloseAllPopup();
            switch (DataInfo.ins.State)
            {
                case 0:
                    OnClick_Exit();
                    break;
                case 1:
                    DataInfo.ins.RoomOutButtonSetting();
                    break;
                case 2:
                    DataInfo.ins.WorldMapOutButtonSetting();
                    break;
            }
        });
        //all popup clase    
        OnClick_CloseAllPopup();
    }

    void OnEnable()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            default:
                DataInfo.ins.State = -1;
                break;
            case "Room_A":
                DataInfo.ins.State = 1;
                break;
            case "World_A":
                DataInfo.ins.State = 2;
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Click"));
            OnClick_CloseAllPopup();
        }

        if (DataInfo.ins.Quest_WinState == 1)
        {
            if (!SuccesssOn)
            {
                SuccesssOn = true;
                QuestSuccess.SetActive(true);
            }
        }
        else
        {
            if (SuccesssOn)
            {
                SuccesssOn = false;
                QuestSuccess.SetActive(false);
            }
        }
    }


    public void CallToastMassage(string mas, float ft = 0.1f)
    {
        ToastMes.OnMessage(mas, ft);
    }

    public void OnClick_CloseAllPopup()
    {
        switch (UiPopupState)
        {//������� �б� ����
            case 1:
                //������ �������� �˾� ����� �ɸ����� �ǻ� ����
                StartCoroutine(ShopItemSetting());
                break;
            case 2:
                //�κ� �丮�� �˾� ����� �ɸ��� �ǻ� ������
                DataInfo.ins.CharacterMain.Hair = DataInfo.ins.CharacterSub.Hair;
                DataInfo.ins.CharacterMain.Shirt = DataInfo.ins.CharacterSub.Shirt;
                DataInfo.ins.CharacterMain.Pants = DataInfo.ins.CharacterSub.Pants;
                DataInfo.ins.CharacterMain.Shoes = DataInfo.ins.CharacterSub.Shoes;
                DataInfo.ins.CharacterMain.Accessory = DataInfo.ins.CharacterSub.Accessory;
                DataInfo.ins.SaveData = JsonUtility.ToJson(DataInfo.ins.CharacterMain);
                myPlayer.itemEquipment(DataInfo.ins.CharacterMain);
                break;
        }

        UiPopupState = 0;
        //all popup clase
        GestureScroll.SetActive(false);
        OptionPopup.SetActive(false);
        popupController.gameObject.SetActive(false);
        QuestPopup.SetActive(false);
        ShopPopup.SetActive(false);
        InvenPopup.SetActive(false);
        CharObj.SetActive(false);
        RoulettePopup.SetActive(false);
        OR_Popup.gameObject.SetActive(false);
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
        InvenPopup.SetActive(true);
        CharObj.SetActive(true);

        UiPopupState = 2;

        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Pop Up"));
    }
    public void OnClick_Shop()
    {
        OnClick_CloseAllPopup();
        ShopPopup.SetActive(true);
        CharObj.SetActive(true);

        UiPopupState = 1;

        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Pop Up"));
    }
    public void OnClick_Quest()
    {
        OnClick_CloseAllPopup();
        QuestPopup.SetActive(true);
    }
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

        //���� ��ġ�� �����Ͽ� �б� �Ͽ��� �Ѵ�. ���̷��� ���� ����
        popupController.gameObject.SetActive(true);
        popupController.PopupYesNo("�˸�", "������ ���� �Ͻðڽ��ϱ�?", new Vector2(400, 300),
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

    public void OnClick_OutRoomPopup(List<ButtonClass> array)
    {
        OR_Popup.gameObject.SetActive(true);
        OR_Popup.InitOutRoomPopup(array);
    }

    public void OnClick_Roulette()
    {
        RoulettePopup.SetActive(true);
    }

    #endregion


    IEnumerator GestureScrollSetting()
    {
        int Size = rtGestureScroll.childCount;

        yield return new WaitForEndOfFrame();

        DataInfo.ins.CostumeScrollList.Clear();
        for (int i = 0; i < DataInfo.ins.EctItemData.Count; i++)
        {
            if (DataInfo.ins.EctItemData[i].State == 1) {
                CoustumItemCsv temp = DataInfo.ins.EctItemData[i];
                DataInfo.ins.CostumeScrollList.Add(temp);
            }
        }
        setGestureScroll.totalCount = DataInfo.ins.CostumeScrollList.Count;

        setGestureScroll.InitScrollCall();
    }

    IEnumerator ShopItemSetting()
    {
        bool SetItemCheck = false;

        //��� ID�� Itemã��
        for (int i = 0; i < DataInfo.ins.CoustumList[0].Count; i++)
        {
            if (DataInfo.ins.CoustumList[0][i].ItemID == DataInfo.ins.CharacterSub.Hair)
            {
                if(DataInfo.ins.CoustumList[0][i].State == 1)
                {// �������� ���� ���� ��� �ش� �������� �ɸ��Ϳ� �����Ѵ�
                    DataInfo.ins.CharacterMain.Hair = DataInfo.ins.CoustumList[0][i].ItemID;
                }
                break;
            }
        }

        yield return null;

        //���� �˻�
        for (int i = 0; i < DataInfo.ins.CoustumList[1].Count; i++)
        {
            //������ ������ ���̵� �˻��� ������� ��Ʈ ������
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
            //��Ʈ �۰˻�
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

        //���� �˻�
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

        //�Ź� �˻�
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

        //�Ǽ����� �����.
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
