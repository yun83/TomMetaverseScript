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
    void Update()
    {
        if (DataInfo.ins.MoneyChange) { 
            ShowMoneyText();
        }
    }

    void ShowMoneyText()
    {
        MoneyText.text = DataInfo.ins.CharacterMain.Money.ToString();
        DataInfo.ins.MoneyChange = false;
    }
}
