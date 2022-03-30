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

    public void InitWorldInteraction()
    {
        if (EventObj != null)
        {
            //�ߺ� ������ ���� ���� ���� �� ����
            Destroy(EventObj);
            EventObj = null;
        }

        if(nowType == InteractionType.NicName || nowType == InteractionType.NPC_PetMaster )
        {
            EventObj = Instantiate(Resources.Load<GameObject>("Prefabs/NicName3DText"));
            NicName = EventObj.GetComponentInChildren<TextMesh>();
            NicName.text = DataInfo.ins.CharacterMain.NicName;
            EventObj.name = "�г���";
        }
        else
        {
            EventObj = Instantiate(Resources.Load<GameObject>("Prefabs/Interaction2DObj"));
            EventIcon = EventObj.GetComponentInChildren<SpriteRenderer>();
            EventObj.name = "��ȣ�ۿ�";
        }

        switch (nowType)
        {
            case InteractionType.OnChair: EventIcon.sprite = Resources.Load<Sprite>("Icon/CHAIR"); break;
            case InteractionType.Meditate: EventIcon.sprite = Resources.Load<Sprite>("Icon/DESK"); break;
            case InteractionType.Pickup: EventIcon.sprite = Resources.Load<Sprite>("Icon/GIFT"); break;
            case InteractionType.Gift: EventIcon.sprite = Resources.Load<Sprite>("Icon/GIFT"); break;
            case InteractionType.NPC_PetMaster:
                NicName.text = "Pet Master";
                break;
        }

        EventObj.transform.parent = transform;
        EventObj.transform.localPosition = IconPos;
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
