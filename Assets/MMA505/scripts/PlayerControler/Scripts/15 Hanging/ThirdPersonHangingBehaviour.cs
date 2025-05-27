using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonController))]

public class ThirdPersonHangingBehaviour : ThirdPersonControllerAbstractBehaviour
{
    protected static ThirdPersonHangingBehaviour _Instance = null;
    public static ThirdPersonHangingBehaviour Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<ThirdPersonHangingBehaviour>();

            return _Instance;
        }
    }

    public List<Collider> GrabbingColliders;

    protected class HangingState
    {
        public const int NotHanging = 0;
        public const int Hanging = 1;
        public const int Dropping = 2;

        public static string ToString(int hangingState)
        {
            switch (hangingState)
            {
                case NotHanging:
                    return "Not Hanging";

                case Hanging:
                    return "Hanging";

                case Dropping:
                    return "Dropping";

                default:
                    return "Unknown";
            }
        }
    }

    protected int hangingState = HangingState.NotHanging;

    GameObject RayCastPlane;
    public Material RayCastMaterial;

    public float lerpSpeed = 5.0f;
    public float minLerpDistanceToObject = 0.1f;

    public bool DrawDebugRaycastPlane = true;

    public Transform hangAnchorTransform = null;

    public override bool ExecuteBehaviourIfRequired(ref string CurrentState)
    {
        if (hangingState == HangingState.Dropping)
        {
            // Do Nothing
            CurrentState = HangingState.ToString(hangingState);
            return true;
        }
        else if (hangingState == HangingState.Hanging)
        {
            ExecuteHangingState();

            CurrentState = HangingState.ToString(hangingState);
            return true;
        }

        return false;
    }

    protected virtual void ExecuteHangingState()
    {
        ThirdPersonController.Instance.animatorController.SetBool("isWalking", false);
        ThirdPersonController.Instance.animatorController.SetFloat("WalkSpeedMultiplier", 0);

        LerpToAnchor();

        if (Input.GetKeyDown(KeyCode.Space))
            Drop();
    }

    public virtual void Drop()
    {
        ThirdPersonController.Instance.animatorController.SetTrigger("Drop");
        ThirdPersonController.Instance.GetComponent<Rigidbody>().isKinematic = false;
        hangAnchorTransform = null;
        hangingState = HangingState.Dropping;
    }

    protected virtual void LerpToAnchor()
    {
        if (!hangAnchorTransform) return;

        if (Vector3.Distance(this.transform.position, hangAnchorTransform.position) > minLerpDistanceToObject)
        {
            this.transform.rotation = Quaternion.Lerp(transform.rotation, hangAnchorTransform.rotation, Time.deltaTime * lerpSpeed);
            this.transform.position = Vector3.Lerp(transform.position, hangAnchorTransform.position, Time.deltaTime * lerpSpeed);
        }
        else
        {
            this.transform.rotation = hangAnchorTransform.rotation;
            this.transform.position = hangAnchorTransform.position;
        }
    }

    public void GrabEdge(Transform target)
    {
        if (hangingState != HangingState.NotHanging) return;
        ThirdPersonController.Instance.animatorController.SetTrigger("GrabEdge");
        this.GetComponent<Rigidbody>().isKinematic = true;
        hangingState = HangingState.Hanging;
        hangAnchorTransform = target;

        UpdateHangableAnchor();
    }

    protected void UpdateHangableAnchor()
    {
        if (hangAnchorTransform == null) return;

        Plane rootPlane = new Plane(hangAnchorTransform.position, hangAnchorTransform.position + hangAnchorTransform.right, hangAnchorTransform.position + hangAnchorTransform.up);

        Vector3 adjustedPos = new Vector3(this.transform.position.x, hangAnchorTransform.position.y, this.transform.position.z);

        Ray ray = new Ray(adjustedPos - (hangAnchorTransform.forward / 2), hangAnchorTransform.forward);
        float rayDistance;
        if (rootPlane.Raycast(ray, out rayDistance))
        {
            hangAnchorTransform.position = ray.GetPoint(rayDistance);
            transform.position = hangAnchorTransform.position;
            transform.rotation = hangAnchorTransform.rotation;
        }

        if (DrawDebugRaycastPlane)
            StartCoroutine(DrawRayCast(hangAnchorTransform, ray));
    }

    IEnumerator DrawRayCast(Transform transform, Ray ray)
    {
        if (RayCastMaterial == null) yield break;

        if (RayCastPlane == null)
        {
            RayCastPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
            RayCastPlane.GetComponent<Renderer>().material = RayCastMaterial;
            RayCastPlane.name = "Hangable Raycast Quad";
        }

        RayCastPlane.transform.position = hangAnchorTransform.position;
        RayCastPlane.transform.rotation = Quaternion.LookRotation(hangAnchorTransform.forward, hangAnchorTransform.up);
        RayCastPlane.transform.localScale = Vector3.one * 100;
        RayCastPlane.SetActive(true);

        Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);
        yield return new WaitForSeconds(5);
        Destroy(RayCastPlane);
    }

    void Dropped()
    {
        hangingState = HangingState.NotHanging;
    }
}
