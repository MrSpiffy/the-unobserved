using UnityEngine;

namespace Unobserved
{
    public sealed class Progression : MonoBehaviour
    {
        public int number;
        public string section;
        public Vector3 checkpoint;
        public int capacity;
        public float checkpointYaw;
        bool entered;
        void OnTriggerEnter(Collider other)
        {
            if(entered || !other.GetComponent<FirstPersonController>()) return;
            entered=true;
            GameFlow.Instance.Player.Checkpoint=checkpoint;
            GameFlow.Instance.Player.CheckpointYaw=checkpointYaw;
            if(capacity > GameFlow.Instance.Anchors.Capacity)
            {
                GameFlow.Instance.Anchors.Upgrade(capacity);
                GameFlow.Instance.Toast(capacity==1 ? "Anchor acquired · E to hold a form · R to release" : "Second anchor acquired · Two forms can now be held",7);
            }
            GameFlow.Instance.EnterSection(number,section);
        }
    }

}
