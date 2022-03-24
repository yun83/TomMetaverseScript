using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenItem : MonoBehaviour
{
    int mIndex = -1;
    public Text ItemText;
    public CoustumItemCsv NowItem = new CoustumItemCsv();
    Image mImage;
    Button NowButton;

    public void ScrollCellIndex(int idx)
    {
        mIndex = idx;
        mImage = GetComponent<Image>();
        NowButton = GetComponent<Button>();

        //if (idx >= DataInfo.ins.TempItem.Length)
        //    return;

        //mImage.color = DataInfo.ins.TempItem[idx];
        NowItem = //DataInfo.ins.CoustumList[DataInfo.ins.InvenNumber][idx];
            DataInfo.ins.CostumeScrollList[idx];

        ItemText.text = idx + NowItem.Description;

        NowButton.onClick.RemoveAllListeners();
        NowButton.onClick.AddListener(OnClick_Evenet);
    }

    public void OnClick_Evenet()
    {
        if (mIndex >= 0)
        {
            //Debug.Log("<color=cyan> �κ� Click Evnet </color> Inven Type [ " + DataInfo.ins.InvenNumber + " ] :: ItemSelectIndex [ " + DataInfo.ins.ItemSelectIndex + " ]");

            if (mIndex != DataInfo.ins.ItemSelectIndex)
            {
                DataInfo.ins.ItemSelectIndex = mIndex;
                switch (DataInfo.ins.InvenNumber)
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
            }
            else
            {
                DataInfo.ins.ItemSelectIndex = -1;
                //���� ��ȣ Ŭ���ÿ� ������ ����
                switch (DataInfo.ins.InvenNumber)
                {
                    case 0: //�Ӹ�
                        if (DataInfo.ins.CharacterSub.Hair != DataInfo.ins.CharacterMain.Hair)
                        {
                            DataInfo.ins.CharacterSub.Hair = DataInfo.ins.CharacterMain.Hair;
                        }
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
                        if (DataInfo.ins.CharacterSub.Shirt != DataInfo.ins.CharacterMain.Shirt)
                        {
                            DataInfo.ins.CharacterSub.Shirt = DataInfo.ins.CharacterMain.Shirt;
                        }
                        else
                        {
                            DataInfo.ins.CharacterSub.Shirt = 10;
                        }
                        break;
                    case 2: //����
                        if (DataInfo.ins.CharacterSub.Pants != DataInfo.ins.CharacterMain.Pants)
                        {
                            DataInfo.ins.CharacterSub.Pants = DataInfo.ins.CharacterMain.Pants;
                        }
                        else
                        {
                            DataInfo.ins.CharacterSub.Pants = 15;
                        }
                        break;
                    case 3: //�Ź�
                        if (DataInfo.ins.CharacterSub.Shoes != DataInfo.ins.CharacterMain.Shoes)
                        {
                            DataInfo.ins.CharacterSub.Shoes = DataInfo.ins.CharacterMain.Shoes;
                        }
                        else
                        {
                            DataInfo.ins.CharacterSub.Shoes = 19;
                        }
                        break;
                    case 5: // �Ǽ��縮
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
            }
        }
    }
}
