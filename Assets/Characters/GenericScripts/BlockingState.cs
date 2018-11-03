using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var playerInterface = animator.gameObject.GetComponent<PlayerInterface>();
        playerInterface.ToggleIsBlocking();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        var playerInterface = animator.gameObject.GetComponent<PlayerInterface>();
        playerInterface.ToggleIsBlocking();
    }
}
