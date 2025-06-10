using UnityEngine;
using System.Linq;

public class MemTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float resouses = Resources.FindObjectsOfTypeAll<Mesh>().Length;
        Debug.Log(resouses);
    }
}
