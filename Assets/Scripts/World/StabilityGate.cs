using UnityEngine;
namespace Unobserved
{
    public sealed class StabilityGate : MonoBehaviour
    {
        public UnstableEntity[] required;
        public Transform door;
        public TextMesh readout;
        public bool final;
        public bool Open { get; private set; }
        Vector3 closed;
        void Start() => closed=door.position;
        void Update()
        {
            if(!GameFlow.Playing) return;
            if(!Open)
            {
                bool valid=true;
                foreach(var e in required) if(!e.Anchored || !e.Traversable) valid=false;
                if(valid)
                {
                    Open=true; Soundscape.Instance.PlayAt(transform.position,SoundCue.Door);
                    readout.text="CONTINUITY VERIFIED\nPROCEED";
                }
            }
            door.position=Vector3.MoveTowards(door.position,closed+Vector3.up*(Open?5:0),Time.deltaTime*2);
        }
    }

}
