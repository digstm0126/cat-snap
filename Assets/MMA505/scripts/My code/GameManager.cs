using UnityEngine;
using MMA505;
using System.Collections.Generic;

namespace MMA505
{
    public class GameManager : MonoBehaviour
    {
        public GameObject[] regions;

        //timer for updating regions
        private float timer = 0f;
        public float UpdateRegionsTimer = 0.5f;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

            PopluateRegions();

            UpdateRegions();


        }

        // Update is called once per frame
        void Update()
        {

            timer += Time.deltaTime;
            if (timer >= UpdateRegionsTimer)
            {
                UpdateRegions();
                timer = 0f;
            }
            
        }


        //popluats with all regions in the sence
        void PopluateRegions()
        {

            regions = GameObject.FindGameObjectsWithTag("region");

        }


        //update each regeions visable objects
        void UpdateRegions()
        {
            foreach (GameObject region in regions)
            {
                region.GetComponent<Region>().populatObjectsToTest();
            }
        }
    }
}