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
    public Transform PickupTrans;

    public string NectSceneName = "97_moveScene";

    public bool NicNameUse = false;
    public WorldInteraction WoIn = null;
    private bool nicCheck = false;

    public ControllerManager cmLogic;

    void Awake()
    {
        DataInfo.ins.LodingCheck = true;

        if (NicNameUse)
        {
            if (WoIn == null)
            {
                WoIn = gameObject.AddComponent<WorldInteraction>();
                WoIn.nowType = InteractionType.NicName;
                WoIn.IconPos = new Vector3(0, 1.5f, 0);
                WoIn.InitWorldInteraction();
            }
        }

        cmLogic = FindObjectOfType<ControllerManager>();
    }

    private void LateUpdate()
    {
        nicOnOff();
    }

    void nicOnOff()
    {
        if (NicNameUse)
        {
            if (nicCheck != DataInfo.ins.OptionInfo.NicNameOpen)
            {
                nicCheck = DataInfo.ins.OptionInfo.NicNameOpen;
                if (nicCheck)
                {
                    if (WoIn.EventObj == null)
                    {
                        WoIn.InitWorldInteraction();
                    }
                    else
                    {
                        if (WoIn.EventObj != null)
                        {
                            Destroy(WoIn.EventObj);
                            WoIn.EventObj = null;
                        }
                        WoIn.InitWorldInteraction();
                    }
                }
                else
                {
                    if (WoIn.EventObj != null)
                    {
                        Destroy(WoIn.EventObj);
                        WoIn.EventObj = null;
                    }
                }
            }
        }
    }

    public void OnClick_SceneChanger()
    {
        cmLogic.LoadScene(NectSceneName);
    }


    public void itemEquipment(Info_Char _Data)
    {
        bool SetItemCheck = true;
        CoustumItemCsv temp = null;
        string DebugMsg = "item Equipment Check";

        _Data.printData();

        //장착 아이템 초기화
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

        //헬멧 ID로 Item찾기
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
            //셔츠의 아이템 아이디가 검색시 없을경우 세트 템으로
            SetItemCheck = true;

            if (DataInfo.ins.CoustumList[1][i].ItemID == _Data.Shirt)
            {
                SetItemCheck = false;
                temp = DataInfo.ins.CoustumList[1][i];
                mShirt.GetChild(temp.Path).gameObject.SetActive(true);
                DebugMsg += ("\nShirt Equipment Item Id[" + temp.ItemID + "]");
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
                    DebugMsg += ("\nSet Item Equipment Item Id[" + temp.ItemID + "]");
                    break;
                }
            }
        }

        if (!SetItemCheck)
        {//셋트아이템 미착용시에만 바지를 입는다.
            //Debug.Log("바지 착용 들어오는지 체크");
            for (int i = 0; i < DataInfo.ins.CoustumList[2].Count; i++)
            {
                if (DataInfo.ins.CoustumList[2][i].ItemID == _Data.Pants)
                {
                    temp = DataInfo.ins.CoustumList[2][i];
                    mPants.GetChild(temp.Path).gameObject.SetActive(true);
                    //Debug.Log("바지 아이디 " + i);
                    DebugMsg += ("\nPants Equipment Item Id[" + temp.ItemID + "]");
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
                DebugMsg += ("\nShoes Equipment Item Id[" + temp.ItemID + "]");
                break;
            }
        }

        //악세서리 착용시.
        if (_Data.Accessory > 0)
        {
            for (int i = 0; i < DataInfo.ins.CoustumList[4].Count; i++)
            {
                if (DataInfo.ins.CoustumList[5][i].ItemID == _Data.Accessory)
                {
                    temp = DataInfo.ins.CoustumList[4][i];
                    mAccessory.GetChild(temp.Path).gameObject.SetActive(true);
                    DebugMsg += ("\nAccessory Equipment Item Id[" + temp.ItemID +"]");
                    break;
                }
            }
        }
        //Debug.Log(DebugMsg);
    }

    void OnControllerColliderHit(ControllerColliderHit cchit)
    {
        if (cchit.transform.tag.Equals("GiftBox"))
        {
            Debug.Log("On Controller Collider Hit[<color=blue>" + cchit.transform.name + "</color>] Tag [<color=yellow>" + cchit.transform.tag + "</color>]");
            cmLogic.EventScripts = cchit.transform.GetComponent<WorldInteraction>();
            cmLogic.EventState = 1;
            cmLogic.RayCastEventLogic();
        }
    }
}
