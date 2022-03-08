using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public Transform mHair;
    public Transform mShirt;
    public Transform mPants;
    public Transform mShoes;
    public Transform mSet;
    public Transform mAccessory;

    public string NectSceneName = "97_moveScene";

    public Canvas PlayUiCanvas;

    void Awake()
    {
        DataInfo.ins.LodingCheck = true;
    }

    private void FixedUpdate()
    {
    }

    public void OnClick_SceneChanger()
    {
        LoadingPage.LoadScene(NectSceneName);
    }

    public void itemEquipment(Info_Char _Data)
    {
        bool SetItemCheck = true;
        info_Costume temp = null;

        _Data.printData();

        //���� ������ �ʱ�ȭ
        for (int i = 0; i < mHair.childCount; i++)
        {
            mHair.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < mShirt.childCount; i++)
        {
            mShirt.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < mPants.childCount; i++)
        {
            mPants.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < mShoes.childCount; i++)
        {
            mShoes.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < mSet.childCount; i++)
        {
            mSet.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < mAccessory.childCount; i++)
        {
            mAccessory.GetChild(i).gameObject.SetActive(false);
        }

        //��� ID�� Itemã��
        for (int i = 0; i < DataInfo.ins.CoustumList[0].Count; i++)
        {
            if(DataInfo.ins.CoustumList[0][i].ItemID == _Data.Hair)
            {
                temp = DataInfo.ins.CoustumList[0][i];
                mHair.GetChild(temp.Path).gameObject.SetActive(true);
                break;
            }
        }

        for (int i = 0; i < DataInfo.ins.CoustumList[1].Count; i++)
        {
            //������ ������ ���̵� �˻��� ������� ��Ʈ ������
            SetItemCheck = true;

            if (DataInfo.ins.CoustumList[1][i].ItemID == _Data.Shirt)
            {
                SetItemCheck = false;
                temp = DataInfo.ins.CoustumList[1][i];
                mShirt.GetChild(temp.Path).gameObject.SetActive(true);
                break;
            }
        }

        if (SetItemCheck)
        {
            for (int i = 0; i < DataInfo.ins.CoustumList[4].Count; i++)
            {
                if (DataInfo.ins.CoustumList[4][i].ItemID == _Data.Shirt)
                {
                    temp = DataInfo.ins.CoustumList[4][i];
                    mSet.GetChild(temp.Path).gameObject.SetActive(true);
                    break;
                }
            }
        }

        if (!SetItemCheck)
        {//��Ʈ������ ������ÿ��� ������ �Դ´�.
            //Debug.Log("���� ���� �������� üũ");
            for (int i = 0; i < DataInfo.ins.CoustumList[2].Count; i++)
            {
                if (DataInfo.ins.CoustumList[2][i].ItemID == _Data.Pants)
                {
                    temp = DataInfo.ins.CoustumList[2][i];
                    mPants.GetChild(temp.Path).gameObject.SetActive(true);
                    //Debug.Log("���� ���̵� " + i);
                    break;
                }
            }
        }
        for (int i = 0; i < DataInfo.ins.CoustumList[3].Count; i++)
        {
            if (DataInfo.ins.CoustumList[3][i].ItemID == _Data.Shoes)
            {
                temp = DataInfo.ins.CoustumList[3][i];
                mShoes.GetChild(temp.Path).gameObject.SetActive(true);
                break;
            }
        }

        //�Ǽ����� �����.
        if (_Data.Accessory > 0)
        {
            for (int i = 0; i < DataInfo.ins.CoustumList[4].Count; i++)
            {
                if (DataInfo.ins.CoustumList[5][i].ItemID == _Data.Accessory)
                {
                    temp = DataInfo.ins.CoustumList[4][i];
                    mAccessory.GetChild(temp.Path).gameObject.SetActive(true);
                    break;
                }
            }
        }
    }
}
