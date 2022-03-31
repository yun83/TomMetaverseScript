using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    int mIndex = -1;
    public Text ItemText;
    public Image ItemIcon;
    public CoustumItemCsv NowItem = new CoustumItemCsv();
    Image mImage;
    Button NowButton;

    public void ScrollCellIndex(int idx)
    {
        string iconName = "CharCut/";

        mIndex = idx;
        mImage = GetComponent<Image>();
        NowButton = GetComponent<Button>();

        NowItem = DataInfo.ins.CostumeScrollList[idx];

        ItemText.text = idx + NowItem.Description;

        if(NowItem.State == 0)
        {//�̱��� ������
            ItemText.text += ("\n" + NowItem.price + "Gold");
        }
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

        ItemIcon.sprite = Resources.Load<Sprite>(iconName);

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
            case 0: //�Ӹ�
                if (DataInfo.ins.CharacterSub.Hair != NowItem.ItemID)
                    DataInfo.ins.CharacterSub.Hair = NowItem.ItemID;
                else
                {
                    //�⺻ ������ �� �ٸ��� ������ �б�
                    if (DataInfo.ins.CharacterSub.Sex == 1)
                    {
                        DataInfo.ins.CharacterSub.Hair = 5;
                    }
                    else
                    {
                        DataInfo.ins.CharacterSub.Hair = 0;
                    }
                }
                break;
            case 1: //����
            case 4: //��Ʈ
                if (DataInfo.ins.CharacterSub.Shirt != NowItem.ItemID)
                    DataInfo.ins.CharacterSub.Shirt = NowItem.ItemID;
                else
                {
                    if (NowItem.Type == 4)
                    {//���ǽ��϶� ���� ������
                        DataInfo.ins.CharacterSub.Pants = 15;
                    }
                    DataInfo.ins.CharacterSub.Shirt = 10;
                }
                break;
            case 2: //����
                if (DataInfo.ins.CharacterSub.Pants != NowItem.ItemID)
                    DataInfo.ins.CharacterSub.Pants = NowItem.ItemID;
                else
                    DataInfo.ins.CharacterSub.Pants = 15;
                break;
            case 3: //�Ź�
                if (DataInfo.ins.CharacterSub.Shoes != NowItem.ItemID)
                    DataInfo.ins.CharacterSub.Shoes = NowItem.ItemID;
                else
                    DataInfo.ins.CharacterSub.Shoes = 19;
                break;
            case 5: // �Ǽ��縮
                if (DataInfo.ins.CharacterSub.Accessory != NowItem.ItemID)
                    DataInfo.ins.CharacterSub.Accessory = NowItem.ItemID;
                else
                    DataInfo.ins.CharacterSub.Accessory = -1;
                break;
            case 100:
                {
                    //DataInfo.ins.SubCharAnimator.SetInteger("Emotion", NowItem.Path);

                    if(DataInfo.ins.CharacterSub.Sex == 1)
                        Com.ins.AniSetInt(DataInfo.ins.SubCharAnimator, "Emotion", NowItem.Path + 100);
                    else
                        Com.ins.AniSetInt(DataInfo.ins.SubCharAnimator, "Emotion", NowItem.Path);

                    itemUse = false;

                    for (int i = 0; i < DataInfo.ins.BuyItemSaveList.Count; i++)
                    {
                        //
                        if (NowItem.ItemID == DataInfo.ins.BuyItemSaveList[i].ItemID) {
                            //���� �Ǿ��� �ڽ�Ƭ�� ������ ����
                            DataInfo.ins.BuyItemSaveList.RemoveAt(i);
                            itemUse = true;
                            break;
                        }
                    }

                    if (!itemUse)
                    {//���� �Ǿ� ���� ������ ����
                        CoustumItemCsv temp = NowItem;
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
