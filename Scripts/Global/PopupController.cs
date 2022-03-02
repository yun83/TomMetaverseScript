using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    public delegate void Callback();

    public GameObject popupOK;
    public GameObject popupNo;
    public GameObject popupEsc;

    public Image BackGround;
    public Text Title;
    public Text Messege;

    RectTransform _OK_rTrans;
    //RectTransform _NO_rTrans;
    //RectTransform _ESC_rTrans;

    Button _OK;
    Button _NO;
    Button _ESC;

    Text _OK_Text;

    void Awake()
    {
        _OK = popupOK.GetComponent<Button>();
        _OK_Text = popupOK.GetComponentInChildren<Text>();
        _OK_rTrans = popupOK.GetComponent<RectTransform>();

        _NO = popupNo.GetComponent<Button>();

        _ESC = popupEsc.GetComponent<Button>();
    }

    public void PopupOk(string mTitle, string mMessege, Vector2 popupSize, Callback _OK_CallBack, Callback _Esc_CallBack)
    {
        BackGround.rectTransform.sizeDelta = popupSize;

        Title.text = mTitle;
        Messege.text = mMessege;

        popupOK.SetActive(true);
        _OK_Text.text = "OK";
        _OK.onClick.RemoveAllListeners();
        _OK.onClick.AddListener(()=> { _OK_CallBack(); });
        _OK_rTrans.anchoredPosition = new Vector2 (0, 40);

        popupNo.SetActive(false);

        popupEsc.SetActive(true);
        _ESC.onClick.RemoveAllListeners();
        _ESC.onClick.AddListener(() => { _Esc_CallBack(); });
    }
    public void PopupYesNo(string mTitle, string mMessege, Vector2 popupSize, Callback _OK_CallBack, Callback _No_CallBack)
    {
        BackGround.rectTransform.sizeDelta = popupSize;

        Title.text = mTitle;
        Messege.text = mMessege;

        popupOK.SetActive(true);
        _OK_Text.text = "Yes";
        _OK.onClick.RemoveAllListeners();
        _OK.onClick.AddListener(() => { _OK_CallBack(); }); 
        _OK_rTrans.anchoredPosition = new Vector2(-80, 40);

        popupNo.SetActive(true);
        _NO.onClick.RemoveAllListeners();
        _NO.onClick.AddListener(() => { _No_CallBack(); });

        popupEsc.SetActive(false);
    }

    public void PopupBase(string mTitle, string mMessege, Vector2 popupSize, Callback _OK_CallBack, Callback _No_CallBack, Callback _Esc_CallBack)
    {
        BackGround.rectTransform.sizeDelta = popupSize;

        Title.text = mTitle;
        Messege.text = mMessege;

        popupOK.SetActive(true);
        _OK_Text.text = "Yes";
        _OK.onClick.RemoveAllListeners();
        _OK.onClick.AddListener(() => { _OK_CallBack(); });
        _OK_rTrans.anchoredPosition = new Vector2(-80, 40);

        popupNo.SetActive(true);
        _NO.onClick.RemoveAllListeners();
        _NO.onClick.AddListener(() => { _No_CallBack(); });

        popupEsc.SetActive(true);
        _ESC.onClick.RemoveAllListeners();
        _ESC.onClick.AddListener(() => { _Esc_CallBack(); });
    }
}
