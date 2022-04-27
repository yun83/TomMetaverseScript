using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetRoomPoint : MonoBehaviour
{
    public PetMoveController PetManager;
    public int State = 0;
    float AniCheckTime = 0;

    private void Awake()
    {
        State = 0;
        PetSearch();
    }

    private void Update()
    {
        PetAniLogig();
    }

    private void PetSearch()
    {
        GameObject trunk = GameObject.FindGameObjectWithTag("Pet");
        if (trunk != null)
        {
            Vector3 tempPos = transform.position;
            PetManager = trunk.GetComponent<PetMoveController>();
            PetManager.MoveStop = true;

            PetManager.transform.position = tempPos;
            PetManager.transform.rotation = transform.rotation;

            Com.ins.AniSetInt(PetManager.PetAni, "Move", 0);
            State = 1;
        }
        else
        {
            Invoke("PetSearch", 0.2f);
        }
    }

    void PetAniLogig()
    {
        int rand2 = 3;
        switch (State)
        {
            case 1:
                {
                    int rand = Random.Range(0, 1000);
                    if (rand < 400)
                        State = 2;
                    else if (rand < 800)
                        State = 3;
                    else if (rand < 900)
                        State = 4;
                    else// if (rand < 1000)
                        State = 5;

                }
                break;
            case 2:
                Com.ins.AniSetInt(PetManager.PetAni, "Touch", 11);
                rand2 = Random.Range(10, 20);
                AniCheckTime = Time.time + rand2;
                State = 10;
                break;
            case 3:
                Com.ins.AniSetInt(PetManager.PetAni, "Touch", 12);
                rand2 = Random.Range(5, 10);
                AniCheckTime = Time.time + rand2;
                State = 10;
                break;
            case 4:
                Com.ins.AniSetInt(PetManager.PetAni, "Touch", 13);
                rand2 = Random.Range(5, 8);
                AniCheckTime = Time.time + rand2;
                State = 10;
                break;
            case 5:
                Com.ins.AniSetInt(PetManager.PetAni, "Touch", 14);
                rand2 = Random.Range(3, 5);
                AniCheckTime = Time.time + rand2;
                State = 10;
                break;
            case 10:
                if(AniCheckTime < Time.time)
                    State = 1;
                break;
        }
    }
}
