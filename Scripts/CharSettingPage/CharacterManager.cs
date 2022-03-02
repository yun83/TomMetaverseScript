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
    Player3DUi mUi;
    bool NicShowCheck = false;

    void Awake()
    {
        if (DataInfo.ins.LodingCheck == false)
        {
            //�ε��� ���� ���� ��쿡 ��� �ε� �Ǵ� �� ����
            //���� �α��� �κп��� ������ �ε����� ����

            #region Costum Item Data Loding
            List<Dictionary<string, object>> data = CSVReader.Read("Doc/CostumItem");

            //���� ������ Ÿ���� 6����
            for (int i = 0; i < DataInfo.ins.CoustumList.Length; i++)
            {
                DataInfo.ins.CoustumList[i] = new List<info_Costume>();
                DataInfo.ins.CoustumList[i].Clear();
            }

            for (var i = 0; i < data.Count; i++)
            {
                //print("ItemID " + data[i]["ItemID"] + " " + "Type " + data[i]["Type"] + " " + "State " + data[i]["State"] + " " + "Name " + data[i]["Name"] + " " +  "resourcesPath " + data[i]["resourcesPath"] );
                info_Costume temp = new info_Costume();

                temp.ItemID = System.Convert.ToInt32(data[i]["ItemID"]);
                temp.Type = System.Convert.ToInt32(data[i]["Type"]);
                temp.State = System.Convert.ToInt32(data[i]["State"]);
                temp.Sex = System.Convert.ToInt32(data[i]["Sex"]);
                temp.Name = System.Convert.ToString(data[i]["Name"]);
                temp.Description = System.Convert.ToString(data[i]["Description"]);
                temp.price = System.Convert.ToInt32(data[i]["price"]);
                temp.Path = System.Convert.ToInt32(data[i]["Path"]);

                DataInfo.ins.CoustumList[temp.Type].Add(temp);
            }
            #endregion

            #region Save Loding
            if (!DataInfo.ins.SaveData.Equals("") && DataInfo.ins.SaveData != null)
            {
                //Debug.Log(" ------------- ���̺� ������ �����Ұ�� �ε� --------------");
                DataInfo.ins.CharacterMain = JsonUtility.FromJson<Info_Char>(DataInfo.ins.SaveData);
                //Debug.Log(" -------------- Save File Data Show " + DataInfo.ins.CharacterMain.printData());
            }

            if (!DataInfo.ins.saveOption.Equals("") && DataInfo.ins.saveOption != null)
            {
                DataInfo.ins.OptionInfo = JsonUtility.FromJson<OptionData>(DataInfo.ins.saveOption);
            }
            #endregion
        }

        PlayUiCanvas = transform.GetComponentInChildren<Canvas>();
        if (PlayUiCanvas != null)
        {
            mUi = (Player3DUi)GetComponentInChildren<Player3DUi>();
            mUi.NicName.text = DataInfo.ins.CharacterMain.NicName;
        }


        DataInfo.ins.LodingCheck = true;
    }

    private void FixedUpdate()
    {
        NicNameShow();
    }

    void NicNameShow()
    {
        if (PlayUiCanvas != null)
        {
            if (NicShowCheck != DataInfo.ins.OptionInfo.NicNameOpen)
            {
                NicShowCheck = DataInfo.ins.OptionInfo.NicNameOpen;
                if (NicShowCheck)
                    mUi.NicName.gameObject.SetActive(true);
                else
                    mUi.NicName.gameObject.SetActive(false);
            }
        }
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
