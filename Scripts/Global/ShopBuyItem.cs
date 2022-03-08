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

    int Index = -1;
    public string nowID;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void ScrollCellIndex(int idx)
    {
        Index = idx;

        Title.text = DataInfo.ins.BuyItemSaveList[idx].Name;
        Description.text = DataInfo.ins.BuyItemSaveList[idx].Description;
        Price.text = DataInfo.ins.BuyItemSaveList[idx].price.ToString();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick_Evenet);
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
}
