using UnityEngine;
using MMA505;
using System.Collections.Generic;
using UnityEditor.UIElements;
using System;
using UnityEngine.UIElements;

namespace MMA505
{
    public class GameManager : MonoBehaviour
    {

        //timer for updating regions
        private float timer = 0f;
        public float UpdateRegionsTimer = 0.5f;
        [SerializeField] private Material visibilitySphereMaterial;
        [SerializeField] private int _frustumLayer;
        [SerializeField] private Material _frustumMaterial;
        [SerializeField] private Camera _mainCam;
        [SerializeField] private GameObject _frustomTranform;
        [SerializeField] private GameObject[] _Cats;
        [SerializeField] private GameObject[] _Players;
        [SerializeField] private GameObject[] _Portals;
        [SerializeField] private GameObject[] _OcludingObjects;
        [SerializeField] private GameObject[] _RegionsIn;
        [SerializeField] private GameObject _RegionEx;
        [SerializeField] private List<Photo> _phtoGoals = new List<Photo>();

        public static event Action<List<Photo>> UpdateUi;




        #region Initialisation & Destruction

        private void Awake()
        {
            PlayerCamControler.OnPhotoTake += CheckPhoto;
        }

        private void OnDestroy()
        {
            PlayerCamControler.OnPhotoTake -= CheckPhoto;
        }

        #endregion

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            populatObjects();
            RegionManager.UpdateVisabl(_Players, _Cats, _RegionsIn);
            RegionManager.UpdatePortals(_Portals, _RegionsIn);
            SetupCats();
            GenGoals(3, 8, 20);
            UpdateUi?.Invoke(_phtoGoals);
        }

        public GameObject getFrustomTransfom()
        {
            return _frustomTranform;
        }

        public int getFrustumLayer()
        {
            return _frustumLayer;
        }

        public Material getFrustumMaterial()
        {
            return _frustumMaterial;
        }

        public GameObject[] getOcludingObjects()
        {
            return _OcludingObjects;
        }

        public void populatObjects()
        {
            _Players = GameObject.FindGameObjectsWithTag("Player");
            addVisibilitySphere(_Players);
            _Cats = GameObject.FindGameObjectsWithTag("Cat");
            addVisibilitySphere(_Cats);
            _Portals = GameObject.FindGameObjectsWithTag("portal");
            _OcludingObjects = GameObject.FindGameObjectsWithTag("ocluding");

            List<GameObject> regions = new List<GameObject>();
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("region"))
            {
                //going to be determing if its in or out
                if (obj.GetComponent<InsideRegion>() != null)
                {
                    regions.Add(obj);
                }
                else
                {
                    _RegionEx = obj;
                }
            }


            _RegionsIn = regions.ToArray();
        }

        public GameObject[] getAllRegions()
        {
            List<GameObject> allRegions = new List<GameObject>(_RegionsIn);
            //allRegions.Add(_RegionEx);
            return allRegions.ToArray();
        }

        //add a sphere site colider to each object in objToTest for visablity
        public void addVisibilitySphere(GameObject[] siteObjectsToCheck)
        {

            // Loop through each game object to add a visablity sphere
            for (int i = 0; i < siteObjectsToCheck.Length; i++)
            {

                //dont need this part as i just wat refrince to the game object
                // If the game object already has a visibility sphere
                if (siteObjectsToCheck[i].transform.Find("VisibilitySphere") == null)
                {
                    // Create a new sphere
                    GameObject visibilitySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    // Set the gameobjects name to "VisibilitySphere"
                    visibilitySphere.name = "VisibilitySphere";
                    // Set it's parent as the game object
                    visibilitySphere.transform.SetParent(siteObjectsToCheck[i].transform, false);
                    // Set it's size to the the game objects bounding box's extent magnitude * 2
                    // (the magnitude is the half the size)
                    Renderer renderer = siteObjectsToCheck[i].GetComponent<Renderer>();
                    Vector3 boundsExtents = renderer.bounds.extents;
                    visibilitySphere.transform.localScale = Vector3.one * boundsExtents.magnitude * 2;
                    // Get the sphere's Renderer and update the material to our visibility sphere material, and turn off shadows
                    Renderer rendererSphere = visibilitySphere.GetComponent<Renderer>();
                    renderer.material = visibilitySphereMaterial;
                    rendererSphere.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    rendererSphere.receiveShadows = false;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

            timer += Time.deltaTime;
            if (timer >= UpdateRegionsTimer)
            {
                RegionManager.UpdateVisabl(_Players, _Cats, _RegionsIn);
                timer = 0f;
            }

        }

        public void CheckPhoto(Photo _photoIn)
        {
            int finished = 0;

            foreach (Photo _Photo in _phtoGoals)
            {
                // Check if the photo goal has not been found yet
                if (_Photo.GetFoundPhoto() == null)
                {
                    // Check if the distance is less than the goal's distance
                    if (_photoIn.GetDist() < _Photo.GetDist())
                    {
                        // Check if the object and state match the goal
                        if (_photoIn.GetObject() == _Photo.GetObject() && _photoIn.GetState() == _Photo.GetState())
                        {
                            finished++;
                            // Mark the goal as completed
                            _Photo.SetFoundPhoto(_photoIn);
                            // Update the UI
                            UpdateUi?.Invoke(_phtoGoals);

                        }
                    }
                }
                else
                {
                    finished++;
                }
            }

            // Check if all photo goals are completed
            if (finished >= _phtoGoals.Count)
            {
                Debug.Log("All photo goals completed!");
                //regen goals?
            }
        }

        public void GenGoals(float _amout, int _distMin, int _distMax)
        {
            _phtoGoals.Clear();
            for (int i = 0; i < _amout; i++)
            {
                GameObject _cat = _Cats[UnityEngine.Random.Range(0, _Cats.Length - 1)];
                int _dist = UnityEngine.Random.Range(_distMin, _distMax);
                CatControler.CatStates _state = (CatControler.CatStates)UnityEngine.Random.Range(0, Enum.GetValues(typeof(CatControler.CatStates)).Length);

                _phtoGoals.Add(new Photo(_cat, _dist, _state));
            }
        }

        public void SetupCats()
        {
            foreach (GameObject cat in _Cats)
            {
                cat.GetComponent<CatControler>().SetupCat(_mainCam);
            }
        }
    }
}