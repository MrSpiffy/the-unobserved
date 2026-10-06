using System.Collections.Generic;
using UnityEngine;

namespace Unobserved
{
    public sealed class AnchorManager : MonoBehaviour
    {
        public int Capacity { get; private set; }
        public int Used => held.Count;
        public UnstableEntity Target { get; private set; }
        readonly List<UnstableEntity> held = new List<UnstableEntity>();
        public void Upgrade(int count) { Capacity = count; Soundscape.Instance.PlayAt(transform.position, SoundCue.Anchor); }
        public void ReleaseAll()
        {
            foreach(var item in held) if(item) item.SetAnchor(false);
            held.Clear();
        }
        void Update()
        {
            Target = null;
            if (!GameFlow.Playing || Capacity == 0) return;
            Camera eye = GameFlow.Instance.Player.eye;
            Target=FindTarget(eye);
            if (Input.GetKeyDown(KeyCode.R)) ReleaseAll();
            if (!Target || Target.permanentlyAnchored || !Input.GetKeyDown(KeyCode.E)) return;
            Toggle(Target);
        }
        public UnstableEntity FindTarget(Camera eye)
        {
            var ray=new Ray(eye.transform.position,eye.transform.forward);
            float distance=16;
            UnstableEntity result=null;
            if(Physics.Raycast(ray,out var hit,distance,~(1<<8),QueryTriggerInteraction.Ignore))
            { distance=hit.distance; result=hit.collider.GetComponentInParent<UnstableEntity>(); }
            // An absence is aimed at through its clear aperture; a nearer wall still blocks it.
            foreach(var candidate in ObservationManager.Instance.EmptyCandidates)
            {
                if(!candidate || candidate.CurrentForm!=Form.Empty) continue;
                if(candidate.GetComponent<ObservationTarget>().WorldBounds.IntersectRay(ray,out float entry) && entry<distance)
                { distance=entry; result=candidate; }
            }
            return result;
        }
        public void Toggle(UnstableEntity entity)
        {
            if(!entity || entity.permanentlyAnchored || Capacity==0) return;
            if (entity.Anchored) { held.Remove(entity); entity.SetAnchor(false); }
            else if (held.Count < Capacity) { held.Add(entity); entity.SetAnchor(true); }
            else GameFlow.Instance.Toast("All anchors in use. Release one with E, or all with R.");
        }
    }
}
