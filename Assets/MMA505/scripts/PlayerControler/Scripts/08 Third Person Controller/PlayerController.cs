using System;
using System.Collections;


using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // protected static PlayerController _Instance = null;
    // public static PlayerController Instance
    // {
    //     get
    //     {
    //         if (_Instance == null)
    //             _Instance = FindObjectOfType<PlayerController>();

    //         return _Instance;
    //     }
    // }

    [SerializeField] private Animator _animatorController;
    [SerializeField] private Transform _cam;
    [SerializeField] private float _Speed = 2.0f;
    [SerializeField] private bool _Jump;
    [SerializeField] private bool _Run;
    [SerializeField] private string _CurrentState;
    [SerializeField] private Rigidbody _rb;

    [SerializeField] private float _turnSmoothTime;
    [SerializeField] private float _turnSmothVelocity;

    // Update is called once per frame
    void Update()
    {

        _CurrentState = "Idle";

        float walkZ = Input.GetAxis("Vertical");
        float walkX = Input.GetAxis("Horizontal");

        if (_Run == true)
        {
            walkZ *= 2;
            walkX *= 2;
        }

        float walkMag = new Vector2(walkZ, walkX).magnitude;

        _animatorController.SetFloat("walkZ", walkZ);
        _animatorController.SetFloat("walkX", walkX);
        
        if (walkMag >= 0.1f)
        {

            Vector3 lookDirections = new Vector3(walkX, 0, walkZ).normalized;

            float targetAngle = MathF.Atan2(lookDirections.x, lookDirections.z) * Mathf.Rad2Deg + _cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmothVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            _rb.MovePosition(_rb.position + moveDir.normalized * _Speed * Time.deltaTime);

        }

        if (walkMag > 0) _CurrentState = "Walking";

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animatorController.SetTrigger("Jump");
            _CurrentState = "Jumping";
        }

    }
}