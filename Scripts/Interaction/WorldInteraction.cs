using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldInteraction : MonoBehaviour
{
    public InteractionType nowType = InteractionType.OutRoom;
    public GameObject EventObj;
    public Vector3 Pos;
    public InteractionCanvas InCanvas;

    public List<ButtonClass> callBackList = new List<ButtonClass>();


    void Awake()
    {
        InitWorldInteraction();
    }
    public void InitWorldInteraction()
    {
        if (EventObj != null)
        {
            //중복 생성을 막기 위해 제거 후 생성
            Destroy(EventObj);
            EventObj = null;
        }

        switch (nowType)
        {
            case InteractionType.NicName:
                EventObj = Instantiate(Resources.Load<GameObject>("Prefabs/InteractionCanvas"));

                EventObj.GetComponent<Canvas>().worldCamera = Camera.main;
                InCanvas = EventObj.GetComponent<InteractionCanvas>();
                break;
            case InteractionType.OutRoom:
                //방 나가는 문
                EventObj = Instantiate(Resources.Load<GameObject>("Prefabs/Interaction2DObj"));
                break;
        }

        EventObj.name = "WoIn";
        EventObj.transform.parent = transform;
        EventObj.transform.localPosition = Pos;
    }

    void OnClick_RomOutPopupCall()
    {
        UiButtonController ubc = GameObject.FindObjectOfType <UiButtonController> ();
        callBackList.Clear ();

        ButtonClass item1 = new ButtonClass();
        item1.text = "World Map";
        item1.addEvent = (() => {
            LoadingPage.LoadScene("World_A");
        });

        ButtonClass item2 = new ButtonClass();
        item2.text = "Quit Game";
        item2.addEvent = (() => {
            ubc.OnClick_Exit();
        });

        callBackList.Add(item1);
        callBackList.Add(item2);
        ubc.OnClick_OutRoomPopup(callBackList);

        ubc.OR_Popup.Title.text = "leave the Room";
    }
}

//데모 버전에서 한글 변수 테스트
public enum InteractionType
{
    NicName = 0,
    OutRoom = 1,
}
