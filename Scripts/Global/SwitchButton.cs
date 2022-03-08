using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    public RectTransform thisRect;
    public Button OptionMenu;
    public RectTransform SwitchRect;
    Image ButtonImage;
    public Color OnColor;
    public Color OffColor;

    public bool StartCheck = false;
    public bool IsTrue = true;
    private bool compareIsTrue = false;

    // Start is called before the first frame update
    void Start()
    {
        if(thisRect == null)
            thisRect = GetComponent<RectTransform>();

        StartCheck = false;
        ButtonImage = OptionMenu.GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        if(IsTrue != compareIsTrue || !StartCheck)
        {
            StartCheck = true;
            compareIsTrue = IsTrue;
            if (compareIsTrue)
            {
                SwitchRect.anchoredPosition = new Vector2(40, 0);
                ButtonImage.color = OnColor;

            }
            else
            {
                SwitchRect.anchoredPosition = new Vector2(-40, 0);
                ButtonImage.color = OffColor;
            }
        }
    }
}
