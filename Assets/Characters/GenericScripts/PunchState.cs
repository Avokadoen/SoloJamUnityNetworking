using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchState : StateMachineBehaviour
{ 
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var playerInterface = animator.gameObject.GetComponent<PlayerInterface>();
        playerInterface.Punch();
    }
}
