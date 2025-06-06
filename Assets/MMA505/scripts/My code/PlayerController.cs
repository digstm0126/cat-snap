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
    
    
        [SerializeField] private PlayerMovement _MovementManager;
        [SerializeField] private PlayerCamControler _camracontroler;

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


            _Fps = _playerInputs.Player.Fps;
            _Fps.Enable();


            _Sprint = _playerInputs.Player.Sprint;
            _Sprint.Enable();

            _action1 = _playerInputs.Player.action1;
            _action1.Enable();
            _action1.performed += Action1;

            _Jump = _playerInputs.Player.Jump;
            _Jump.Enable();
            

        }
        private void ODisable()
        {
            _Move.Disable();
            _fight.Disable();
            _Fps.Disable();
            _Sprint.Disable();
            _Look.Disable();
            _action1.Disable();
            _Jump.Disable();

        }

        #region inputActions
        private void Action1(InputAction.CallbackContext context)
        {
            if (_MovementManager.GetPlayerState() == PlayerMovement.PlayerState.fps)
            {
                _camracontroler.TakePhoto();
            }
        }


        #endregion

    }
}