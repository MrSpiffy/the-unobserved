using UnityEngine;
namespace Unobserved
{
    public sealed class ExitTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if(!other.GetComponent<FirstPersonController>()) return;
            if(GameFlow.Instance.ConnectedCircuits>=17) GameFlow.Instance.Finish();
            else GameFlow.Instance.Toast("The departure line is incomplete. Restore the remaining circuits.");
        }
    }
}
