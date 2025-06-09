using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace MMA505
{
    public class CatControler : MonoBehaviour
    {
        [SerializeField] private CombinedFrustums _combinedFrustum;
        [SerializeField] private Animator _catAnimator;
        [SerializeField] private NavMeshAgent _catAgint;
        [SerializeField] private Color _collour;
        private Camera _mainCam;
        [SerializeField] private Canvas _canvice;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Rigidbody _rb;


        [SerializeField] private List<GameObject> _intristingTings;
        [SerializeField] private float _bordemLimt;
        [SerializeField] private float _bordemCurrent;
        [SerializeField] private GameObject _sceryThing;
        [SerializeField] private Vector3 _target;
        [SerializeField] private float _scearTime = 3;
        [SerializeField] private float _scearTimer;
        [SerializeField] private float _distAhead = 2;
        [SerializeField] private float _randomRadice = 5f;



        public enum CatStates {runing, interacting, moving};
        [SerializeField] private CatStates _currentState = CatStates.moving;


        void Start()
        {
            _catAgint = GetComponent<NavMeshAgent>();
            SetTargeint(transform.position + (transform.forward * _distAhead));
            SetDestination();
        }

        // Update is called once per frame
        void Update()
        {

            //Move();
            //SetRandomTarget(transform.position);
            _catAnimator.SetFloat("speed", _catAgint.velocity.magnitude);

        }

        private void Move()
        {
            switch (_currentState)
            {
                case CatStates.runing:
                    Run();
                    break;
                case CatStates.moving:
                    Walk();
                    break;
                case CatStates.interacting:
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
                    // _sceryThing = new Vector3(obj.transform.position.x, transform.position.y, obj.transform.position.z);

                    //SetRandomTarget(transform.position + (transform.position - _sceryThing).normalized * _distAhead);
                    _scearTimer = _scearTime;

                }
            }
        }

        private void Run()
        {
            if (_scearTimer > 0)
            {

                SetTargeint(_target + ((_target - transform.position).normalized * _distAhead));
                SetDestination();
                _scearTimer -= Time.deltaTime;
            
            }
            else
            {
                _scearTimer = 0;
                _currentState = CatStates.moving;
                UpdateCanvice();
            }
        }

        private void Walk()
        {
            
            CheckForPlayer();

        }

        [ContextMenu("New Random destination")]
        private void SetDestination()
        {


            if (_catAgint.pathPending || !_catAgint.isOnNavMesh || _catAgint.remainingDistance > 0.1f)
                return;
            
            _catAgint.destination = _target + (_randomRadice * Random.insideUnitSphere);;

        }

        private void SetTargeint(Vector3 targt)
        {
            float x = targt.x;
            float z = targt.z;
            float y = transform.position.y; // Assuming the same height as basespot

            _target = new Vector3(x, y, z);
        }
        
        [ContextMenu("newIntrist")]
        private void SetNewIntrist()
        {
            //chose a randome item form intrisintg things
            if (_intristingTings != null)
            {
                GameObject randomItem = _intristingTings[Random.Range(0, _intristingTings.Count - 1)];
                SetTargeint(randomItem.transform.position);
            }
        }

        public CatStates getState()
        {
            return _currentState;
        }

    }
}
