using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopup : MonoBehaviour
{
    public CharacterManager ShopCharacter;
    public Text NicName;

    private Vector2 nowPos, prePos;
    private Vector2 movePosDiff;
    // Start is called before the first frame update
    void Start()
    {
        if (!DataInfo.ins.SaveData.Equals("") && DataInfo.ins.SaveData != null)
        {
            Debug.Log(" ------------- CharacterWindow 로딩 ------------- ");
            DataInfo.ins.CharacterSub = JsonUtility.FromJson<Info_Char>(DataInfo.ins.SaveData);
        }

        NicName.text = DataInfo.ins.CharacterSub.NicName;
        ShopCharacter.itemEquipment(DataInfo.ins.CharacterSub);
    }

    public void Evnet_PointDown()
    {
        prePos = Input.mousePosition;
    }

    public void Event_PointUp()
    {
    }

    public void Event_Drag()
    {
        movePosDiff = Vector3.zero;

        nowPos = Input.mousePosition;
        movePosDiff = (Vector2)((prePos - nowPos) * Time.deltaTime);

        if (movePosDiff.x < 0)
        {
            ShopCharacter.transform.Rotate(0, movePosDiff.x, 0);
        }
        else if (movePosDiff.x > 0)
        {
            ShopCharacter.transform.Rotate(0, movePosDiff.x, 0);
        }
    }

    public void OnClick_Item()
    {
        DataInfo.ins.ItemSelectIndex = -1;

        //DataInfo.ins.InvenNumber = idx;

        //if (setScroll == null)
        //    setScroll = InvenScrollView.GetComponent<InitScroll>();

        //DataInfo.ins.CostumeScrollList.Clear();

        //for (int i = 0; i < DataInfo.ins.CoustumList[idx].Count; i++)
        //{
        //    //성별이 공통이거나 같을때에 리스트에 추가
        //    if (DataInfo.ins.CoustumList[idx][i].Sex == DataInfo.ins.CharacterSub.Sex || DataInfo.ins.CoustumList[idx][i].Sex == 2)
        //    {
        //        DataInfo.ins.CostumeScrollList.Add(DataInfo.ins.CoustumList[idx][i]);
        //    }
        //}

        //yield return new WaitForEndOfFrame();
        ////yield return new WaitForSeconds(1.0f);
        //setScroll.totalCount = DataInfo.ins.CostumeScrollList.Count;

        //setScroll.InitScrollCall();
    }
}
