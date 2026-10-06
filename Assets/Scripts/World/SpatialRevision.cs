using UnityEngine;
namespace Unobserved
{
    // Deliberately authored replacements, not a repeating global polygon cycle.
    // Test the union of all versions, and do not replace space occupied by the player.
    public sealed class SpatialRevision : MonoBehaviour
    {
        public GameObject[] versions;
        public Bounds localEnvelope;
        public int AppliedChanges {get;private set;}
        public int Current {get;private set;}
        bool seen;
        ObservationTarget observation;
        public Bounds WorldEnvelope
        {
            get
            {
                var e=localEnvelope.extents;
                var x=transform.TransformVector(e.x,0,0); var y=transform.TransformVector(0,e.y,0); var z=transform.TransformVector(0,0,e.z);
                return new Bounds(transform.TransformPoint(localEnvelope.center),new Vector3(Mathf.Abs(x.x)+Mathf.Abs(y.x)+Mathf.Abs(z.x),Mathf.Abs(x.y)+Mathf.Abs(y.y)+Mathf.Abs(z.y),Mathf.Abs(x.z)+Mathf.Abs(y.z)+Mathf.Abs(z.z))*2);
            }
        }
        public void Initialize(GameObject[] sets,Bounds envelope,int initial=0)
        {
            // Include the real mesh bounds of every future version, not just the shell.
            foreach(var set in sets) foreach(var r in set.GetComponentsInChildren<Renderer>(true))
            {
                var b=r.bounds;
                for(int c=0;c<8;c++) envelope.Encapsulate(transform.InverseTransformPoint(b.center+Vector3.Scale(b.extents,new Vector3((c&1)==0?-1:1,(c&2)==0?-1:1,(c&4)==0?-1:1))));
            }
            versions=sets; localEnvelope=envelope; Current=initial;
            observation=gameObject.AddComponent<ObservationTarget>(); observation.center=envelope.center; observation.size=envelope.size;
            for(int i=0;i<sets.Length;i++) versions[i].SetActive(i==Current);
        }
        void LateUpdate()
        {
            if(!GameFlow.Playing || AppliedChanges>=versions.Length-1) return;
            var eye=GameFlow.Instance.Player.eye;
            Bounds envelope=WorldEnvelope;
            if(ObservationManager.Instance.MaySee(envelope,transform,false)) { if(observation.Visible) seen=true; return; }
            var safety=envelope; safety.Expand(1);
            if(!seen || safety.Contains(GameFlow.Instance.Player.transform.position+Vector3.up)) return;
            seen=false; versions[Current].SetActive(false); Current++; versions[Current].SetActive(true); AppliedChanges++;
            GetComponentInParent<EnvironmentThemeController>().RequestShift();
        }
    }
}
