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

            State = 1;
        }
        else
        {
            Invoke("PetSearch", 0.2f);
        }
    }

    void PetAniLogig()
    {
        switch (State)
        {
            case 1:
                {
                    int rand = Random.Range(0, 100);
                    int rand2 = Random.Range(5, 15);
                    if (rand < 50)
                    {
                        State = 2;
                    }
                    else
                    {
                        State = 3;
                    }
                    AniCheckTime = Time.time + rand2;
                }
                break;
            case 2:
                Com.ins.AniSetInt(PetManager.PetAni, "Touch", 11);
                State = 10;
                break;
            case 3:
                Com.ins.AniSetInt(PetManager.PetAni, "Touch", 12);
                State = 10;
                break;
            case 10:
                if(AniCheckTime < Time.time)
                    State = 1;
                break;
        }
    }
}
