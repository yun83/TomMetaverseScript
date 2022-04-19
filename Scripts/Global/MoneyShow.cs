using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyShow : MonoBehaviour
{
    public Text MoneyText;
    // Start is called before the first frame update
    void Start()
    {
        ShowMoneyText();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (DataInfo.ins.MoneyChange) { 
            ShowMoneyText();
        }
    }

    void ShowMoneyText()
    {
        MoneyText.text = string.Format("{0:#,###}", DataInfo.ins.CharacterMain.Money);
        DataInfo.ins.MoneyChange = false;
    }
}
