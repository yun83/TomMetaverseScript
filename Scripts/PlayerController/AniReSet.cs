using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniReSet : StateMachineBehaviour
{
    public string IntegerName = "Emotion";
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("���ϸ��̼� ���۽�");

        //Ʈ���� ����ó�� ����� �ֱ� ���ؼ� �������ͽ� �ʱ�ȭ
        if(DataInfo.ins.CharacterMain.Sex != 1)
            animator.SetInteger(IntegerName, 0);
        else
            animator.SetInteger(IntegerName, 100);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("���ϸ��̼� ����ÿ� ȣ��");
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
