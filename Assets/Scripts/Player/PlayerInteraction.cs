using UnityEngine;
namespace Unobserved
{
    public sealed class PlayerInteraction : MonoBehaviour
    {
        public static string Prompt {get; private set;}
        void Update()
        {
            Prompt=""; if(!GameFlow.Playing) return;
            var eye=GameFlow.Instance.Player.eye;
            if(Physics.Raycast(eye.transform.position,eye.transform.forward,out var hit,3.3f,~(1<<8),QueryTriggerInteraction.Ignore))
            {
                var relay=hit.collider.GetComponentInParent<PuzzleRelay>();
                if(!relay) return; Prompt=relay.Prompt;
                if(Input.GetKeyDown(KeyCode.F)) relay.Activate();
            }
        }
    }
}
