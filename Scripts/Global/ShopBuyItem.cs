using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyItem : MonoBehaviour
{
    public Image IconImage;

    public Text Title;
    public Text Description;
    public Text Price;

    public GameObject CheckBox;
    public Button button;
    public Button DeletButton;
    public ShopPopup spScript;

    int Index = -1;
    public string nowID;
    public CoustumItemCsv NowItem = new CoustumItemCsv();

    void Awake()
    {
        spScript = DataInfo.ins.GameUI.ShopPopup.GetComponent<ShopPopup>();
        button = GetComponent<Button>();
        CheckBox.SetActive(true);
    }

    public void ScrollCellIndex(int idx)
    {
        string iconName = "CharCut/";
        Index = idx;

        NowItem = DataInfo.ins.BuyItemSaveList[idx];

        Title.text = NowItem.Name;
        Description.text = NowItem.Description;
        Price.text = NowItem.price.ToString();

        switch (NowItem.Type)
        {
            case 0: iconName += "Hair_"; break;
            case 1: iconName += "Shirt_"; break;
            case 2: iconName += "Pants_"; break;
            case 3: iconName += "Shoes_"; break;
            case 4: iconName += "Set_"; break;
            case 5: iconName += "Accessory_"; break;
        }
        iconName += (NowItem.Sex + "_" + NowItem.Path);

        if (NowItem.Type == 100)
        {
            iconName = "Emotion/" + NowItem.Path + "_" + NowItem.Description;
        }
        IconImage.sprite = Resources.Load<Sprite>(iconName);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick_Evenet);

        DeletButton.onClick.RemoveAllListeners();
        DeletButton.onClick.AddListener(OnClick_DeletButton);
    }

    void OnClick_Evenet()
    {
        if (DataInfo.ins.BuyItemSaveList[Index].inGameUse== 1)
        {
            DataInfo.ins.BuyItemSaveList[Index].inGameUse = 0;
            CheckBox.SetActive(false);
        }
        else
        {
            DataInfo.ins.BuyItemSaveList[Index].inGameUse = 1;
            CheckBox.SetActive(true);
        }
        DataInfo.ins.TotlaMoneySumCheck = true;
    }

    void OnClick_DeletButton()
    {
        if (DataInfo.ins.BuyItemSaveList.Count <= 1)
            DataInfo.ins.BuyItemSaveList.Clear();
        else
            DataInfo.ins.BuyItemSaveList.RemoveAt(Index);

        DataInfo.ins.TotlaMoneySumCheck = true;

        spScript.OnClick_Purchase();
    }
}
