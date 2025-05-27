using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonController))]
public class ThirdPersonClimbingBehaviour : ThirdPersonShimmyingBehaviour
{
    protected class ClimbingState: ShimmyingState
    {
        public const int Climbing = 4;

        public new static string ToString(int hangingState)
        {
            switch (hangingState)
            {
                case NotHanging:
                    return "Not Hanging";

                case Hanging:
                    return "Hanging";

                case Dropping:
                    return "Dropping";

                case Shimmying:
                    return "Shimmying";

                case Climbing:
                    return "Climbing";

                default:
                    return "Unknown";
            }
        }
    };

    public override bool ExecuteBehaviourIfRequired(ref string CurrentState)
    {
        if (hangingState == ClimbingState.Climbing)
        {
            // Do Nothing
            CurrentState = ClimbingState.ToString(hangingState);
            return true;
        }
        else if (hangingState == HangingState.Hanging)
        {
            ExecuteHangingState();

            CurrentState = ClimbingState.ToString(hangingState);
            return true;
        }

        return base.ExecuteBehaviourIfRequired(ref CurrentState);
    }

    protected override void ExecuteHangingState()
    {
        base.ExecuteHangingState();

        if (Input.GetAxis("Vertical") > 0)
        {
            ThirdPersonController.Instance.animatorController.SetTrigger("Climb");
            hangAnchorTransform = null;
            hangingState = ClimbingState.Climbing;
        }
    }

    public void StandingUp()
    {
        hangingState = HangingState.NotHanging;
        this.GetComponent<Rigidbody>().isKinematic = false;
        hangAnchorTransform = null;
    }
}
