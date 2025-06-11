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
        [SerializeField] private float _intristUpdatTime = 1f;
        [SerializeField] private float _intristUpdatTimer;



        [SerializeField] private Intrist _mostIntristing;
        [SerializeField] private Intrist _mostScerry;
        [SerializeField] public List<Intrist> _intristingTings = new List<Intrist>();
        [SerializeField] private int _bravery;
        [SerializeField] private int _cureosity;
        [SerializeField] private int _bordem;
        [SerializeField] private float _speed = 3.5f;
        [SerializeField] private float _runSpeed = 7f;


//debprectated
        [SerializeField] private float _scearTime = 3;
        [SerializeField] private float _scearTimer;
        [SerializeField] private float _distAhead = 2;
        [SerializeField] private float _randomRadice = 5f;



        public enum CatStates { runing, interacting, moving, wondering };
        [SerializeField] private CatStates _currentState = CatStates.interacting;

        public enum CatIdelStat { mowing, siting, looking, Idel };
        [SerializeField] private CatIdelStat _idelCurrent;



        void Start()
        {
            GameObject Home = new GameObject("Home");
            Home.transform.position = transform.position;
            _intristingTings.Add(new Intrist(Home, this));
            _catAgint = GetComponent<NavMeshAgent>();
            SetNewIntrist();
            SetDestination();
            _catAgint.speed = _speed;

        }

        // Update is called once per frame
        void Update()
        {

            if (_intristUpdatTimer <= 0)
            {
                _intristUpdatTimer = _intristUpdatTime;
                AddIntristingThings();
                UpdateIntrists();
                UpdateMosts();

            }
            else
            {
                _intristUpdatTimer -= Time.deltaTime;
            }

            Move();
            UpdateCanvice();
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

        private void AddIntristingThings()
        {
            //gets all visable objects
            List<GameObject> visable = _combinedFrustum.GetVisableObjects();

            if (_intristingTings != null)
            {

                foreach (Intrist intrist in _intristingTings)
                {
                    //true if one of the viable objects are in frustom and in intrists
                    GameObject obj = intrist.GetGameObject();
                    if (visable.Contains(obj))
                    {
                        //remove if is already in intrists
                        visable.Remove(obj);

                    }
                }
            }
            foreach (GameObject obj in visable)
            {
                //adds all objects not in intrists into intrists
                _intristingTings.Add(new Intrist(obj, 5, 5));
            }
        }

        private void UpdateIntrists()
        {
            foreach (Intrist intrist in _intristingTings)
            {
                intrist.UpdateState(_combinedFrustum.GetVisableObjects());
            }
        }

        private void UpdateMosts()
        {
            foreach (Intrist intrist in _intristingTings)
            {
                if (intrist.GetFearValue() > _mostIntristing.GetIntristVal())
                {
                    _mostIntristing = intrist;
                }
                if (intrist.GetFearValue() > _mostScerry.GetFearValue())
                {
                    _mostScerry = intrist;
                }
            }
        }


        private void Intract()
        {

            
            if (_catAgint.velocity.magnitude <= 0.1f)
            {
                transform.LookAt(_target.GetLastLocation());
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
            if (Random.Range(_bordem, _cureosity) >= _cureosity)
            {
                //new intersting thing
                SetNewIntrist();
                SetDestination();
                _idelCurrent = CatIdelStat.Idel;
                Idel();
                _bordem = 0;
                _currentState = CatStates.moving;
            }
            int random = Random.Range(0, System.Enum.GetValues(typeof(CatIdelStat)).Length);
            _idelCurrent = (CatIdelStat)random;
        }

        private void Run()
        {
            if (_scearTimer > 0)
            {
                
                Vector3 target = transform.position + ((transform.position - _sceryThing.GetLastLocation()).normalized * _distAhead);
                Debug.DrawLine(transform.position, target, Color.red, 0.1f);
                Debug.DrawRay(target, Vector3.up * 0.5f, Color.blue, 0.1f);
                _catAgint.destination = target; //+ (_randomRadice * Random.insideUnitSphere);
                _scearTimer -= Time.deltaTime;
                _catAgint.speed = _runSpeed;

            }
            else
            {
                _catAgint.speed = _speed;
                _sceryThing = null;
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

                if (Vector3.Distance(transform.position, _target.GetLastLocation()) <= _randomRadice)
                {
                    _catIk.SwapIK(true);
                }
            }

            

        }

        [ContextMenu("New Random destination")]
        private void SetDestination()
        {


            if (_catAgint.pathPending || !_catAgint.isOnNavMesh || _catAgint.remainingDistance > 0.1f)
                return;

            _catAgint.destination = _target.GetLastLocation() + (_randomRadice * Random.insideUnitSphere);

        }

        [ContextMenu("newIntrist")]
        private void SetNewIntrist()
        {
            //chose a randome item form intrisintg things
            if (_intristingTings != null)
            {
                Intrist newIntrist = _intristingTings[Random.Range(0, _intristingTings.Count - 1)];
                foreach (Intrist intrist in _intristingTings)
                {
                    if (intrist.GetIntristVal() > newIntrist.GetIntristVal())
                    {
                        newIntrist = intrist;
                    }
                }
                _target = newIntrist;
                _catIk.SetTarget(_target.GetLastLocation());
            }
        }

        public CatStates getState()
        {
            return _currentState;
        }

        public Intrist GetTarget()
        {
            return _target;
        }

        public int GetBravery()
        {
            return _bravery;
        }

    }
}
