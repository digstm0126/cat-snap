using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangableObject : MonoBehaviour
{
    public GameObject Anchor;

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonHangingBehaviour hangingBehaviour = ThirdPersonHangingBehaviour.Instance;
        if (hangingBehaviour != null)
        {
            if (hangingBehaviour.GrabbingColliders.Contains(other))
                hangingBehaviour.GrabEdge(Anchor.transform);
        }
    }
}
