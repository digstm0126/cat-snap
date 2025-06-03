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

        [SerializeField] private  PlayerControlsInput _playerInputs;
        [SerializeField] private InputAction _Sprint;
        [SerializeField] private InputAction _fight;
        [SerializeField] private InputAction _Jump;
        [SerializeField] private InputAction _action1;
        [SerializeField] private InputAction _Move;
        [SerializeField] private InputAction _Look;
        [SerializeField] private float _Speed = 2.0f;
        [SerializeField] public enum PlayerState {run, walk, fight, fps};
        [SerializeField] private PlayerState _currentState;
        [SerializeField] private Rigidbody _rb;

        [SerializeField] private float _turnSmoothTime;
        [SerializeField] private float _turnSmothVelocity;

        private void Awake()
        {
            _playerInputs = new PlayerControlsInput();
        }
        private void OnEnable()
        {
            _Move = _playerInputs.Player.Move;
            _Move.Enable();

            _Look = _playerInputs.Player.Look;
            _Look.Enable();

            _fight = _playerInputs.Player.fight;
            _fight.Enable();
            _fight.performed += Fight;

            _Sprint = _playerInputs.Player.Sprint;
            _Sprint.Enable();
            _Sprint.performed += Sprint;
            _Sprint.canceled += SprintCanceled;

            _action1 = _playerInputs.Player.action1;
            _action1.Enable();
            _action1.performed += Action1;

            _Jump = _playerInputs.Player.Jump;
            _Jump.Enable();
            _Jump.performed += Jump;

        }

        private void ODisable()
        {
            _Move.Disable();
            _fight.Disable();
            _Sprint.Disable();
            _Look.Disable();
            _action1.Disable();
            _Jump.Disable();
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            
            Move();
            

        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (_currentState != PlayerState.fight)
            {
                _animatorController.SetTrigger("Jump");
            }
        }
        private void Action1(InputAction.CallbackContext context)
        {
            if (_currentState == PlayerState.fight)
            {
                _animatorController.SetTrigger("uperAtack");
            }
        }
        private void Sprint(InputAction.CallbackContext context)
        {
            if (_currentState == PlayerState.walk)
            {
                _camRun.enabled = true;
                _camWalk.enabled = false;
                _currentState = PlayerState.run;
            }
        }
        private void SprintCanceled(InputAction.CallbackContext context)
        {
            if (_currentState == PlayerState.run)
            {
                _camWalk.enabled = true;
                _camRun.enabled = false;
                _currentState = PlayerState.walk;
            }
        }
        private void Fight(InputAction.CallbackContext context)
        {
            if (_currentState == PlayerState.fight)
            {
                _animatorController.SetBool("fight", false);
                _camWalk.enabled = true;
                _camRun.enabled = false;
                _currentState = PlayerState.walk;
            }
            else if (_currentState == PlayerState.walk)
            {
                _animatorController.SetBool("fight", true);
                _camRun.enabled = true;
                _camWalk.enabled = false;
                _currentState = PlayerState.fight;
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
                case PlayerState.fight:
                    Fight();
                    break;
            }
        }


        private void Fight()
        {

            float walkZ = _Move.ReadValue<Vector2>().y;
            float walkX = _Move.ReadValue<Vector2>().x;

            _animatorController.SetFloat("walkZ", walkZ);
            _animatorController.SetFloat("walkX", walkX);

            float _SpeedFight = 2f;

            _rb.MovePosition(_rb.position + _rb.transform.forward * walkZ * _SpeedFight * Time.deltaTime);
            _rb.MovePosition(_rb.position + _rb.transform.right * walkX * _SpeedFight * Time.deltaTime);
            

        }


        private void Run()
        {

            //get inputs
            float walkZ = _Move.ReadValue<Vector2>().y;
            float walkX = _Move.ReadValue<Vector2>().x;

            walkZ *= 2;
            //walkX *= 2;
            _animatorController.SetFloat("walkZ", walkZ);
            _animatorController.SetFloat("walkX", walkX);

            //rotate to camra
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

            float _mag = new Vector2(walkZ, walkX).magnitude;
            if (_mag > 0.1f)
            {
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
            else
            {
                _animatorController.SetFloat("walkZ", walkZ);
                _animatorController.SetFloat("walkX", walkX);
            }
        }
    }
}