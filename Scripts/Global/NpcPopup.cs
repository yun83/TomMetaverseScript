using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcPopup : MonoBehaviour
{
    public int State = 0;
    public GameObject ButtonGroup;
    public GameObject MsgPage;
    public Text NpcMsg;

    int MsgCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitNpc() { }

    // Update is called once per frame
    void Update()
    {
        StateLogic();
    }

    void StateLogic()
    {
        switch (State)
        {
            case 0: break;
            case 1:
                if (Input.GetMouseButtonDown(0))
                {
                }
                break;
        }
    }
}
