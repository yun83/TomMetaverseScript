using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldInteraction : MonoBehaviour
{
    public InteractionType nowType = InteractionType.OutRoom;
    [Header("��ġ����")]
    public Vector3 IconPos;

    [Tooltip("��Ȯ�� ��ǥ ������ ���ؼ� ���� ������ش�")]
    public Vector3 PlayerPos;
    public Vector3 PlayerRotation;

    [Header("Type Ect")]
    public GameObject EventObj;
    public int UseState = 0;
    public TextMesh NicName;
    private SpriteRenderer EventIcon;
    private Transform Player;
    public Collider mCollider;
    private float area = 10;

    public int ItemId = -1;

    void Awake()
    {
        InitWorldInteraction();
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        if(gameObject.TryGetComponent<Collider>(out var Col))
        {
            mCollider = Col;
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
            if (area > Vector3.Distance(Player.position, transform.position))
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
        mCollider.isTrigger = true;
    }

    public void OutInteraction()
    {

        UseState = 0;

        switch (nowType)
        {
            default:
                if (mCollider != null)
                {
                    mCollider.isTrigger = false;
                }
                break;
            case InteractionType.NicName:
            case InteractionType.NPC_PetMaster:
                break;
            case InteractionType.OutRoom:
            case InteractionType.WorldMapOut:
                break;
        }
    }

    public void InitWorldInteraction()
    {
        if (EventObj != null)
        {
            //�ߺ� ������ ���� ���� ���� �� ����
            Destroy(EventObj);
            EventObj = null;
        }

        switch (nowType)
        {
            default:
                EventObj = Instantiate(Resources.Load<GameObject>("Prefabs/Interaction2DObj"));
                EventIcon = EventObj.GetComponentInChildren<SpriteRenderer>();
                EventObj.name = "��ȣ�ۿ�";
                break;
            case InteractionType.NicName:
            case InteractionType.NPC_PetMaster:
                EventObj = Instantiate(Resources.Load<GameObject>("Prefabs/NicName3DText"));
                NicName = EventObj.GetComponentInChildren<TextMesh>();
                NicName.text = DataInfo.ins.CharacterMain.NicName;
                EventObj.name = "�г���";
                break;
            case InteractionType.OutRoom:
            case InteractionType.WorldMapOut:
                break;
        }

        if (EventObj != null)
        {
            EventObj.transform.parent = transform;
            EventObj.transform.localPosition = IconPos;
        }

        switch (nowType)
        {
            case InteractionType.OnChair: EventIcon.sprite = Resources.Load<Sprite>("Icon/CHAIR"); break;
            case InteractionType.Meditate: EventIcon.sprite = Resources.Load<Sprite>("Icon/DESK"); break;
            case InteractionType.Pickup: EventIcon.sprite = Resources.Load<Sprite>("Icon/GIFT"); break;
            case InteractionType.Gift: EventIcon.sprite = Resources.Load<Sprite>("Icon/GIFT"); break;
            case InteractionType.NPC_PetMaster: NicName.text = "Pet Master"; break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("World Interaction On Trigger Enter[<color=blue>" + other.name + "</color>] Tag [<color=yellow>" + other.tag + "</color>]");
        switch (nowType)
        {
            case InteractionType.OutRoom:
            case InteractionType.WorldMapOut:
                if(other.tag == "Player")
                {
                    DataInfo.ins.infoController.EventScripts = this;
                    DataInfo.ins.infoController.EventState = 1;
                    DataInfo.ins.infoController.RayCastEventLogic();
                }
                break;
        }
    }
}

//���� �������� �ѱ� ���� �׽�Ʈ
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
}
