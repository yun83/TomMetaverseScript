using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopup : MonoBehaviour
{
    public CharacterManager ShopCharacter;
    public InitScroll setScroll;

    public Text NicName;

    private Vector2 nowPos, prePos;
    private Vector2 movePosDiff;

    public Button[] ShopButton;
    public Color ShopButtonSelectColor = Color.yellow;
    public GameObject SubButton;
    public GameObject[] SelectDetailedMenu;

    [Header("결제 팝업")]
    public Button Purchase;
    Text PurchaseText;
    public GameObject BuyPopup;
    public InitScroll BuyPopupInitScroll;
    public Text TotlaPrice;
    public Button PurchaseClose;
    public Button PurchaseBuy;
    public Button AllSelect;
    public Button AllDelete;

    int State = -1;
    int subState = -1;
    int sumMoney = 0;
    int ItemIndexCheck = 0;

    private bool PurchaseUse = false;
    private RectTransform scrollRect;

    // Start is called before the first frame update
    void Start()
    {
        PurchaseUse = false;

        Purchase.onClick.RemoveAllListeners();
        PurchaseClose.onClick.RemoveAllListeners();
        PurchaseBuy.onClick.RemoveAllListeners();
        AllSelect.onClick.RemoveAllListeners();
        AllDelete.onClick.RemoveAllListeners();

        for (int i = 0; i < ShopButton.Length; i++)
        {
            ShopButton[i].onClick.RemoveAllListeners();
        }

        ShopButton[0].onClick.AddListener(OnClick_Suggestion);
        ShopButton[1].onClick.AddListener(OnClick_Item);
        ShopButton[2].onClick.AddListener(OnClick_Gesture);
        ShopButton[3].onClick.AddListener(OnClick_MyRoom);

        Purchase.onClick.AddListener(OnClick_Purchase);

        PurchaseClose.onClick.AddListener(() =>
        {
            BuyPopup.SetActive(false);
        });

        PurchaseBuy.onClick.AddListener(() => { 
            if(!PurchaseUse)
                StartCoroutine(OnClick_purchaseBuy()); 
        });

        AllSelect.onClick.AddListener(() => { });
        AllDelete.onClick.AddListener(() => { });

        scrollRect = setScroll.GetComponent<RectTransform>();

        State = 0;
        SubButton.SetActive(true);
        SuggestonSetting();
    }
    
    private void OnEnable()
    {
        if (!DataInfo.ins.SaveData.Equals("") && DataInfo.ins.SaveData != null)
        {
            DataInfo.ins.CharacterSub = JsonUtility.FromJson<Info_Char>(DataInfo.ins.SaveData);
        }

        NicName.text = DataInfo.ins.CharacterSub.NicName;

        ShopCharacter.itemEquipment(DataInfo.ins.CharacterSub);
        DataInfo.ins.SubCharAnimator = ShopCharacter.GetComponentInChildren<Animator>();

        DataInfo.ins.BuyItemSaveList.Clear();
        PurchaseText = Purchase.GetComponentInChildren<Text>();
        Purchase.interactable = false;
        PurchaseText.text = "0";

        BuyPopup.SetActive(false);
        
        if(State != 1)
            SubButton.SetActive(false);
    }

    void FixedUpdate()
    {
        if (DataInfo.ins.CharacterChangeCheck)
        {
            ShopCharacter.itemEquipment(DataInfo.ins.CharacterSub);
            PurchaseItemSetting();
            DataInfo.ins.CharacterChangeCheck = false;
        }

        if (DataInfo.ins.TotlaMoneySumCheck)
        {
            int getSize = DataInfo.ins.BuyItemSaveList.Count;
            sumMoney = 0;
            for(int i = 0; i < DataInfo.ins.BuyItemSaveList.Count; i++)
            {
                if(DataInfo.ins.BuyItemSaveList[i].inGameUse != 0)
                {
                    sumMoney += DataInfo.ins.BuyItemSaveList[i].price;
                }
            }
            TotlaPrice.text = "Total " + sumMoney + " Gold";
            DataInfo.ins.TotlaMoneySumCheck = false;
            PurchaseText.text = getSize.ToString();
            if (getSize > 0)
                Purchase.interactable = true;
            else
                Purchase.interactable = false;
        }

        if(DataInfo.ins.ItemSelectIndex != ItemIndexCheck)
        {
            ItemIndexCheck = DataInfo.ins.ItemSelectIndex;
            if (ItemIndexCheck >= 0)
            {
                ShopButtonColorChagne();
            }
        }
    }

    void PurchaseItemSetting()
    {
        if (DataInfo.ins.CharacterSub.Hair != DataInfo.ins.CharacterMain.Hair)
        {
            AddBuyCostumes(DataInfo.ins.CharacterSub.Hair);
        }
        if (DataInfo.ins.CharacterSub.Shirt != DataInfo.ins.CharacterMain.Shirt)
        {
            AddBuyCostumes(DataInfo.ins.CharacterSub.Shirt);
        }
        if (DataInfo.ins.CharacterSub.Pants != DataInfo.ins.CharacterMain.Pants)
        {
            AddBuyCostumes(DataInfo.ins.CharacterSub.Pants);
        }
        if (DataInfo.ins.CharacterSub.Shoes != DataInfo.ins.CharacterMain.Shoes)
        {
            AddBuyCostumes(DataInfo.ins.CharacterSub.Shoes);
        }
        if (DataInfo.ins.CharacterSub.Accessory != DataInfo.ins.CharacterMain.Accessory)
        {
            AddBuyCostumes(DataInfo.ins.CharacterSub.Accessory);
        }

        if (DataInfo.ins.BuyItemSaveList.Count > 0)
        {
            Purchase.interactable = true;
            PurchaseText.text = DataInfo.ins.BuyItemSaveList.Count.ToString();
        }
    }

    void AddBuyCostumes(int itemId)
    {
        CoustumItemCsv temp = new CoustumItemCsv();
        temp = DataInfo.ins.getItemData(itemId);
        if (temp != null)
        {
            //아이템의 아이디가 검색 되면 해당 아이템이 구매 가능한 아이템인지 확인
            if (temp.State == 0)
            {
                //Costumes.Add(temp);
                //저장된 구매 가능 코스튬이 있을 경우
                if (DataInfo.ins.BuyItemSaveList.Count > 0)
                {
                    for (int i = 0; i < DataInfo.ins.BuyItemSaveList.Count; i++)
                    {
                        //한번에 구매 가능한 타입을 1개로 제한 상의는 변경된 상의 한가지 형태로
                        if (temp.Type == DataInfo.ins.BuyItemSaveList[i].Type) {
                            //저장 되었던 코스튬의 데이터 제거
                            DataInfo.ins.BuyItemSaveList.RemoveAt(i);
                            break;
                        }
                    }
                }

                temp.inGameUse = 1;
                //구매 하려는 아이템에 저장
                DataInfo.ins.BuyItemSaveList.Add(temp);
            }
        }
        //Debug.Log("Buy Item List Size " + DataInfo.ins.BuyItemSaveList.Count);
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
            ShopCharacter.transform.Rotate(0, movePosDiff.x, 0);
        }
        else if (movePosDiff.x > 0)
        {
            ShopCharacter.transform.Rotate(0, movePosDiff.x, 0);
        }
    }

    public void OnClick_Suggestion()
    {
        if (State != 0)
        {
            State = 0;
            //이전에 클릭된 버튼이 존재한다
            if (subState >= 0)
                ShopButton[subState].GetComponent<Image>().color = Color.white;

            SuggestonSetting();

        }
    }

    void SuggestonSetting()
    {
        ShopButton[State].GetComponent<Image>().color = ShopButtonSelectColor;
        subState = State;

        //아이템 리스트 셋팅
        DataInfo.ins.CostumeScrollList.Clear();
        for (int i = 0; i < DataInfo.ins.CoustumList.Length; i++)
        {
            for (int k = 0; k < DataInfo.ins.CoustumList[i].Count; k++)
            {
                //아이템 데이터에 추천 항목 축가 하여 추천 검사
                if (DataInfo.ins.CoustumList[i][k].State == 1)
                {
                    if (DataInfo.ins.CoustumList[i][k].Sex == DataInfo.ins.CharacterSub.Sex
                        || DataInfo.ins.CoustumList[i][k].Sex == 2)
                    {
                        DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[i][k]);
                    }
                }
            }
        }

        SubButton.SetActive(true);
        scrollRect.offsetMax = (new Vector2(0, -150));
        ClickCheckClose(0);

        StartCoroutine(ScrollViewSetting());
    }

    public void OnClick_Item()
    {
        if (State != 1)
        {
            State = 1;
            //이전에 클릭된 버튼이 존재한다
            if (subState >= 0)
                ShopButton[subState].GetComponent<Image>().color = Color.white;
            ShopButton[State].GetComponent<Image>().color = ShopButtonSelectColor;
            subState = State;

            setScroll.totalCount = 0;
            //아이템 리스트 셋팅
            DataInfo.ins.CostumeScrollList.Clear();
            for (int i = 0; i < DataInfo.ins.CoustumList.Length; i++)
            {
                for (int k = 0; k < DataInfo.ins.CoustumList[i].Count; k++)
                {
                    //성별 검사 하여 현재 나에게 맞는 성멸의 아이템만 출력
                    if (DataInfo.ins.CoustumList[i][k].Sex == DataInfo.ins.CharacterSub.Sex
                        || DataInfo.ins.CoustumList[i][k].Sex == 2)
                        DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[i][k]);
                }
            }

            SubButton.SetActive(true);
            scrollRect.offsetMax = (new Vector2(0, -150));
            ClickCheckClose(0);

            StartCoroutine(ScrollViewSetting());
        }
    }

    #region detailed Menu Button Setting
    void ClickCheckClose(int idx)
    {
        for(int i = 0; i < SelectDetailedMenu.Length; i++)
        {
            SelectDetailedMenu[i].SetActive(false);
        }
        SelectDetailedMenu[idx].SetActive(true);
    }

    public void OnClick_detailedMenu_All()
    {
        ClickCheckClose(0);
        //아이템 리스트 셋팅
        ItemListReSet(-1);
    }
    public void OnClick_detailedMenu_Hair()
    {
        ClickCheckClose(1);
        //아이템 리스트 셋팅
        ItemListReSet(0);
    }
    public void OnClick_detailedMenu_Shirt()
    {
        ClickCheckClose(2);
        //아이템 리스트 셋팅
        ItemListReSet(1);
    }
    public void OnClick_detailedMenu_Pants()
    {
        ClickCheckClose(3);
        //아이템 리스트 셋팅
        ItemListReSet(2);
    }
    public void OnClick_detailedMenu_Shoes()
    {
        ClickCheckClose(4);
        //아이템 리스트 셋팅
        ItemListReSet(3);
    }
    public void OnClick_detailedMenu_Set()
    {
        ClickCheckClose(5);
        //아이템 리스트 셋팅
        ItemListReSet(4);

        StartCoroutine(ScrollViewSetting());
    }
    public void OnClick_detailedMenu_Accessory()
    {
        ClickCheckClose(6);

        ItemListReSet(5);

        StartCoroutine(ScrollViewSetting());
    }
    #endregion

    void ShopButtonColorChagne()
    {
        //setScroll
    }

    public void OnClick_Gesture()
    {
        if (State != 2)
        {
            State = 2;
            //이전에 클릭된 버튼이 존재한다
            if (subState >= 0)
                ShopButton[subState].GetComponent<Image>().color = Color.white;
            ShopButton[State].GetComponent<Image>().color = ShopButtonSelectColor;
            subState = State;

            setScroll.totalCount = 0;
            //아이템 리스트 셋팅
            DataInfo.ins.CostumeScrollList.Clear();
            SubButton.SetActive(false);
            scrollRect.offsetMax = (new Vector2(0, -90));

            for (int i = 0; i < DataInfo.ins.EctItemData.Count; i++)
            {
                //아이템 데이터에 추천 항목 축가 하여 추천 검사
                if (DataInfo.ins.EctItemData[i].Type == 100)
                {
                    DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.EctItemData[i]);
                }
            }
            StartCoroutine(ScrollViewSetting());
        }
    }

    public void OnClick_Purchase()
    {
        BuyPopup.SetActive(true);
        
        DataInfo.ins.ItemSelectIndex = -1;

        BuyPopupInitScroll.totalCount = DataInfo.ins.BuyItemSaveList.Count;
        BuyPopupInitScroll.InitScrollCall();

        DataInfo.ins.TotlaMoneySumCheck = true;

    }

    IEnumerator ScrollViewSetting()
    {
        //ReSetScrollView();

        yield return null;

        setScroll.gameObject.SetActive(true);
        DataInfo.ins.ItemSelectIndex = -1;

        yield return null;

        setScroll.totalCount = DataInfo.ins.CostumeScrollList.Count;
        setScroll.InitScrollCall();
    }

    void ReSetScrollView()
    {
        RectTransform ContentRect = setScroll.GetComponent<LoopVerticalScrollRect>().content;
        int Size = ContentRect.childCount;

        setScroll.GetComponent<LoopVerticalScrollRect>().totalCount = 0;
        setScroll.totalCount = 0;

        if (Size <= 0)
            return;

        for (int i = Size - 1; i >= 0; i--)
        {
            Destroy(ContentRect.GetChild(i).gameObject);
        }

    }

    IEnumerator OnClick_purchaseBuy()
    {
        //연속 클릭 방지
        PurchaseUse = true;
        //구매처리
        string savetrunk = "";

        sumMoney = 0;
        for (int i = 0; i < DataInfo.ins.BuyItemSaveList.Count; i++)
        {
            if (DataInfo.ins.BuyItemSaveList[i].inGameUse == 0)
                continue;

            //DataInfo.ins.BuyItemSaveList 의 아이템을 구매 한다
            CoustumItemCsv itemTrunk = DataInfo.ins.BuyItemSaveList[i];

            DataInfo.ins.BuyItemId.Add(System.Convert.ToInt32(itemTrunk.ItemID));
            sumMoney += itemTrunk.price;

            //구매된 아이템 체크
            if (itemTrunk.Type < 6)
            {
                for (int cnt = 0; cnt < DataInfo.ins.CoustumList[itemTrunk.Type].Count; cnt++)
                {
                    if (DataInfo.ins.CoustumList[itemTrunk.Type][cnt].ItemID == itemTrunk.ItemID)
                    {
                        DataInfo.ins.CoustumList[itemTrunk.Type][cnt].State = 1;
                        continue;
                    }
                }
            }
            else
            {
                for (int cnt = 0; cnt < DataInfo.ins.EctItemData.Count; cnt++)
                {
                    if (DataInfo.ins.EctItemData[cnt].ItemID == itemTrunk.ItemID)
                    {
                        DataInfo.ins.EctItemData[cnt].State = 1;
                        continue;
                    }
                }
            }
        }

        yield return null;

        DataInfo.ins.BuyItemId = DataInfo.ins.BuyItemId.Distinct().ToList();

        for (int i = 0; i < DataInfo.ins.BuyItemId.Count; i++)
        {
            savetrunk += DataInfo.ins.BuyItemId[i] + ",";
        }
        DataInfo.ins.saveBuyItem = savetrunk;
        //Debug.Log(savetrunk);
        BuyPopup.SetActive(false);
        setScroll.gameObject.SetActive(false);

        yield return null;

        if (DataInfo.ins.CharacterMain.Money >= sumMoney)
        {
            DataInfo.ins.AddMoney(-sumMoney);
        }


        yield return null;

        DataInfo.ins.BuyItemSaveList.Clear();

        switch (State)
        {
            case 1:
                State = -1;
                OnClick_Item();
                break;
            case 2:
                State = -1;
                OnClick_Gesture();
                break;
            default:
                State = -1;
                OnClick_Suggestion();
                break;
        }

        PurchaseText.text = "0";

        yield return null;

        PurchaseUse = false;
    }

    public void OnClick_MyRoom()
    {
        if (State != 3)
        {
            State = 3;
            //이전에 클릭된 버튼이 존재한다
            if (subState >= 0)
                ShopButton[subState].GetComponent<Image>().color = Color.white;
            ShopButton[State].GetComponent<Image>().color = ShopButtonSelectColor;
            subState = State;

            setScroll.totalCount = 0;
            //아이템 리스트 셋팅
            DataInfo.ins.CostumeScrollList.Clear();
            SubButton.SetActive(false);
            scrollRect.offsetMax = (new Vector2(0, -90));

            StartCoroutine(ScrollViewSetting());
        }
    }

    void ItemListReSet(int mType)
    {
        DataInfo.ins.CostumeScrollList.Clear();
        if (State == 0)
        { //내 아이템
            //아이템 리스트 셋팅
            for (int i = 0; i < DataInfo.ins.CoustumList.Length; i++)
            {
                for (int k = 0; k < DataInfo.ins.CoustumList[i].Count; k++)
                {
                    //아이템 데이터에 추천 항목 축가 하여 추천 검사
                    if (DataInfo.ins.CoustumList[i][k].State == 1)
                    {
                        if (DataInfo.ins.CoustumList[i][k].Sex == DataInfo.ins.CharacterSub.Sex
                            || DataInfo.ins.CoustumList[i][k].Sex == 2)
                        {
                            DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[i][k]);
                        }
                    }
                }
            }
        }
        if (State == 1)
        {//의상구매
            for (int i = 0; i < DataInfo.ins.CoustumList.Length; i++)
            {
                for (int k = 0; k < DataInfo.ins.CoustumList[i].Count; k++)
                {
                    //성별 검사 하여 현재 나에게 맞는 성멸의 아이템만 출력
                    if (DataInfo.ins.CoustumList[i][k].Sex == DataInfo.ins.CharacterSub.Sex
                        || DataInfo.ins.CoustumList[i][k].Sex == 2)
                    {
                        DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[i][k]);
                    }
                }
            }
        }
        
        //아이템 리스트 셋팅
        List<CoustumItemCsv> tempList = new List<CoustumItemCsv>();
        tempList.Clear();
        for (int idx = 0; idx < DataInfo.ins.CostumeScrollList.Count; idx++)
        {
            tempList.Add(DataInfo.ins.CostumeScrollList[idx]);
        }

        DataInfo.ins.CostumeScrollList.Clear();
        if (mType < 0)
        {
            for (int k = 0; k < tempList.Count; k++)
            {
                DataInfo.ins.CostumeScrollList.Add(tempList[k]);
            }
        }
        else
        {
            for (int k = 0; k < tempList.Count; k++)
            {
                if (tempList[k].Type == mType)
                {
                    DataInfo.ins.CostumeScrollList.Add(tempList[k]);
                }
            }
        }

        StartCoroutine(ScrollViewSetting());
    }

    public void OnClick_Exit()
    {

    }

    void OnClick_AllSelectButton()
    {

    }

    void OnClick_AllDeletButton()
    {

    }
}
