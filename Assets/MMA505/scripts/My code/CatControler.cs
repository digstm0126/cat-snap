using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace MMA505
{
    public class CatControler : MonoBehaviour
    {
        #region baseThing
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
        #endregion


        #region funThings
        [SerializeField] private Intrist _mostIntristing;
        [SerializeField] private Intrist _mostScerry;
        [SerializeField] public List<Intrist> _intristingTings = new List<Intrist>();
        [SerializeField, Range(0, 5)] private int _bravery;
        [SerializeField, Range(0, 5)] private int _cureosity;
        [SerializeField, Range(0, 5)] private int _consentration;
        [SerializeField] private int _bordem;
        [SerializeField] private bool _bordemSwitch;
        [SerializeField] private float _speed = 3.5f;
        [SerializeField] private float _runSpeed = 7f;
        [SerializeField] private float _OcludingObjects;
        [SerializeField] private float _atentionDist = 2;
        [SerializeField] private GameObject _wonderDestionation;
        #endregion




        public enum CatStates { runing, interacting, moving, wondering };
        [SerializeField] private CatStates _currentState = CatStates.interacting;

        public enum CatIdelStat { mowing, siting, looking, Idel };
        [SerializeField] private CatIdelStat _idelCurrent;



        void Start()
        {

            
            _wonderDestionation =  new GameObject("wonderDestionation");
            _wonderDestionation.transform.position = transform.position;
            _catAgint = GetComponent<NavMeshAgent>();
            _catAgint.speed = _speed;
            
        }

        void Update()
        {


            
            if (_intristUpdatTimer <= 0)
            {
                _intristUpdatTimer = _intristUpdatTime;
                AddIntristingThings();
                Move();
                AddIntristingThings();
                UpdateIntrists();
                UpdateMosts();
                Desition();

            }
            else
            {
                _intristUpdatTimer -= Time.deltaTime;
            }

            UpdateCanvice();
            _catAnimator.SetFloat("speed", _catAgint.velocity.magnitude);

        }

        private void Desition()
        {
            //make destion

            //get proritys
            int[] Proity = new int[4];
            int intristPrority = Mathf.Clamp(_mostIntristing.GetIntristVal() - _cureosity, 0, 10);
            Proity[0] = intristPrority;

            int fareProity = Mathf.Clamp(_mostScerry.GetFearValue() - _bravery, 0, 10);
            Proity[1] = fareProity;

            int bordemProity = Mathf.Clamp(_bordem - _cureosity, 0, 10);
            Proity[2] = bordemProity;

            int wonderProity = Mathf.Clamp(2, 0, 10); // Default priority for wondering
            Proity[3] = wonderProity;
            
            // Determine the highest priority
            int maxPriorityIndex = System.Array.IndexOf(Proity, Mathf.Max(Proity));
            Debug.Log($"intrist = {intristPrority} fare = {fareProity} bordem = {bordemProity} wonder = {wonderProity} maxproity = {maxPriorityIndex}");
            // Set the state based on the highest priority
            switch (maxPriorityIndex)
            {
                //intrist
                case 0:
                    _currentState = CatStates.interacting;
                    break;
                //fare
                case 1:
                    _currentState = CatStates.runing;
                    break;
                //bordem
                case 2:
                    //switch sub stare
                    _bordemSwitch = true;
                    break;
                //wonder
                case 3:
                    _currentState = CatStates.wondering;
                    break;
            }
            
        }

        private void Move()
        {
            switch (_currentState)
            {
                case CatStates.runing:
                    Run();
                    break;
                case CatStates.interacting:
                    Intract();
                    break;
                case CatStates.wondering:
                    Wonder();
                    break;
                case CatStates.moving:
                    //moving();
                    Debug.Log($"no move anymore");
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

        //adds any items in viwe to intrint things if not already there
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
                if (!(obj == this))
                {
                    _intristingTings.Add(new Intrist(obj, 5, 5));  
                }
            }
        }

        //updates the info each intrist holds
        private void UpdateIntrists()
        {
            List<GameObject> visableThings = _combinedFrustum.GetVisableObjects();
            foreach (Intrist intrist in _intristingTings)
            {
                intrist.UpdateState(visableThings);
            }
        }

        //updates the intrist that is the most intristing and the most scerry one
        private void UpdateMosts()
        {
            foreach (Intrist intrist in _intristingTings)
            {
                if (intrist.GetIntristVal() > _mostIntristing.GetIntristVal())
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
            // moves to most intristing thing random location around it
            // once there will intract with it each time this happens its intrist in it gose down 
            // 
            if (_catAgint.velocity.magnitude <= 0.1f && Vector3.Distance(_mostIntristing.GetLastLocation(), transform.position) < _atentionDist)
            {
                // do one of the intract ideels 
                if (_bordemSwitch)
                {
                    _bordemSwitch = false;
                    //switch intracty thing
                    SwitchIdel();
                }

                //look at where it thinks it is 
                transform.LookAt(_mostIntristing.GetLastLocation());


                // if its in site reduce intrist and do idel 
                if (_mostIntristing.GetInsite())
                {
                    // amount to reduce intrist by 2-10
                    int intristReduction = 2;
                    IdelAnimation();
                    //stops all animations
                    Idel();
                    _mostIntristing.SetIntrist(_mostIntristing.GetIntristVal() - intristReduction);
                }
                else
                {
                    _mostIntristing.SetIntrist(0);
                }
                

            }
            else
            {
                // set destionation and move with random lcation around it.
                _catAgint.destination = _mostIntristing.GetLastLocation();
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
            //switches to a random intract idel
            int random = UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(CatIdelStat)).Length);
            _idelCurrent = (CatIdelStat)random;

        }


        private void Run()
        {
            // runs away from target untill sceryist things fare value is less then cats bravery value

            //check if fare is below bravery of cat
            if (_mostScerry.GetFearValue() >= _bravery)
            {

                // do sub state stuff here
                // state 1 runing
                Vector3 target = transform.position + ((transform.position - _mostScerry.GetLastLocation()).normalized * _atentionDist);
                Debug.DrawLine(transform.position, target, Color.red, 0.1f);
                Debug.DrawRay(target, Vector3.up * 0.5f, Color.blue, 0.1f);
                _catAgint.destination = target;
                _catAgint.speed = _runSpeed;

                //stare 2 looking for scery things


            }

        }

        private void Wonder()
        {



            // if at destination sett a new one
            if (_catAgint.remainingDistance > 0.1f || !_catAgint.pathPending || !_catAgint.isOnNavMesh)
            {
                //set new destionation 
                _wonderDestionation.transform.position = _wonderDestionation.transform.position + (_atentionDist * UnityEngine.Random.insideUnitSphere);
                _wonderDestionation.transform.position = new Vector3(_wonderDestionation.transform.position.x, transform.position.y, _wonderDestionation.transform.position.z);
                _catAgint.destination = _wonderDestionation.transform.position;
            }
            
        }
        
        // private void moving()
        // {
        //     if (_catAgint.pathPending || !_catAgint.isOnNavMesh || _catAgint.remainingDistance > 0.1f)
        //     {
        //         //if there
        //         _currentState = CatStates.interacting;
        //     }
        //     else
        //     {

        //         if (Vector3.Distance(transform.position, _target.GetLastLocation()) <= _randomRadice)
        //         {
        //             _catIk.SwapIK(true);
        //         }
        //     }
        // }

        // [ContextMenu("New Random destination")]

        // [ContextMenu("newIntrist")]


        public CatStates getState()
        {
            return _currentState;
        }

        public Intrist GetTarget()
        {
            return _mostIntristing;
        }

        public int GetBravery()
        {
            return _bravery;
        }

    }
}
