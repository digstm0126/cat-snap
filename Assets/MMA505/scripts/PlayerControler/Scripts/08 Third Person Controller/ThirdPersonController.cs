using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    protected static ThirdPersonController _Instance = null;
    public static ThirdPersonController Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<ThirdPersonController>();

            return _Instance;
        }
    }

    public Animator animatorController;
    public float walkSpeed = 2.0f;
    public float rotateSpeed = 180.0f;

    public bool useWalkSpeedMultiplier = false;

    public ThirdPersonControllerAbstractBehaviour[] thirdPersonControllerBehaviours;

    public string CurrentState;

    // Update is called once per frame
    protected virtual void Update()
    {
        CurrentState = "Idle";

        foreach (ThirdPersonControllerAbstractBehaviour thirdPersonControllerBehaviour in thirdPersonControllerBehaviours)
            if (thirdPersonControllerBehaviour.ExecuteBehaviourIfRequired(ref CurrentState)) return;
            
        float walkAmount = Input.GetAxis("Vertical") * walkSpeed ;
        float rotateAmount = Input.GetAxis("Horizontal") * rotateSpeed ;

        transform.Translate(0, 0, walkAmount * Time.deltaTime);
        transform.Rotate(0, rotateAmount * Time.deltaTime, 0);

        if (!useWalkSpeedMultiplier)
        {
            animatorController.SetBool("isWalking", walkAmount > 0);
            if (walkAmount > 0) CurrentState = "Walking";
        }
        else
        {
            animatorController.SetBool("isWalking", walkAmount != 0);
            animatorController.SetFloat("WalkSpeedMultiplier", walkAmount);
            if (walkAmount != 0) CurrentState = "Walking";
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animatorController.SetTrigger("Jump");
            CurrentState = "Jumping";
        }
    }
}
