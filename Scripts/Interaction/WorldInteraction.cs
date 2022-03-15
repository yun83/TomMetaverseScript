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

    void Awake()
    {
        InitWorldInteraction();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
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
        }
        else
        {
            EventObj = Instantiate(Resources.Load<GameObject>("Prefabs/Interaction2DObj"));
            EventIcon = EventObj.GetComponentInChildren<SpriteRenderer>();
        }

        switch (nowType)
        {
            case InteractionType.OnChair: EventIcon.sprite = Resources.Load<Sprite>("Icon/CHAIR"); break;
            //default: EventIcon.sprite = Resources.Load<Sprite>("Icon/SignOut"); break;
            case InteractionType.Meditate: EventIcon.sprite = Resources.Load<Sprite>("Icon/DESK"); break;
        }

        EventObj.name = "��ȣ�ۿ�";
        EventObj.transform.parent = transform;
        EventObj.transform.localPosition = IconPos;
    }

    public void RayCastEventTrigger()
    {
        Debug.Log("Ray Cast Event Trigger [<color=blue>" + transform.name + "</color>] Tag [<color=yellow>" + nowType.ToString() + "</color>]");
        switch (nowType)
        {
            case InteractionType.OutRoom:
                DataInfo.ins.RoomOutButtonSetting();
                break;
            case InteractionType.OnChair:
                Player.position = transform.position;
                break;
        }
    }

}

//���� �������� �ѱ� ���� �׽�Ʈ
public enum InteractionType
{
    NicName = 0,
    OutRoom = 1,
    OnChair,
    Meditate,
    Pickup,
}
