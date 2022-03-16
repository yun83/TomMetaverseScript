using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRespawn : MonoBehaviour
{

    [Tooltip("�ѹ��� ������ ������ ����")]
    public int multipleSize = 1;
    [Tooltip("�ʻ� ���� �� ������ ����")]
    public int CreateItemMax = 1;

    public int SpawnSize = 0;

    [System.Serializable]
    public  class ItemData
    {
        public int itemid = 0;
        public Vector3 pos = new Vector3();
        public GameObject ItemObj = null;
    }
    // Start is called before the first frame update

    public List<ItemData> SpawnList = new List<ItemData>();

    public int CreatCount = 0;
    public int mapItemCount = 0;

    void Start()
    {
        SpawnList.Clear();

        SpawnSize = transform.childCount;
        if (multipleSize > SpawnSize)
            multipleSize = SpawnSize;

        GameObject parentObj = new GameObject("����������");
        for (int i = 0; i < SpawnSize; i++)
        {
            int idx = i;
            ItemData item = new ItemData();
            Vector3 v3 = transform.GetChild(i).position;
            GameObject temp = Instantiate(Resources.Load("Prefabs/TestGift") as GameObject);
            WorldInteraction Woin;

            item.itemid = idx;
            item.pos = v3;
            item.ItemObj = temp;
            Woin = item.ItemObj.GetComponent<WorldInteraction>();
            Woin.PlayerPos = v3;
            item.ItemObj.transform.position = v3;
            item.ItemObj.transform.parent = parentObj.transform;
            item.ItemObj.SetActive(false);


            SpawnList.Add(item);
        }

        CreatCount = 0;
        Com.ins.ShuffleList(SpawnList);

        MakingItem();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MakingItem()
    {
        int sum = multipleSize - mapItemCount;
        for (int i = 0; i < sum; i++)
        {
            if(CreatCount >= SpawnList.Count)
            {
                mapItemCount = ShowItemCount();

                CreatCount = 0;
                Com.ins.ShuffleList(SpawnList);

                MakingItem();
                return;
            }

            if (SpawnList[CreatCount].ItemObj.activeSelf)
            {
                CreatCount++;
                continue;
            }
            SpawnList[CreatCount].ItemObj.SetActive(true);
            
            mapItemCount++;
            CreatCount++;
        }

        //���� ������ ����Ʈ�� ������ �����ϰ� �������� �ֵ��� �ٽ� ����
        if (CreatCount >= SpawnSize)
        {
            CreatCount = 0;
            Com.ins.ShuffleList(SpawnList);
        }
    }

    public void ItemDelet(WorldInteraction Woin)
    {
        Woin.gameObject.SetActive(false);

        mapItemCount--;

        if(mapItemCount < 0)
            mapItemCount = 0;

        if(mapItemCount <= CreateItemMax - 1)
        {
            MakingItem();
        }
    }

    int ShowItemCount()
    {
        int retCount = 0;
        for( int i = 0; i < SpawnList.Count; i++)
        {
            if (SpawnList[i].ItemObj.activeSelf)
                retCount++;
        }
        return retCount;
    }
}
