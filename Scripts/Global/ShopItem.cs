using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    int mIndex = -1;
    public Text ItemText;
    public info_Costume NowItem = new info_Costume();
    Image mImage;
    Button NowButton;

    public void ScrollCellIndex(int idx)
    {
        mIndex = idx;
        mImage = GetComponent<Image>();
        NowButton = GetComponent<Button>();

        NowItem = DataInfo.ins.CostumeScrollList[idx];

        ItemText.text = idx + NowItem.Description;

        if(NowItem.State == 0)
        {//미구매 아이템
            ItemText.text += ("\n" + NowItem.price);
        }

        NowButton.onClick.RemoveAllListeners();
        NowButton.onClick.AddListener(OnClick_Evenet);
    }

    void OnEnable()
    {
        for (int i = 0; i < DataInfo.ins.BuyItemId.Count; i++)
        {
            if(NowItem.ItemID == DataInfo.ins.BuyItemId[i])
            {
                NowItem.State = 1;
            }
        }
    }

    void OnClick_Evenet()
    {
        bool itemUse = false;
        switch (NowItem.Type)
        {
            case 0: //머리
                DataInfo.ins.CharacterSub.Hair = NowItem.ItemID;
                break;
            case 1: //셔츠
            case 4: //세트
                DataInfo.ins.CharacterSub.Shirt = NowItem.ItemID;
                break;
            case 2: //바지
                DataInfo.ins.CharacterSub.Pants = NowItem.ItemID;
                break;
            case 3: //신발
                DataInfo.ins.CharacterSub.Shoes = NowItem.ItemID;
                break;
            case 5: // 악세사리
                DataInfo.ins.CharacterSub.Accessory = NowItem.ItemID;
                break;
            case 100:
                {
                    DataInfo.ins.SubCharAnimator.SetInteger("Emotion", NowItem.Path);
                    itemUse = false;

                    for (int i = 0; i < DataInfo.ins.BuyItemSaveList.Count; i++)
                    {
                        //
                        if (NowItem.ItemID == DataInfo.ins.BuyItemSaveList[i].ItemID) {
                            //저장 되었던 코스튬의 데이터 제거
                            DataInfo.ins.BuyItemSaveList.RemoveAt(i);
                            itemUse = true;
                            break;
                        }
                    }

                    if (!itemUse)
                    {//저장 되어 있지 않으면 저장
                        info_Costume temp = NowItem;
                        temp.inGameUse = 1;
                        DataInfo.ins.BuyItemSaveList.Add(NowItem);
                    }
                }
                break;
        }
        DataInfo.ins.CharacterChangeCheck = true;
    }

    //PurchaseItemSetting();
}
