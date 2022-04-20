using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldInteraction : MonoBehaviour
{
    public InteractionType nowType = InteractionType.OutRoom;
    [Header("배치관련")]
    public Vector3 IconPos;

    [Tooltip("명확한 좌표 설정을 위해서 따로 명시해준다")]
    public Vector3 PlayerPos;
    public Vector3 PlayerRotation;

    [Header("Type Ect")]
    public GameObject EventObj;
    public int UseState = 0;
    public TextMesh NicName;
    private Transform Player;
    public Collider[] mColliders;
    public bool ColliderCheck = false;
    public Collider EventCollider = null;
    private float area = 10;
    public float PlayerDis = 0;

    [Header("픽업 아이템")]
    public GameObject PickUpObj;
    [Tooltip("보조용 PickUp 일경우 아이템 ID")]
    public int ItemId = -1;

    [Header("NPC 일경우 OnOff")]
    public GameObject OnOffObject;

    string SceneName;
    int RoomState = 0;
    bool useCheck = false;

    void Awake()
    {
        if (EventObj == null)
            useCheck = true;
        else
            useCheck = false;

        InitWorldInteraction();
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        ColliderCheck = false;
        
        mColliders = GetComponentsInChildren<Collider>();

        if (mColliders.Length > 0)
            ColliderCheck = true;

        if (OnOffObject != null)
            OnOffObject.SetActive(false);

        SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        switch (SceneName)
        {
            default:
                area = 7;
                RoomState = 0;
                break;
            case "Room_A":
            case "Room_B":
            case "CoffeeShop":
                area = 2.1f;
                RoomState = 1;
                break;
        }
    }

    private void FixedUpdate()
    {
        ObjectOpenCheck();
    }

    public void ObjectOpenCheck()
    {
        if (EventObj == null)
            return;

        if (UseState == 0)
        {
            PlayerDis = Vector3.Distance(Player.position, transform.position);

            if (area > PlayerDis)
            {
                if (!EventObj.activeSelf)
                    EventObj.SetActive(true);
            }
            else
                EventObj.SetActive(false);
        }
        else
        {
            EventObj.SetActive(false);
        }
    }

    public void OnInteraction()
    {
        for(int i=0;i< mColliders.Length; i ++)
            mColliders[i].isTrigger = true;

    }

    public void OutInteraction()
    {
        UseState = 0;

        switch (nowType)
        {
            default:
                for (int i = 0; i < mColliders.Length; i++)
                    mColliders[i].isTrigger = false;

                if (EventCollider != null)
                {
                    EventCollider.isTrigger = true;
                }
                break;
            case InteractionType.NicName:
            case InteractionType.NPC_PetMaster:
            case InteractionType.NPC_Cafe_0:
            case InteractionType.NPC_Cafe_1:
            case InteractionType.OutRoom:
            case InteractionType.WorldMapOut:
            case InteractionType.Cafe_In:
                break;
        }
    }

    public void InitWorldInteraction()
    {

        switch (nowType)
        {
            default:
                if (useCheck)
                {
                    EventObj = Instantiate(Resources.Load<GameObject>("Prefabs/Interaction2DObj"));
                    EventCollider = EventObj.GetComponent<Collider>();
                    EventObj.name = "상호작용";
                }
                break;
            case InteractionType.NicName:
            case InteractionType.NPC_PetMaster:
            case InteractionType.NPC_Cafe_0:
            case InteractionType.NPC_Cafe_1:
                if (EventObj != null)
                {
                    //중복 생성을 막기 위해 제거 후 생성
                    Destroy(EventObj);
                    EventObj = null;
                }

                EventObj = Instantiate(Resources.Load<GameObject>("Prefabs/NicName3DText"));
                NicName = EventObj.GetComponentInChildren<TextMesh>();
                NicName.text = DataInfo.ins.CharacterMain.NicName;
                EventObj.name = "닉네임";
                break;
            //case InteractionType.OutRoom:
            case InteractionType.Gift:
            case InteractionType.WorldMapOut:
            case InteractionType.Cafe_In:
                break;
        }

        if (useCheck && EventObj != null)
        {
            EventObj.transform.parent = transform;
            EventObj.transform.localPosition = IconPos;
            EventCollider = EventObj.GetComponent<Collider>();
        }

        switch (nowType)
        {
            case InteractionType.NPC_PetMaster: NicName.text = "Pet Master"; break;
            case InteractionType.NPC_Cafe_0: NicName.text = "Cafe Master"; break;
            case InteractionType.NPC_Cafe_1: NicName.text = "Cafe Manager"; break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("World Interaction On Trigger Enter[<color=blue>" + other.name + "</color>] Tag [<color=yellow>" + other.tag + "</color>]");
        switch (nowType)
        {
            //case InteractionType.OutRoom:
            case InteractionType.WorldMapOut:
            case InteractionType.Cafe_In:
            case InteractionType.Gift:
                if (other.tag == "Player")
                {
                    DataInfo.ins.infoController.EventScripts = this;
                    DataInfo.ins.infoController.EventState = 1;
                    DataInfo.ins.infoController.RayCastEventLogic();
                }
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}

//데모 버전에서 한글 변수 테스트
public enum InteractionType
{
    NicName = 0,
    OutRoom = 1,
    WorldMapOut,
    OnChair,
    Meditate,
    Pickup,
    Gift,
    Pet_Idx0,
    Pet_Idx1,
    Pet_Idx2,
    NPC_PetMaster,
    NPC_Cafe_0,
    NPC_Cafe_1,
    Cafe_In,
    Sleep,
    CafeChair_B,
    CafeChair_A,
    CafeChair_C,
    On_Car_A,
    Lay_Down_A,
}
