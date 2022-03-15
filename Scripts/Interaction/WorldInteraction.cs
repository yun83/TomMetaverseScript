using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldInteraction : MonoBehaviour
{
    public InteractionType nowType = InteractionType.OutRoom;
    public GameObject EventObj;
    public Vector3 IconPos;

    [Tooltip("명확한 좌표 설정을 위해서 따로 명시해준다")]
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
            //중복 생성을 막기 위해 제거 후 생성
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

        EventObj.name = "상호작용";
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

//데모 버전에서 한글 변수 테스트
public enum InteractionType
{
    NicName = 0,
    OutRoom = 1,
    OnChair,
    Meditate,
    Pickup,
}
