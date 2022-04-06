using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcPopup : MonoBehaviour
{
    public int State = 0;
    public Transform ParentTrans;
    public Text NpcMsg;

    public WorldInteraction EventScripts;
    public GameObject Trunk;
    List<Button> TrunkButtonList = new List<Button>();

    int MsgCount = 0;
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
            EventScripts.UiCameraObj.SetActive(false);
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
        EventScripts.UiCameraObj.SetActive(true);
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
                PetNpcStateLogic();
                break;
            case InteractionType.NPC_Cafe_0:
                CafeNpcStateLogic();
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
                CreatePage("Prefabs/PetNpc/Set_0_PetNpc");
                State = 1;
                break;
            case 1:
                if (Input.GetMouseButtonDown(0))
                {
                    CreatePage("Prefabs/PetNpc/Set_2_PetNpc");
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
                    State = 2;
                }
                break;
        }
    }

    void CafeNpcStateLogic()
    {
        switch (State)
        {
            case 0:
                CreatePage("Prefabs/PetNpc/Set_0_CafeMaster");
                State = 1;
                break;
            case 1:
                if (Input.GetMouseButtonDown(0))
                {
                }
                break;
        }
    }
}
