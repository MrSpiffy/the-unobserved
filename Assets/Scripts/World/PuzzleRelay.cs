using UnityEngine;
namespace Unobserved
{
    [System.Serializable]
    public struct FormRequirement
    {
        public UnstableEntity entity;
        public Form form;
        public bool held;
        public bool Met => entity && entity.CurrentForm==form && (!held || entity.Anchored);
        public string Description => entity.label+" / "+form.ToString().ToUpperInvariant()+(held?" / HELD":"");
    }
    [System.Serializable]
    public sealed class OpticalCondition
    {
        public Vector3 from,to;
        public UnstableEntity subject;
        public bool interrupted;
        public string clue;
        public bool Met
        {
            get
            {
                var delta=to-from;
                bool hit=Physics.Raycast(from,delta.normalized,out var result,delta.magnitude,~((1<<8)|(1<<2)),QueryTriggerInteraction.Ignore);
                return interrupted ? hit && result.collider.GetComponentInParent<UnstableEntity>()==subject : !hit;
            }
        }
    }
    public sealed class PuzzleRelay : MonoBehaviour
    {
        public string label;
        public FormRequirement[] requirements=new FormRequirement[0];
        public PuzzleRelay prerequisite;
        public OpticalCondition[] optics=new OpticalCondition[0];
        public string clue="ACCESS THE CONTROL";
        float nextStatus;
        LineRenderer[] paths;
        public int upgrade;
        public TextMesh display;
        public Renderer lamp;
        public bool Active {get; private set;}
        public bool Ready
        {
            get { if(prerequisite && !prerequisite.Active) return false; foreach(var r in requirements) if(!r.Met) return false; foreach(var ray in optics) if(!ray.Met) return false; return true; }
        }
        public string Prompt => Active ? label+" · CONNECTED" : "F  "+(upgrade>0?"take continuity instrument": "connect "+label);
        public void Activate()
        {
            if(Active) return;
            if(!Ready)
            {
                GameFlow.Instance.Toast(prerequisite && !prerequisite.Active ? "Connect "+prerequisite.label+" first." : clue);
                Soundscape.Instance.PlayAt(transform.position,SoundCue.Release,.28f); return;
            }
            Active=true;
            if(upgrade>0)
            {
                GameFlow.Instance.Anchors.Upgrade(upgrade);
                GameFlow.Instance.Toast(upgrade==1?"Continuity instrument · E holds a form · R releases holds":"Second channel online · Two forms can be held together",7);
            }
            else { GameFlow.Instance.RecordCircuit(); GameFlow.Instance.Toast(label+" connected · Departure line "+GameFlow.Instance.ConnectedCircuits+" / 17"); Soundscape.Instance.PlayAt(transform.position,SoundCue.Signal,.5f); }
            Refresh();
        }
        void Start()
        {
            paths=new LineRenderer[optics.Length];
            for(int i=0;i<paths.Length;i++)
            {
                var go=new GameObject("Measured optical path"); go.transform.SetParent(transform,false);
                var line=go.AddComponent<LineRenderer>(); line.positionCount=2; line.useWorldSpace=true;
                line.startWidth=line.endWidth=.025f; line.sharedMaterial=WorldBuilder.Material(new Color(.8f,.4f,.12f),true); paths[i]=line;
            }
            Refresh();
        }
        void Update()
        {
            if(!GameFlow.Playing || Time.time<nextStatus) return; nextStatus=Time.time+.12f;
            Refresh();
            for(int i=0;i<optics.Length;i++)
            {
                var ray=optics[i]; var delta=ray.to-ray.from; var end=ray.to;
                if(Physics.Raycast(ray.from,delta.normalized,out var hit,delta.magnitude,~((1<<8)|(1<<2)),QueryTriggerInteraction.Ignore)) end=hit.point;
                paths[i].SetPosition(0,ray.from); paths[i].SetPosition(1,end);
                var color=ray.Met?new Color(.25f,.75f,.55f):new Color(.75f,.35f,.12f);
                paths[i].sharedMaterial.color=color; paths[i].sharedMaterial.SetColor("_EmissionColor",color*.7f);
            }
        }
        void Refresh()
        {
            if(display)
            {
                string text=label.ToUpperInvariant()+"\n"+(Active?"CONNECTED":upgrade>0?"F / TAKE INSTRUMENT":"F / CONNECT");
                if(!Active && upgrade==0)
                {
                    text+="\n"+(prerequisite && !prerequisite.Active?"AWAITING "+prerequisite.label:clue);
                    foreach(var ray in optics) text+="\n"+ray.clue+" / "+(ray.Met?"READY":"NOT READY");
                }
                display.text=text;
            }
            if(lamp) { var m=lamp.material; m.color=Active?new Color(.25f,.95f,.72f):new Color(1,.62f,.23f); m.SetColor("_EmissionColor",m.color*2); }
        }
        void OnDestroy()
        {
            if(paths!=null) foreach(var path in paths) if(path && path.sharedMaterial) Destroy(path.sharedMaterial);
        }
    }
}
