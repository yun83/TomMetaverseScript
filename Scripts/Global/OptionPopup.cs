using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPopup : MonoBehaviour
{
    public RectTransform BackGround;
    public SwitchButton[] SwitchButton;
    public Color SwitchColor;
    public Color Button_OnColor;
    public Color Button_OffColor;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < SwitchButton.Length; i++)
        {
            int tryI = i;

            SwitchButton[tryI].SwitchRect.GetComponent<Image>().color = SwitchColor;
            SwitchButton[tryI].OnColor = Button_OnColor;
            SwitchButton[tryI].OffColor = Button_OffColor;

            switch (tryI)
            {
                case 0: SwitchButton[tryI].IsTrue = DataInfo.ins.OptionInfo.EffectSound; break;
                case 1: SwitchButton[tryI].IsTrue = DataInfo.ins.OptionInfo.BgmSound; break;
                case 2: SwitchButton[tryI].IsTrue = DataInfo.ins.OptionInfo.NicNameOpen; break;
            }

            SwitchButton[tryI].OptionMenu.onClick.RemoveAllListeners();
            SwitchButton[tryI].OptionMenu.onClick.AddListener(() => { OnClick_SwitchButton(tryI); });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClick_SwitchButton(int idx)
    {
        SwitchButton[idx].IsTrue = (SwitchButton[idx].IsTrue == true) ? false : true;
        bool tempCheck = SwitchButton[idx].IsTrue;

        switch (idx)
        {
            case 0: DataInfo.ins.OptionInfo.EffectSound = (DataInfo.ins.OptionInfo.EffectSound == true) ? false : true; ; break;
            case 1:
                DataInfo.ins.OptionInfo.BgmSound = (DataInfo.ins.OptionInfo.BgmSound == true) ? false : true;

                if (tempCheck)
                    Com.ins.BgmSoundPlay(Resources.Load<AudioClip>("BGM/Progress"));
                else
                    Com.ins.BgmStop();
                break;
            case 2: DataInfo.ins.OptionInfo.NicNameOpen = (DataInfo.ins.OptionInfo.NicNameOpen == true) ? false : true; ; break;
        }

        //Debug.Log("Click Check [ " + idx + " ] :: " + tempCheck);
        DataInfo.ins.saveOption = JsonUtility.ToJson(DataInfo.ins.OptionInfo);
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Pop Up"));
    }

    public void OnClick_Esc()
    {
        Com.ins.SoundPlay(Resources.Load<AudioClip>("Sound/Pop Up"));
        gameObject.SetActive(false);
    }
}
