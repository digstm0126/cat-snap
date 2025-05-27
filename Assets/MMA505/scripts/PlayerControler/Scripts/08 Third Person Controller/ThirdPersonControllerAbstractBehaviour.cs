using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThirdPersonControllerAbstractBehaviour : MonoBehaviour
{
    public abstract bool ExecuteBehaviourIfRequired(ref string CurrentState);

}
