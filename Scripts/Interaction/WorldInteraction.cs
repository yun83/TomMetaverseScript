using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldInteraction : MonoBehaviour
{
    public InteractionType nowType = InteractionType.OutRoom;
    public GameObject EventObj;
    public Vector3 IconPos;

    [Tooltip("��Ȯ�� ��ǥ ������ ���ؼ� ���� ������ش�")]
    public Vector3 PlayerPos;
    public Vector3 PlayerRotation;

    public TextMesh NicName;
    private SpriteRenderer EventIcon;
    private Transform Player;
    public Collider mCollider;

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

    public void InitWorldInteraction()
    {
        if (EventObj != null)
        {
            //�ߺ� ������ ���� ���� ���� �� ����
            Destroy(EventObj);
            EventObj = null;
        }

        if(nowType == InteractionType.NicName)
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
            case InteractionType.Gift: EventIcon.sprite = Resources.Load<Sprite>("Icon/GIFT"); break;
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
}
