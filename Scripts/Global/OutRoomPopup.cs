using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutRoomPopup : MonoBehaviour
{
    public Text Title;
    public Transform Panel;

    public GameObject Sample;

    public List<GameObject> bList = new List<GameObject>();

    public void InitOutRoomPopup(List<ButtonClass> array)
    {
        for (int i = bList.Count - 1; i >= 0; i--)
        {
            Destroy(bList[i]);
        }

        bList.Clear();
        Sample.SetActive(true);

        for (int i = 0; i < array.Count; i++)
        {
            int idx = i;

            GameObject temp = Instantiate(Sample);
            Button _button = temp.GetComponent<Button>();
            Text _text = temp.GetComponentInChildren<Text>();

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => {
                array[idx].addEvent();
                });

            _text.text = array[idx].text;

            temp.transform.parent = Panel;
            bList.Add(temp);
        }
        Sample.SetActive(false);
    }
}
public class ButtonClass
{
    public string text;
    public delegate void ClickEvent();
    public ClickEvent addEvent;
}

