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
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Animator _animatorController;
        [SerializeField] private PlayerCamControler _camraControler;
        [SerializeField] private IkHandCam _ikHanderl;
        [SerializeField] private CinemachineCamera _camWalk;
        [SerializeField] private CinemachineCamera _camRun;
        [SerializeField] private CinemachineCamera _camFps;
        [SerializeField] private GameObject _CamHolder;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private GameObject _gourndcheck;
        [SerializeField] private float _groundCheckDistance = 0.3f;
        private bool _isGrounded;
        [SerializeField] private float _jumpFource = 45.0f;
        [SerializeField] private float _Speed = 4.0f;
        [SerializeField] private float _runMult = 2.0f;
        public enum PlayerState { run, walk, fight, fps };
        [SerializeField] private PlayerState _currentState = PlayerState.walk;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _turnSmoothTime = 0.1f;
        [SerializeField] private float _turnSmothVelocity;

        #region inputactions


        private PlayerControlsInput _playerInputs;
        private InputAction _Sprint;
        private InputAction _fight;
        private InputAction _Jump;
        private InputAction _action1;
        private InputAction _Fps;
        private InputAction _Move;
        private InputAction _Look;

        #endregion


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

            _Fps = _playerInputs.Player.Fps;
            _Fps.Enable();
            _Fps.performed += Fps;

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
        private void OnDisable()
        {
            _Move.Disable();
            _fight.Disable();
            _Fps.Disable();
            _Sprint.Disable();
            _Look.Disable();
            _action1.Disable();
            _Jump.Disable();

        }

        void Start()
        {

        }
        void Update()
        {
            CheckGrounded();
            Move();

            
            if (Input.GetKeyDown(KeyCode.Escape) && _currentState == PlayerState.fps)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            // Visual debug
            Debug.DrawRay(_gourndcheck.transform.position, Vector3.down * _groundCheckDistance, _isGrounded ? Color.green : Color.red);

        }

        #region inputActions
        private void Jump(InputAction.CallbackContext context)
        {
            if (_currentState != PlayerState.fight && _currentState != PlayerState.fps && _isGrounded)
            {

                _animatorController.SetTrigger("Jump");
                _rb.AddForce(new Vector3(0.0f, _jumpFource, 0.0f), ForceMode.Impulse);

            }
        }
        private void Action1(InputAction.CallbackContext context)
        {

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (_currentState == PlayerState.fight)
            {
                _animatorController.SetTrigger("uperAtack");
            }
            else if (_currentState == PlayerState.fps)
            {
                _camraControler.TakePhoto();
            }
        }
        private void Sprint(InputAction.CallbackContext context)
        {
            if (_currentState == PlayerState.walk)
            {
                _currentState = PlayerState.run;
            }
            else if (_currentState == PlayerState.fight)
            {
                _animatorController.SetTrigger("lowAtack");
            }
        }
        private void SprintCanceled(InputAction.CallbackContext context)
        {
            if (_currentState == PlayerState.run)
            {
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
                //turns it into an agle from the perspective of the camra
                float targetAngle = _camWalk.transform.rotation.eulerAngles.y;
                //rotats player
                transform.eulerAngles = new Vector3(0f, targetAngle, 0f);

                _animatorController.SetBool("fight", true);
                _camRun.enabled = true;
                _camWalk.enabled = false;
                _currentState = PlayerState.fight;
            }
        }
        private void Fps(InputAction.CallbackContext context)
        {
            if (_currentState == PlayerState.walk)
            {

                //turns it into an agle from the perspective of the camra
                float targetAngle = _camWalk.transform.rotation.eulerAngles.y;
                //rotats player
                transform.eulerAngles = new Vector3(0f, targetAngle, 0f);
                //rotates camra this was a pain to figure out
                _camFps.GetComponent<CinemachinePanTilt>().PanAxis.Value = targetAngle;
                //reset holders
                _CamHolder.transform.eulerAngles = new Vector3(0, _CamHolder.transform.eulerAngles.y, _CamHolder.transform.eulerAngles.z);
                //switches cams
                _camWalk.enabled = false;
                _camFps.enabled = true;

                //set ik
                _ikHanderl.SwapIK();

                //sets animation stuff   
                _animatorController.SetBool("fps", true);
                //changes state
                _currentState = PlayerState.fps;
            }
            else if (_currentState == PlayerState.fps)
            {
                _animatorController.SetBool("fps", false);
                _camWalk.enabled = true;
                _camFps.enabled = false;

                //set ik
                _ikHanderl.SwapIK();

                _currentState = PlayerState.walk;
            }
            else if (_currentState == PlayerState.fight)
            {
                _animatorController.SetTrigger("tount");
            }
        }

        #endregion

        private void CheckGrounded()
        {

            _isGrounded = Physics.Raycast(
                _gourndcheck.transform.position,
                Vector3.down,
                _groundCheckDistance,
                _groundLayer
            );
        }

        public PlayerState GetPlayerState()
        {
            return _currentState;
        }

        #region movemnts

        private void Move()
        {
            switch (_currentState)
            {
                case PlayerState.run:
                    Walk();
                    break;
                case PlayerState.walk:
                    Walk();
                    break;
                case PlayerState.fight:
                    Fight();
                    break;
                case PlayerState.fps:
                    Fpsmove();
                    break;
                default:
                    break;
            }
        }

        private void Fight()
        {

            float walkZ = _Move.ReadValue<Vector2>().y;
            float walkX = _Move.ReadValue<Vector2>().x;

            _animatorController.SetFloat("walkZ", walkZ);
            _animatorController.SetFloat("walkX", walkX);

            float _SpeedFight = 0.5f;

            _rb.AddForce(_rb.transform.forward * (walkZ * _SpeedFight), ForceMode.VelocityChange);
            _rb.AddForce(_rb.transform.right * (walkX * _SpeedFight), ForceMode.VelocityChange);
        }

        private void Fpsmove()
        {
            float walkZ = _Move.ReadValue<Vector2>().y;
            float walkX = _Move.ReadValue<Vector2>().x;

            _animatorController.SetFloat("walkZ", walkZ);
            _animatorController.SetFloat("walkX", walkX);

            float _SpeedFps = 0.5f;

            //rotate camra left to right pan
            float targetAngle = _camFps.transform.eulerAngles.y;

            //with smoth turning
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmothVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            _CamHolder.transform.eulerAngles = new Vector3(_camFps.GetComponent<CinemachinePanTilt>().TiltAxis.Value, _CamHolder.transform.eulerAngles.y, _CamHolder.transform.eulerAngles.z);

            _rb.AddForce(_rb.transform.forward * (walkZ * _SpeedFps), ForceMode.VelocityChange);
            _rb.AddForce(_rb.transform.right * (walkX * _SpeedFps), ForceMode.VelocityChange);

        }


        private void Walk()
        {
            float walkZ;
            float walkX;
            if (_currentState == PlayerState.walk)
            {
                //get inputs
                walkZ = _Move.ReadValue<Vector2>().y;
                walkX = _Move.ReadValue<Vector2>().x;
            }
            else
            {
                //run

                //get inputs
                walkZ = _Move.ReadValue<Vector2>().y * _runMult;
                walkX = _Move.ReadValue<Vector2>().x * _runMult;

            }


            float _mag = new Vector2(walkZ, walkX).magnitude;
            if (_mag > 0.1f)
            {
                //gets direction of input
                Vector3 _moveDirections = new Vector3(walkX, 0, walkZ);

                //turns it into an agle from the perspective of the camra
                float targetAngle = MathF.Atan2(_moveDirections.normalized.x, _moveDirections.normalized.z) * Mathf.Rad2Deg + _camWalk.transform.eulerAngles.y;
                //gives you the agile with nice smoth truning dampaning that the player needs to 
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmothVelocity, _turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                //the directions player is moving
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                _rb.linearVelocity = new Vector3(moveDir.x * _Speed * _moveDirections.magnitude, _rb.linearVelocity.y, moveDir.z * _Speed * _moveDirections.magnitude);

                //sets animation so they walk foard
                _animatorController.SetFloat("walkZ", _moveDirections.magnitude);

            }
            else
            {
                _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);
                _animatorController.SetFloat("walkZ", walkZ);
                _animatorController.SetFloat("walkX", walkX);
            }
        }

        #endregion
    }
}