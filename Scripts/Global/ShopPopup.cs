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
    public Text MoneyText;

    private Vector2 nowPos, prePos;
    private Vector2 movePosDiff;

    public Button[] ShopButton;
    [Header("���� �˾�")]
    public Button Purchase;
    Text PurchaseText;
    public GameObject BuyPopup;
    public InitScroll BuyPopupInitScroll;
    public Text TotlaPrice;
    public Button PurchaseClose;
    public Button PurchaseBuy;

    int State = -1;
    int subState = -1;
    int sumMoney = 0;

    // Start is called before the first frame update
    void Start()
    {
        ShopButton[0].onClick.RemoveAllListeners();
        ShopButton[0].onClick.AddListener(OnClick_Suggestion);
        ShopButton[1].onClick.RemoveAllListeners();
        ShopButton[1].onClick.AddListener(OnClick_Item);
        ShopButton[2].onClick.RemoveAllListeners();
        ShopButton[2].onClick.AddListener(OnClick_Gesture);

        Purchase.onClick.RemoveAllListeners();
        Purchase.onClick.AddListener(OnClick_Purchase);

        PurchaseClose.onClick.RemoveAllListeners();
        PurchaseClose.onClick.AddListener(() =>
        {
            BuyPopup.SetActive(false);
        });

        PurchaseBuy.onClick.RemoveAllListeners();
        PurchaseBuy.onClick.AddListener(OnClick_purchaseBuy);
    }

    private void OnEnable()
    {
        if (!DataInfo.ins.SaveData.Equals("") && DataInfo.ins.SaveData != null)
        {
            Debug.Log(" ------------- Shop Popup Character Loding ------------- ");
            DataInfo.ins.CharacterSub = JsonUtility.FromJson<Info_Char>(DataInfo.ins.SaveData);
        }

        NicName.text = DataInfo.ins.CharacterSub.NicName;

        ShopCharacter.itemEquipment(DataInfo.ins.CharacterSub);

        MoneyText.text = DataInfo.ins.CharacterMain.Money.ToString();

        DataInfo.ins.BuyItemSaveList.Clear();
        PurchaseText = Purchase.GetComponentInChildren<Text>();
        Purchase.enabled = false;
        PurchaseText.text = "Purchase";

        BuyPopup.SetActive(false);
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
            Purchase.enabled = true;
            PurchaseText.text = "Purcharse [" + DataInfo.ins.BuyItemSaveList.Count + "]";
        }
    }

    void AddBuyCostumes(int itemId)
    {
        info_Costume temp = new info_Costume();
        temp = DataInfo.ins.getItemData(itemId);
        if (temp != null)
        {
            //�������� ���̵� �˻� �Ǹ� �ش� �������� ���� ������ ���������� Ȯ��
            if (temp.State == 0)
            {
                //Costumes.Add(temp);
                //����� ���� ���� �ڽ�Ƭ�� ���� ���
                if (DataInfo.ins.BuyItemSaveList.Count > 0)
                {
                    for (int i = 0; i < DataInfo.ins.BuyItemSaveList.Count; i++)
                    {
                        //�ѹ��� ���� ������ Ÿ���� 1���� ���� ���Ǵ� ����� ���� �Ѱ��� ���·�
                        if (temp.Type == DataInfo.ins.BuyItemSaveList[i].Type) {
                            //���� �Ǿ��� �ڽ�Ƭ�� ������ ����
                            DataInfo.ins.BuyItemSaveList.RemoveAt(i);
                            break;
                        }
                    }
                }

                temp.inGameUse = 1;
                //���� �Ϸ��� �����ۿ� ����
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
            //������ Ŭ���� ��ư�� �����Ѵ�
            if (subState >= 0)
                ShopButton[subState].GetComponent<Image>().color = Color.white;
            ShopButton[State].GetComponent<Image>().color = Color.yellow;
            subState = State;

            setScroll.totalCount = 0;
            //������ ����Ʈ ����
            DataInfo.ins.CostumeScrollList.Clear();
            for (int i = 0; i < DataInfo.ins.CoustumList.Length; i++)
            {
                for (int k = 0; k < DataInfo.ins.CoustumList[i].Count; k++)
                {
                    //������ �����Ϳ� ��õ �׸� �డ �Ͽ� ��õ �˻�
                    if (DataInfo.ins.CoustumList[i][k].Suggestion == 1)
                    {
                        if (DataInfo.ins.CoustumList[i][k].Sex == DataInfo.ins.CharacterSub.Sex
                            || DataInfo.ins.CoustumList[i][k].Sex == 2)
                        {
                            DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[i][k]);
                        }
                    }
                }
            }

            StartCoroutine(ScrollViewSetting());
        }
    }

    public void OnClick_Item()
    {
        if (State != 1)
        {
            State = 1;
            //������ Ŭ���� ��ư�� �����Ѵ�
            if (subState >= 0)
                ShopButton[subState].GetComponent<Image>().color = Color.white;
            ShopButton[State].GetComponent<Image>().color = Color.yellow;
            subState = State;

            setScroll.totalCount = 0;
            //������ ����Ʈ ����
            DataInfo.ins.CostumeScrollList.Clear();
            for (int i = 0; i < DataInfo.ins.CoustumList.Length; i++)
            {
                for (int k = 0; k < DataInfo.ins.CoustumList[i].Count; k++)
                {
                    //���� �˻� �Ͽ� ���� ������ �´� ������ �����۸� ���
                    if (DataInfo.ins.CoustumList[i][k].Sex == DataInfo.ins.CharacterSub.Sex
                        || DataInfo.ins.CoustumList[i][k].Sex == 2)
                        DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[i][k]);
                }
            }

            StartCoroutine(ScrollViewSetting());
        }
    }

    public void OnClick_Gesture()
    {
        if (State != 2)
        {
            State = 2;
            //������ Ŭ���� ��ư�� �����Ѵ�
            if (subState >= 0)
                ShopButton[subState].GetComponent<Image>().color = Color.white;
            ShopButton[State].GetComponent<Image>().color = Color.yellow;
            subState = State;

            setScroll.totalCount = 0;
            //������ ����Ʈ ����
            DataInfo.ins.CostumeScrollList.Clear();
            for (int i = 0; i < DataInfo.ins.CoustumList.Length; i++)
            {
                for (int k = 0; k < DataInfo.ins.CoustumList[i].Count; k++)
                {
                    //������ �����Ϳ� ��õ �׸� �డ �Ͽ� ��õ �˻�
                    if (DataInfo.ins.CoustumList[i][k].Type == 100)
                    {
                        if (DataInfo.ins.CoustumList[i][k].Sex == DataInfo.ins.CharacterSub.Sex
                            || DataInfo.ins.CoustumList[i][k].Sex == 2)
                        {
                            DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[i][k]);
                        }
                    }
                }
            }

            StartCoroutine(ScrollViewSetting());
        }
    }

    void OnClick_Purchase()
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

    void OnClick_purchaseBuy()
    {
        //����ó��
        string savetrunk = "";

        for (int i = 0; i < DataInfo.ins.BuyItemSaveList.Count; i++)
        {
            //DataInfo.ins.BuyItemSaveList �� �������� ���� �Ѵ�
            info_Costume itemTrunk = DataInfo.ins.BuyItemSaveList[i];

            DataInfo.ins.BuyItemId.Add(System.Convert.ToInt32(itemTrunk.ItemID));

            //���ŵ� ������ üũ
            if (itemTrunk.Type < 6)
            {
                for (int j = 0; j < DataInfo.ins.CoustumList[itemTrunk.Type].Count; j++)
                {
                    if (DataInfo.ins.CoustumList[itemTrunk.Type][i].ItemID == itemTrunk.ItemID)
                    {
                        DataInfo.ins.CoustumList[itemTrunk.Type][i].State = 1;
                        continue;
                    }
                }
            }
            else
            {
                for (int j = 0; j < DataInfo.ins.EctItemData.Count; j++)
                {
                    if (DataInfo.ins.EctItemData[i].ItemID == itemTrunk.ItemID)
                    {
                        DataInfo.ins.EctItemData[i].State = 1;
                        continue;
                    }
                }
            }
        }

        DataInfo.ins.BuyItemId = DataInfo.ins.BuyItemId.Distinct().ToList();

        for (int i = 0; i < DataInfo.ins.BuyItemId.Count; i++)
        {
            savetrunk += DataInfo.ins.BuyItemId[i] + ",";
        }
        DataInfo.ins.saveBuyItem = savetrunk;
        Debug.Log(savetrunk);
        BuyPopup.SetActive(false);
        setScroll.gameObject.SetActive(false);

        if (DataInfo.ins.CharacterMain.Money >= sumMoney)
            DataInfo.ins.CharacterMain.Money -= sumMoney;


        MoneyText.text = DataInfo.ins.CharacterMain.Money.ToString();

        switch (State)
        {
            case 1:
                OnClick_Item();
                break;
            case 2:
                OnClick_Gesture();
                break;
            default:
                OnClick_Suggestion();
                break;
        }
    }
}
