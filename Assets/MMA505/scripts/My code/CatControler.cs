using System.Collections.Generic;
using MMA505;
using TMPro;
using UnityEngine;

namespace MMA505
{
    public class CatControler : MonoBehaviour
    {
        [SerializeField] private CombinedFrustums _combinedFrustum;
        [SerializeField] private Vector3 _secrrySpot;
        [SerializeField] private Vector3 _target;
        [SerializeField] private float _scearTime = 3;
        [SerializeField] private float _scearTimer;
        [SerializeField] private float _distAhead = 2;
        [SerializeField] private float _switchDist = 1;
        [SerializeField] private float _randomRadice = 5f;
        [SerializeField] private float _walkSpeed = 1f;
        [SerializeField] private float _runSpeed = 2f;
        [SerializeField] private Color _collour;
        private Camera _mainCam;
        [SerializeField] private Canvas _canvice;
        [SerializeField] private TextMeshProUGUI _text;

        [SerializeField] private Rigidbody _rb;

        public enum CatStates { runing, walking };
        [SerializeField] private CatStates _currentState = CatStates.walking;




        // Update is called once per frame
        void Update()
        {

            Move();
            DeBugTarget();

        }

        private void Move()
        {
            switch (_currentState)
            {
                case CatStates.runing:
                    Run();
                    break;
                case CatStates.walking:
                    Walk();
                    break;

            }
        }

        public void SetupCat(Camera mainCam)
        {
            _mainCam = mainCam;
            SetupCanvice();
        }

        private void SetupCanvice()
        {
            _canvice.worldCamera = _mainCam;
            _text.color = _collour;
            _text.text = $"{name} is {_currentState}";
        }

        private void UpdateCanvice()
        {
            _text.text = $"{name} is {_currentState}";
        }

        private void CheckForPlayer()
        {
            List<GameObject> objects = _combinedFrustum.GetVisableObjects();
            foreach (GameObject obj in objects)
            {
                if (obj.tag == "Player")
                {
                    _currentState = CatStates.runing;
                    UpdateCanvice();
                    _secrrySpot = new Vector3(obj.transform.position.x, transform.position.y, obj.transform.position.z);

                    SetRandomTarget(transform.position + (transform.position - _secrrySpot).normalized * _distAhead);
                    _scearTimer = _scearTime;
                    Debug.Log("player");
                }
            }
        }

        private void Run()
        {
            if (_scearTimer > 0)
            {
                if (Vector3.Distance(_target, transform.position) <= _switchDist)
                {
                    SetRandomTarget(_target + ((_target - transform.position).normalized * _distAhead));
                }
                _scearTimer -= Time.deltaTime;
                Vector3 direction = _target - transform.position;
                direction.y = 0; // Ignore y-axis rotation
                _rb.transform.rotation = Quaternion.LookRotation(direction);
                _rb.AddForce(_rb.transform.forward * _runSpeed, ForceMode.VelocityChange);
            }
            else
            {
                _scearTimer = 0;
                _currentState = CatStates.walking;
                UpdateCanvice();
            }
        }

        private void Walk()
        {

            if (Vector3.Distance(_target, transform.position) <= _switchDist)
            {
                SetRandomTarget(_target + ((_target - transform.position).normalized * _distAhead ));
            }

            Vector3 direction = _target - transform.position;
            direction.y = 0; // Ignore y-axis rotation
            _rb.transform.rotation = Quaternion.LookRotation(direction);
            _rb.AddForce(_rb.transform.forward * _walkSpeed, ForceMode.VelocityChange);
            CheckForPlayer();

        }

        private void SetRandomTarget(Vector3 basespot)
        {

            float angle = Random.Range(0f, Mathf.PI * 2);
            float x = basespot.x + Mathf.Cos(angle) * _randomRadice;
            float z = basespot.z + Mathf.Sin(angle) * _randomRadice;
            float y = transform.position.y; // Assuming the same height as basespot

            _target = new Vector3(x, y, z);
            
        }

        public CatStates getState()
        {
            return _currentState;
        }

        private void DeBugTarget()
        {
            // Debug solid sphere at target position
            Debug.DrawLine(_target, _target + Vector3.up * 0.1f, Color.red, 0.1f); // Small solid sphere representation

            // Debug transparent sphere with radius of _randomRadice
            for (float angle = 0; angle < Mathf.PI * 2; angle += Mathf.PI / 16)
            {
                Vector3 point1 = _target + new Vector3(Mathf.Cos(angle) * _randomRadice, 0, Mathf.Sin(angle) * _randomRadice);
                Vector3 point2 = _target + new Vector3(Mathf.Cos(angle + Mathf.PI / 16) * _randomRadice, 0, Mathf.Sin(angle + Mathf.PI / 16) * _randomRadice);
                Debug.DrawLine(point1, point2, Color.blue, 0.1f); // Transparent sphere representation
            }
        }
    }
}
