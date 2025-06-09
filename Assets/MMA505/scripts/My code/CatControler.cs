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
        [SerializeField] private IkCatHead _catIk;
        [SerializeField] private Color _collour;
        private Camera _mainCam;
        [SerializeField] private Canvas _canvice;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Rigidbody _rb;


        [SerializeField] private List<GameObject> _intristingTings;
        [SerializeField] private int _bordemLimt = 10;
        [SerializeField] private int _bordemCurrent;
        [SerializeField] private float _bordemSecsTill = 1;
        [SerializeField] private float _bordTimer;
        [SerializeField] private GameObject _sceryThing;
        [SerializeField] private Vector3 _target;
        [SerializeField] private float _scearTime = 3;
        [SerializeField] private float _scearTimer;
        [SerializeField] private float _distAhead = 2;
        [SerializeField] private float _randomRadice = 5f;



        public enum CatStates {runing, interacting, moving, wondering};
        [SerializeField] private CatStates _currentState = CatStates.interacting;

        public enum CatIdelStat {mowing, siting, looking, Idel};
        [SerializeField] private CatIdelStat _idelCurrent;




        
        void Start()
        {
            _catAgint = GetComponent<NavMeshAgent>();
            SetNewIntrist();
            SetDestination();
        }

        // Update is called once per frame
        void Update()
        {
            CheckForPlayer();
            Move();
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
                    moving();
                    break;
                case CatStates.interacting:
                    Intract();
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

        private void Intract()
        {

            if (_catAgint.velocity.magnitude <= 0.1f)
            {
                transform.LookAt(_target);
                _bordTimer += Time.deltaTime;
                if (_bordTimer >= _bordemSecsTill)
                {
                    _bordTimer = 0;
                    _bordemCurrent += Random.Range(1, 2);
                    SwitchIdel();
                }
                IdelAnimation();
            }
            
        }


        private void IdelAnimation()
        {
            switch (_idelCurrent)
            {
                case CatIdelStat.siting:
                    Sit();
                    break;
                case CatIdelStat.mowing:
                    Mowing();
                    break;
                case CatIdelStat.looking:
                    Looking();
                    break;
                case CatIdelStat.Idel:
                    Idel();
                    break;
                default:
                    Idel();
                    break;
            }
        }

        private void Looking()
        {
            _catAnimator.SetBool("siting", false);
            _catAnimator.SetBool("Looking", false);
            _catAnimator.SetBool("mowing", false);
        }

        private void Sit()
        {
            _catAnimator.SetBool("siting", true);
            _catAnimator.SetBool("Looking", false);
            _catAnimator.SetBool("mowing", false);

        }

        private void Mowing()
        {
            _catAnimator.SetBool("siting", false);
            _catAnimator.SetBool("Looking", false);
            _catAnimator.SetBool("mowing", true);
        }

        private void Idel()
        {
            _catAnimator.SetBool("siting", false);
            _catAnimator.SetBool("Looking", false);
            _catAnimator.SetBool("mowing", false);
        }

        private void SwitchIdel()
        {
            if (Random.Range(_bordemCurrent, _bordemLimt) >= _bordemLimt)
            {
                //new intersting thing
                SetNewIntrist();
                SetDestination();
                _idelCurrent = CatIdelStat.Idel;
                Idel();
                _bordemCurrent = 0;
                _currentState = CatStates.moving;
            }
            int random = Random.Range(0, System.Enum.GetValues(typeof(CatIdelStat)).Length);
            _idelCurrent = (CatIdelStat)random;
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

        private void moving()
        {

            if (_catAgint.pathPending || !_catAgint.isOnNavMesh || _catAgint.remainingDistance > 0.1f)
            {
                //if there
                _currentState = CatStates.interacting;
            }
            else
            {
                
                if (Vector3.Distance(transform.position, _target) <= _randomRadice)
                {
                    _catIk.SwapIK(true);
                }
            }
            
            CheckForPlayer();

        }

        [ContextMenu("New Random destination")]
        private void SetDestination()
        {


            if (_catAgint.pathPending || !_catAgint.isOnNavMesh || _catAgint.remainingDistance > 0.1f)
                return;
            
            _catAgint.destination = _target + (_randomRadice * Random.insideUnitSphere);

        }

        private void SetTargeint(Vector3 targt)
        {
            float x = targt.x;
            float z = targt.z;
            float y = transform.position.y; // Assuming the same height as basespot

            _target = new Vector3(x, y, z);
            _catIk.SetTarget(_target);
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
