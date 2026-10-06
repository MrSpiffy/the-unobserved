using UnityEngine;
namespace Unobserved
{
    public sealed class CircuitDoor : MonoBehaviour
    {
        public PuzzleRelay[] circuits;
        public Transform slab;
        public TextMesh display;
        public bool Open {get; private set;}
        Vector3 closed;
        void Start()=>closed=slab.localPosition;
        void Update()
        {
            if(!GameFlow.Playing) return;
            int ready=0; foreach(var relay in circuits) if(relay.Active) ready++;
            if(!Open && ready==circuits.Length) { Open=true; Soundscape.Instance.PlayAt(transform.position,SoundCue.Door,.6f); }
            slab.localPosition=Vector3.MoveTowards(slab.localPosition,closed+Vector3.up*(Open?5.2f:0),Time.deltaTime*1.9f);
            if(display) display.text=Open?"ROUTE RESTORED":"CIRCUITS  "+ready+" / "+circuits.Length;
        }
    }
}
