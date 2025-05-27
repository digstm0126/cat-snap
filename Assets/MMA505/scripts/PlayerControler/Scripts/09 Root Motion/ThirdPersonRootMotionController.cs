using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonRootMotionController : ThirdPersonController
{

    // Update is called once per frame
    protected override void Update()
    {
        CurrentState = "Idle";

        useWalkSpeedMultiplier = true;

        foreach (ThirdPersonControllerAbstractBehaviour thirdPersonControllerBehaviour in thirdPersonControllerBehaviours)
            if (thirdPersonControllerBehaviour.ExecuteBehaviourIfRequired(ref CurrentState)) return;

        float walkAmount = Input.GetAxis("Vertical") * walkSpeed ;
        float rotateAmount = Input.GetAxis("Horizontal") * rotateSpeed ;

        //transform.Translate(0, 0, walkAmount * Time.deltaTime);
        transform.Rotate(0, rotateAmount * Time.deltaTime, 0);

        animatorController.SetBool("isWalking", walkAmount != 0);
        animatorController.SetFloat("WalkSpeedMultiplier", walkAmount);
        if (walkAmount != 0) CurrentState = "Walking";

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animatorController.SetTrigger("Jump");
            CurrentState = "Jumping";
        }
    }
}
