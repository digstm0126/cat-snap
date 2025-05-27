using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairInteractionHandler : MonoBehaviour
{
    public Transform WalkToAnchor;

    public List<GameObject> ObjectsInRange;

    private void OnMouseDown()
    {
        ThirdPersonSittingBehaviour sittingBehviour = FindObjectOfType<ThirdPersonSittingBehaviour>();
        if (sittingBehviour!=null)
            sittingBehviour.ToggleSitting(WalkToAnchor);

    }

    private void Update()
    {
        ThirdPersonSittingBehaviour sittingBehviour = FindObjectOfType<ThirdPersonSittingBehaviour>();
        if (sittingBehviour != null)
        {
            if (Input.GetKeyDown(KeyCode.Return) && ObjectsInRange.Contains(sittingBehviour.gameObject))
                sittingBehviour.ToggleSitting(WalkToAnchor);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ObjectsInRange.Contains(other.gameObject))
            ObjectsInRange.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (ObjectsInRange.Contains(other.gameObject))
            ObjectsInRange.Remove(other.gameObject);
    }
}
