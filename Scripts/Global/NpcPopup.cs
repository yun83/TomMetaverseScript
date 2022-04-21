using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcPopup : MonoBehaviour
{
    public int State = 0;
    public Transform ParentTrans;
    public GameObject NpcButtonObj;

    public WorldInteraction EventScripts;
    public GameObject Trunk;
    List<Button> TrunkButtonList = new List<Button>();

    int MsgCount = 0;
    bool EctClickCheck = false;
    /// <summary>
    /// 0-갯풀, 1-카페
    /// </summary>

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDisable()
    {
        if(DataInfo.ins != null)
            DataInfo.ins.CallNpc = false;

        if (EventScripts != null)
        {
            EventScripts.OnOffObject.SetActive(false);
        }
        TrunkButtonList.Clear();
        if (Trunk != null)
        {
            Destroy(Trunk);
        }
    }

    public void InitNpc(WorldInteraction wi) {
        EventScripts = wi;
        MsgCount = 0;
        State = 0;
        DataInfo.ins.CallNpc = true;
        EventScripts.OnOffObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        StateLogic();
    }

    void StateLogic()
    {
        switch (EventScripts.nowType)
        {
            case InteractionType.NPC_PetMaster:
            case InteractionType.getPoolPotal:
                PetNpcStateLogic();
                break;
            case InteractionType.Cafe_In:
            case InteractionType.NPC_Cafe_0:
                CafeNpc_0_StateLogic();
                break;
            case InteractionType.NPC_Cafe_1:
                CafeNpc_1_StateLogic();
                break;
        }
    }

    void CreatePage(string resPath)
    {
        if(Trunk != null)
        {
            Destroy(Trunk);
            Trunk = null;
        }

        Trunk = Instantiate(Resources.Load<GameObject>(resPath));

        RectTransform rt = Trunk.GetComponent<RectTransform>();
        rt.parent = ParentTrans;
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;

        Button[] temp = rt.GetComponentsInChildren<Button>();


        //for (int i = 0; i < BCount; i++)
        //{
        //    GameObject tempO = Instantiate(NpcButtonObj);
        //    tempO.transform.parent = Trunk.transform;

        //    NpcButton tempN = tempO.GetComponent<NpcButton>();
        //}

        TrunkButtonList.Clear();
        for (int i = 0; i < temp.Length; i++)
        {
            TrunkButtonList.Add(temp[i]);
        }
    }

    void PetNpcStateLogic()
    {
        switch (State)
        {
            case 0:
                CreatePage("Prefabs/PetNpc/PetNpc0_0");
                TrunkButtonList[0].onClick.RemoveAllListeners();
                TrunkButtonList[0].onClick.AddListener(() =>
                {
                    Application.OpenURL("https://tomntoms.notion.site/tomntoms/GETPOOL-7bb3724306a24551aaaf3d26358e6dd0");
                }
                );
                TrunkButtonList[1].onClick.RemoveAllListeners();
                TrunkButtonList[1].onClick.AddListener(() => { 
                    State = 2;
                    EctClickCheck = false;
                });
                EctClickCheck = false;
                State = 1;
                break;
            case 1:
                if(EctClickCheck)
                {
                    EctClickCheck = false;
                    //State = 2;
                }
                break;
            case 2:
                CreatePage("Prefabs/PetNpc/PetNpc0_1");
                for (int i = 0; i < TrunkButtonList.Count; i++)
                {
                    int idx = i;
                    TrunkButtonList[idx].onClick.RemoveAllListeners();
                    TrunkButtonList[idx].onClick.AddListener(() =>
                    {
                        DataInfo.ins.CharacterMain.PetID = idx;
                        DataInfo.ins.SaveData = JsonUtility.ToJson(DataInfo.ins.CharacterMain);
                        DataInfo.ins.infoController.CreatePetObject();
                        gameObject.SetActive(false);
                    }
                    );
                }
                State = 3;
                break;
        }
    }

    void CafeNpc_0_StateLogic()
    {
        switch (State)
        {
            case 0:
                CreatePage("Prefabs/PetNpc/CafeNpc0_0");
                TrunkButtonList[0].onClick.RemoveAllListeners();
                TrunkButtonList[0].onClick.AddListener(() =>
                {
                    Application.OpenURL("https://www.tomntoms.com");
                }
                );
                TrunkButtonList[1].onClick.RemoveAllListeners();
                TrunkButtonList[1].onClick.AddListener(() => {

                    //DataInfo.ins.OutRoomButton.Clear();

                    //ButtonClass item1 = new ButtonClass();
                    //item1.text = "CoffeeShop";
                    //item1.addEvent = (() => {
                    //    DataInfo.ins.infoController.LoadScene("CoffeeShop");
                    //});
                    //DataInfo.ins.OutRoomButton.Add(item1);

                    //DataInfo.ins.GameUI.OR_Popup.Title.text = "카페 들어가기";
                    //DataInfo.ins.GameUI.OnClick_OutRoomPopup(DataInfo.ins.OutRoomButton);

                    DataInfo.ins.infoController.LoadScene("CoffeeShop");

                });
                EctClickCheck = false;
                State = 1;
                break;
            case 1:
                break;
        }
    }

    void CafeNpc_1_StateLogic()
    {
        switch (State)
        {
            case 0:
                CreatePage("Prefabs/PetNpc/CafeNpc1_0");
                for (int i = 0; i < TrunkButtonList.Count; i++)
                {
                    int idx = i;
                    TrunkButtonList[idx].onClick.RemoveAllListeners();
                }
                TrunkButtonList[0].onClick.AddListener(() =>
                {
                    State = 2;
                });
                State = 1;
                break;
            case 1:
                break;
            case 2:
                CreatePage("Prefabs/PetNpc/CafeNpc1_1");
                for (int i = 0; i < TrunkButtonList.Count; i++)
                {
                    int idx = i;
                    TrunkButtonList[idx].onClick.RemoveAllListeners();
                }
                TrunkButtonList[0].onClick.AddListener(() =>
                {
                    Npc_Cafe1_ButtonSetting("CoffeeCup_A", 0.834f, 0);
                });
                TrunkButtonList[1].onClick.AddListener(() =>
                {
                    Npc_Cafe1_ButtonSetting("CoffeeLatte_A", 0.792f, 1);
                });
                TrunkButtonList[2].onClick.AddListener(() =>
                {
                    Npc_Cafe1_ButtonSetting("TeaCup_A", 0.791f, 2);
                });
                TrunkButtonList[3].onClick.AddListener(() =>
                {
                    Npc_Cafe1_ButtonSetting("Slushie", 0.873f, 3);
                });
                State = 3;
                break;
        }
    }
    void Npc_Cafe1_ButtonSetting(string adr, float posy, int itemId)
    {
        string _path = "Prefabs/PickUpItem/";

        GameObject TempObj = Instantiate(Resources.Load<GameObject>(_path + "PickUpHand"));
        GameObject SubObj = Instantiate(Resources.Load<GameObject>(_path + adr));

        TempObj.transform.localPosition = new Vector3(4.5f, 0.76f, -1.6f);
        SubObj.transform.localPosition = new Vector3(4.5f, posy, -1.6f);
        SubObj.transform.parent = TempObj.transform;

        WorldInteraction wiData = TempObj.GetComponent<WorldInteraction>();
        wiData.ItemId = itemId;
        wiData.PickUpObj = SubObj;

        DataInfo.ins.WinQuest(9);
        gameObject.SetActive(false);
    }

    public void OnClick_EctClick()
    {
        EctClickCheck = true;
    }
}
