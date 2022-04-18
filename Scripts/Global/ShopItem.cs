using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    int mIndex = -1;
    int selectNumber = -1;
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

        ItemText.text = NowItem.Name;

        if(NowItem.State == 0)
        {//미구매 아이템
            ItemText.text = NowItem.price + "Gold";
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

        if(NowItem.Type == 100)
        {
            iconName = "Emotion/" + NowItem.Path + "_" + NowItem.Description;
        }

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

    private void FixedUpdate()
    {
        if (DataInfo.ins.ItemSelectIndex != NowItem.ItemID && mImage.color != Color.white)
        {
            mImage.color = Color.white;
        }
    }

    void OnClick_Evenet()
    {
        bool itemUse = false;
        DataInfo.ins.ItemSelectIndex = NowItem.ItemID;
        switch (NowItem.Type)
        {
            case 0: //머리
                if (DataInfo.ins.CharacterSub.Hair != NowItem.ItemID)
                    DataInfo.ins.CharacterSub.Hair = NowItem.ItemID;
                else
                {
                    //기본 남녀의 헤어가 다르기 때문에 분기
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
            case 1: //셔츠
            case 4: //세트
                if (DataInfo.ins.CharacterSub.Shirt != NowItem.ItemID)
                    DataInfo.ins.CharacterSub.Shirt = NowItem.ItemID;
                else
                {
                    if (NowItem.Type == 4)
                    {//원피스일때 바지 입히기
                        DataInfo.ins.CharacterSub.Pants = 15;
                    }
                    DataInfo.ins.CharacterSub.Shirt = 10;
                }
                break;
            case 2: //바지
                if (DataInfo.ins.CharacterSub.Pants != NowItem.ItemID)
                    DataInfo.ins.CharacterSub.Pants = NowItem.ItemID;
                else
                    DataInfo.ins.CharacterSub.Pants = 15;
                break;
            case 3: //신발
                if (DataInfo.ins.CharacterSub.Shoes != NowItem.ItemID)
                    DataInfo.ins.CharacterSub.Shoes = NowItem.ItemID;
                else
                    DataInfo.ins.CharacterSub.Shoes = 19;
                break;
            case 5: // 악세사리
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
                            //저장 되었던 코스튬의 데이터 제거
                            DataInfo.ins.BuyItemSaveList.RemoveAt(i);
                            itemUse = true;
                            break;
                        }
                    }

                    if (!itemUse)
                    {//저장 되어 있지 않으면 저장
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

    void ItemSelectCheck()
    {
        mImage.color = Color.white;
        for (int i = 0; i < DataInfo.ins.BuyItemSaveList.Count; i++)
        {
            if (DataInfo.ins.BuyItemSaveList[i].ItemID == NowItem.ItemID)
            {
                mImage.color = new Color(0.7f, 0.9f, 1f);
            }
        }
    }
}

