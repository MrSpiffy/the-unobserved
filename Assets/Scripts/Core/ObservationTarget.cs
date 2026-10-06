using UnityEngine;
namespace Unobserved
{
    public sealed class ObservationTarget : MonoBehaviour
    {
        public Vector3 center = new Vector3(0, 1, 0);
        public Vector3 size = new Vector3(5, 3, 6);
        Renderer[] visualBounds=new Renderer[0];
        public void RefreshVisualBounds()=>visualBounds=GetComponentsInChildren<Renderer>(true);
        public Bounds WorldBounds
        {
            get
            {
                Vector3 x=transform.TransformVector(Vector3.right*size.x), y=transform.TransformVector(Vector3.up*size.y), z=transform.TransformVector(Vector3.forward*size.z);
                var result=new Bounds(transform.TransformPoint(center),new Vector3(Mathf.Abs(x.x)+Mathf.Abs(y.x)+Mathf.Abs(z.x),Mathf.Abs(x.y)+Mathf.Abs(y.y)+Mathf.Abs(z.y),Mathf.Abs(x.z)+Mathf.Abs(y.z)+Mathf.Abs(z.z)));
                foreach(var r in visualBounds) if(r && r.enabled && r.gameObject.activeInHierarchy) result.Encapsulate(r.bounds);
                return result;
            }
        }
        public bool Visible { get; private set; }
        public event System.Action FullyUnobserved;
        bool armed;
        void OnEnable() { if (ObservationManager.Instance) ObservationManager.Instance.Register(this); }
        void Start() => ObservationManager.Instance.Register(this);
        void OnDisable() { if (ObservationManager.Instance) ObservationManager.Instance.Unregister(this); }
        public void Sample(bool visible, float dt, TransitionSettings settings)
        {
            Visible = visible;
            if (visible)
            {
                armed = true;
            }
            else
            {
                if (armed)
                {
                    armed = false; // The visible -> hidden edge IS the cause. No delay or unseen cycling.
                    FullyUnobserved?.Invoke();
                }
            }
        }
    }
}
