using System;
using System.Collections;


using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using MMA505;

namespace MMA505
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Animator _animatorController;
        [SerializeField] private CinemachineCamera _camWalk;
        [SerializeField] public CinemachineCamera _camRun;

        [SerializeField] private InputAction _Sprint;
        [SerializeField] private InputAction _Move;
        [SerializeField] private InputAction _Look;
        [SerializeField] private float _Speed = 2.0f;
        [SerializeField] private bool _Jump;
        //[SerializeField] private bool _Run;
        [SerializeField] public enum PlayerState {run, walk, fight, fps};
        [SerializeField] private PlayerState _currentState;
        [SerializeField] private Rigidbody _rb;

        [SerializeField] private float _turnSmoothTime;
        [SerializeField] private float _turnSmothVelocity;

        void Start()
        {

            _Move = InputSystem.actions.FindAction("Move");
            _Sprint = InputSystem.actions.FindAction("Sprint");
            _Look = InputSystem.actions.FindAction("Look");

        }

        // Update is called once per frame
        void Update()
        {

            float walkZ = _Move.ReadValue<Vector2>().y;
            float walkX = _Move.ReadValue<Vector2>().x;
            float _mag = new Vector2(walkZ, walkX).magnitude;


            if (_Sprint.ReadValue<float>() == 1)
            {

                _currentState = PlayerState.run;
            }
            else
            {
                _currentState = PlayerState.walk;
            }

            SetCams();


            if (_mag > 0.1f)
            {
                Move();
            }
            else
            {
                _animatorController.SetFloat("walkZ", walkZ);
                _animatorController.SetFloat("walkX", walkX);
            }

        }

        private void Move()
        {
            switch (_currentState)
            {
                case PlayerState.run:
                    Run();
                    break;
                case PlayerState.walk:
                    Walk();
                    break;
            }
        }

        private void SetCams()
        {
            switch (_currentState)
            {
                case PlayerState.run:
                    _camRun.enabled = true;
                    _camWalk.enabled = false;
                    break;
                case PlayerState.walk:
                    _camRun.enabled = false;
                    _camWalk.enabled = true;
                    break;
            }
        }



        private void Run()
        {

            //get inputs
            float walkZ = _Move.ReadValue<Vector2>().y;
            float walkX = _Move.ReadValue<Vector2>().x;

            walkZ *= 2;
            walkX *= 2;
            _animatorController.SetFloat("walkZ", walkZ);

            //rotate to camra
            //float mouseX = Input.GetAxis("Mouse X");
            float _mouseX = _Look.ReadValue<Vector2>().x;
            transform.Rotate(Vector3.up, _mouseX);

            _rb.MovePosition(_rb.position + _rb.transform.forward * walkZ * _Speed * Time.deltaTime);
            //move along left and wright
            //_rb.MovePosition(_rb.position + _rb.transform.right * walkX * _Speed * Time.deltaTime);

        }

        private void Walk()
        {

            //get inputs
            float walkZ = _Move.ReadValue<Vector2>().y;
            float walkX = _Move.ReadValue<Vector2>().x;

            //gets direction of input
            Vector3 lookDirections = new Vector3(walkX, 0, walkZ).normalized;

            //turns it into an agle from the perspective of the camra
            float targetAngle = MathF.Atan2(lookDirections.x, lookDirections.z) * Mathf.Rad2Deg + _camWalk.transform.eulerAngles.y;
            //gives you the agile with nice smoth truning dampaning that the player needs to 
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmothVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //the directions player is moving
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            // Debug.DrawRay(transform.position, moveDir.normalized * 2, Color.red, 0.1f);
            _rb.MovePosition(_rb.position + moveDir.normalized * _Speed * Time.deltaTime);

            //sets animation so they walk foard
            _animatorController.SetFloat("walkZ", moveDir.magnitude);

        }
    }
}