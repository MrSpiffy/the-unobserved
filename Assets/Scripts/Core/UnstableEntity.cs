using UnityEngine;

namespace Unobserved
{
    [RequireComponent(typeof(ObservationTarget))]
    public sealed class UnstableEntity : MonoBehaviour
    {
        public static event System.Action<UnstableEntity> Shifted;
        public RealityState[] states;
        public float length = 6;
        public float width = 4;
        public float rise = 2.4f;
        public bool permanentlyAnchored;
        public bool allowForgotten;
        public string label = "Structural reserve";
        public bool Anchored { get; private set; }
        public Form CurrentForm => states[index].form;
        public bool Traversable => CurrentForm == Form.Bridge || CurrentForm == Form.Steps || CurrentForm == Form.Islands;
        public int TransitionCount { get; private set; }
        public int Interpretation {get;private set;}
        public string ArchitecturalName => FormArchitecture.Name(CurrentForm,Interpretation);
        int index;
        StateDeck deck;
        GameObject shape;
        Material material;
        LineRenderer outline;
        void Start()
        {
            deck = new StateDeck(Mathf.RoundToInt(transform.position.z * 103 + transform.position.x * 17));
            GetComponent<ObservationTarget>().FullyUnobserved += TryTransition;
            Anchored = permanentlyAnchored;
            material = WorldBuilder.Material(states[0].color);
            BuildShape();
            outline = gameObject.AddComponent<LineRenderer>();
            outline.useWorldSpace = false; outline.loop = true; outline.positionCount = 4;
            outline.SetPositions(new[] { new Vector3(-width/2,.08f,-length/2), new Vector3(width/2,.08f,-length/2),
                new Vector3(width/2,.08f,length/2), new Vector3(-width/2,.08f,length/2) });
            outline.startWidth = outline.endWidth = .045f;
            outline.material = WorldBuilder.Material(new Color(.22f,.85f,.83f), true);
            outline.enabled = Anchored;
        }
        void TryTransition()
        {
            if (Anchored || !GameFlow.Playing) return;
            // A supporting structure must never disappear underneath the player.
            Bounds safety = GetComponent<ObservationTarget>().WorldBounds;
            safety.Expand(new Vector3(.8f, 3, .8f));
            if (safety.Contains(GameFlow.Instance.Player.transform.position)) return;
            index = deck.Next(states.Length, index);
            Shifted?.Invoke(this);
            BuildShape(); TransitionCount++;
            Soundscape.Instance.Shift(transform.position, CurrentForm);
        }
        public void SetAnchor(bool value)
        {
            if (permanentlyAnchored) return;
            Anchored = value;
            if (outline) outline.enabled = value;
            RefreshColor();
            Soundscape.Instance.PlayAt(transform.position, value ? SoundCue.Anchor : SoundCue.Release);
        }
        void RefreshColor()
        {
            material.color = Anchored ? new Color(.3f,.75f,.7f) : states[index].color;
            material.SetColor("_EmissionColor", Anchored ? new Color(.07f,.3f,.28f) : Color.black);
            material.EnableKeyword("_EMISSION");
        }
        void Part(Vector3 p, Vector3 scale)
        {
            var part=WorldBuilder.Box("State geometry", transform.TransformPoint(p), scale, material, shape.transform);
            part.transform.rotation=transform.rotation;
            part.GetComponent<Renderer>().enabled=false;
            part.AddComponent<SurfaceTag>().kind=Interpretation==2?SurfaceKind.Metal:SurfaceKind.Stone;
        }
        void BuildShape()
        {
            if (shape) { shape.SetActive(false); Destroy(shape); }
            shape = new GameObject(states[index].displayName); shape.transform.SetParent(transform, false);
            var environment=GetComponentInParent<EnvironmentThemeController>();
            Interpretation=environment?environment.Index:0;
            RefreshColor();
            switch (CurrentForm)
            {
                case Form.Bridge: Part(new Vector3(0,-.3f,0), new Vector3(width,.6f,length)); break;
                case Form.Pillar: Part(new Vector3(0,1.9f,0), new Vector3(width,3.8f,length)); break;
                case Form.Steps:
                    for (int i = 0; i < 10; i++)
                    {
                        float h = rise*(i+1)/10;
                        Part(new Vector3(0,(h-.4f)/2,-length/2 + (i+.5f)*length/10), new Vector3(width,h+.4f,length/10+.02f));
                    }
                    foreach(var collider in shape.GetComponentsInChildren<Collider>()) collider.enabled=false;
                    StairCollision.Build(shape,width,length,rise);
                    break;
                case Form.Islands:
                    for (int i = 0; i < 3; i++) Part(new Vector3((i%2 == 0 ? -.5f : .5f),-.25f,-length/3+i*length/3),new Vector3(width*.65f,.5f,length*.23f));
                    break;
                case Form.Empty:
                    // Thin side markers leave the unstable location readable and targetable.
                    Part(new Vector3(-width/2,.35f,-length/2), new Vector3(.15f,.7f,.15f)); break;
            }
            FormArchitecture.Build(shape.transform,CurrentForm,width,length,rise,Interpretation);
            GetComponent<ObservationTarget>().RefreshVisualBounds();
            foreach(var tag in shape.GetComponentsInChildren<SurfaceTag>()) tag.kind=Interpretation==2?SurfaceKind.Metal:SurfaceKind.Stone;
        }
        void OnDestroy() { if(material) Destroy(material); if(outline && outline.material) Destroy(outline.material); }
    }
}
