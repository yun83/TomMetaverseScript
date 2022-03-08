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
        {//�̱��� ������
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
        if (mIndex >= 0)
        {
            switch (NowItem.Type)
            {
                case 0: //�Ӹ�
                    DataInfo.ins.CharacterSub.Hair = NowItem.ItemID;
                    break;
                case 1: //����
                case 4: //��Ʈ
                    DataInfo.ins.CharacterSub.Shirt = NowItem.ItemID;
                    break;
                case 2: //����
                    DataInfo.ins.CharacterSub.Pants = NowItem.ItemID;
                    break;
                case 3: //�Ź�
                    DataInfo.ins.CharacterSub.Shoes = NowItem.ItemID;
                    break;
                case 5: // �Ǽ��縮
                    DataInfo.ins.CharacterSub.Accessory = NowItem.ItemID;
                    break;
            }
            DataInfo.ins.CharacterChangeCheck = true;
        }
    }

    //PurchaseItemSetting();
}
