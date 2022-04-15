using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemBuyPopup : MonoBehaviour
{
    public Text Title;
    public Text Description;
    public Text BuyText;

    public Button OkButton;
    public Button CloseButton;

    public Image ItemIcon;

    public CoustumItemCsv NowItem;

    public delegate void CallBack();

    private void OnEnable()
    {
        NowItem = DataInfo.ins.BuyItemSelect;
        if (NowItem != null)
        {
            Title.text = NowItem.Name;
            Description.text = NowItem.Description;
            BuyText.text = "구매하시는데에 " + NowItem.price + "Gold 가 소모됩니다.\n구매 하시겠습니까?";

            OkButton.onClick.RemoveAllListeners();
            CloseButton.onClick.RemoveAllListeners();

            string iconName = "CharCut/";

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
            ItemIcon.sprite = Resources.Load<Sprite>(iconName);
        }
    }

    public void ButtonSetting(CallBack Buy, CallBack Close, CallBack NoMoney)
    {
        OkButton.onClick.AddListener(() =>
        {
            Close();
            if (DataInfo.ins.CharacterMain.Money < NowItem.price)
            {
                NoMoney();
            }
            else
            {
                BuyData();
                Buy();
            }
        });
        CloseButton.onClick.AddListener(() =>
        {
            Close();
        });
    }

    void BuyData()
    {
        string savetrunk = "";

        DataInfo.ins.BuyItemId.Add(System.Convert.ToInt32(NowItem.ItemID));

        //구매된 아이템 체크
        if (NowItem.Type < 6)
        {
            for (int cnt = 0; cnt < DataInfo.ins.CoustumList[NowItem.Type].Count; cnt++)
            {
                if (DataInfo.ins.CoustumList[NowItem.Type][cnt].ItemID == NowItem.ItemID)
                {
                    DataInfo.ins.CoustumList[NowItem.Type][cnt].State = 1;
                    continue;
                }
            }
        }
        else
        {
            for (int cnt = 0; cnt < DataInfo.ins.EctItemData.Count; cnt++)
            {
                if (DataInfo.ins.EctItemData[cnt].ItemID == NowItem.ItemID)
                {
                    DataInfo.ins.EctItemData[cnt].State = 1;
                    continue;
                }
            }
        }

        //중복된 문자열 제거
        DataInfo.ins.BuyItemId = DataInfo.ins.BuyItemId.Distinct().ToList();

        for (int i = 0; i < DataInfo.ins.BuyItemId.Count; i++)
        {
            savetrunk += DataInfo.ins.BuyItemId[i] + ",";
        }
        DataInfo.ins.saveBuyItem = savetrunk;

        DataInfo.ins.AddMoney(-NowItem.price);
    }
}
