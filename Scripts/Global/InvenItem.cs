using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenItem : MonoBehaviour
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

        if (NowItem.State == 0)
            ItemText.text = NowItem.price.ToString() + "Gold";
        else
            ItemText.text = "";

        NowButton.onClick.RemoveAllListeners();
        NowButton.onClick.AddListener(OnClick_Evenet);
    }

    public void OnClick_Evenet()
    {
        if (mIndex >= 0)
        {
            if (mIndex != DataInfo.ins.ItemSelectIndex)
            {
                DataInfo.ins.ItemSelectIndex = mIndex;
                switch (DataInfo.ins.InvenNumber)
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
                }
                AddBuyCostumes(NowItem.ItemID);

            }
            else
            {
                DataInfo.ins.ItemSelectIndex = -1;
                //같은 번호 클릭시에 아이템 해제
                switch (DataInfo.ins.InvenNumber)
                {
                    case 0: //머리
                        if (DataInfo.ins.CharacterSub.Hair != DataInfo.ins.CharacterMain.Hair)
                        {
                            DataInfo.ins.CharacterSub.Hair = DataInfo.ins.CharacterMain.Hair;
                        }
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
                        if (DataInfo.ins.CharacterSub.Shirt != DataInfo.ins.CharacterMain.Shirt)
                        {
                            DataInfo.ins.CharacterSub.Shirt = DataInfo.ins.CharacterMain.Shirt;
                        }
                        else
                        {
                            DataInfo.ins.CharacterSub.Shirt = 10;
                        }
                        break;
                    case 2: //바지
                        if (DataInfo.ins.CharacterSub.Pants != DataInfo.ins.CharacterMain.Pants)
                        {
                            DataInfo.ins.CharacterSub.Pants = DataInfo.ins.CharacterMain.Pants;
                        }
                        else
                        {
                            DataInfo.ins.CharacterSub.Pants = 15;
                        }
                        break;
                    case 3: //신발
                        if (DataInfo.ins.CharacterSub.Shoes != DataInfo.ins.CharacterMain.Shoes)
                        {
                            DataInfo.ins.CharacterSub.Shoes = DataInfo.ins.CharacterMain.Shoes;
                        }
                        else
                        {
                            DataInfo.ins.CharacterSub.Shoes = 19;
                        }
                        break;
                    case 5: // 악세사리
                        if (DataInfo.ins.CharacterSub.Accessory != DataInfo.ins.CharacterMain.Accessory)
                        {
                            DataInfo.ins.CharacterSub.Accessory = DataInfo.ins.CharacterMain.Accessory;
                        }
                        else
                        {
                            DataInfo.ins.CharacterSub.Accessory = -1;
                        }
                        break;
                }
                RemoveBuycostumes(NowItem.ItemID);
            }
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
                        if (temp.Type == DataInfo.ins.BuyItemSaveList[i].Type)
                        {
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

    void RemoveBuycostumes(int itemId)
    {
        CoustumItemCsv temp = new CoustumItemCsv();
        temp = DataInfo.ins.getItemData(itemId);

        if (DataInfo.ins.BuyItemSaveList.Count > 0)
        {
            for (int i = 0; i < DataInfo.ins.BuyItemSaveList.Count; i++)
            {
                //저장 되었던 코스튬의 데이터 제거
                if(DataInfo.ins.BuyItemSaveList[i].ItemID == itemId)
                {
                    DataInfo.ins.BuyItemSaveList.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
