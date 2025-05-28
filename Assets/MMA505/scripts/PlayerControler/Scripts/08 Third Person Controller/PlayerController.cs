using System.Collections;


using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    protected static PlayerController _Instance = null;
    public static PlayerController Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<PlayerController>();

            return _Instance;
        }
    }

    public CharacterController controller;
    public Animator animatorController;
    public Transform cam;
    public float walkSpeed = 2.0f;
    public float rotateSpeed = 180.0f;
    public bool useWalkSpeedMultiplier = false;
    public ThirdPersonControllerAbstractBehaviour[] thirdPersonControllerBehaviours;
    public string CurrentState;

    // Update is called once per frame
    void Update()
    {

        CurrentState = "Idle";

        foreach (ThirdPersonControllerAbstractBehaviour thirdPersonControllerBehaviour in thirdPersonControllerBehaviours)
            if (thirdPersonControllerBehaviour.ExecuteBehaviourIfRequired(ref CurrentState)) return;

        float walkX = Input.GetAxis("Vertical");
        float walkY = Input.GetAxis("Horizontal");

        float walkMag = new Vector2(walkX, walkY).magnitude;

        transform.Translate(walkY, 0, walkX * Time.deltaTime);

        animatorController.SetFloat("walkX", walkX);
        animatorController.SetFloat("walkY", walkY);
        if (walkMag > 0) CurrentState = "Walking";

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animatorController.SetTrigger("Jump");
            CurrentState = "Jumping";
        }

    }
}