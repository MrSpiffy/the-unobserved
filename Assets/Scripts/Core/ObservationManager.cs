using System.Collections.Generic;
using UnityEngine;

namespace Unobserved
{
    // One scheduler, one frustum calculation, then distance/FOV rejection before rays.
    public sealed class ObservationManager : MonoBehaviour
    {
        public static ObservationManager Instance { get; private set; }
        public Camera eye;
        public TransitionSettings settings;
        readonly List<ObservationTarget> targets = new List<ObservationTarget>();
        public readonly List<UnstableEntity> EmptyCandidates=new List<UnstableEntity>();
        readonly Plane[] planes = new Plane[6];
        Matrix4x4 lastViewProjection;
        bool hasPlanes;
        public void Register(ObservationTarget target) { if (!targets.Contains(target)) targets.Add(target); var e=target.GetComponent<UnstableEntity>(); if(e && !EmptyCandidates.Contains(e)) EmptyCandidates.Add(e); }
        public void Unregister(ObservationTarget target) { targets.Remove(target); var e=target.GetComponent<UnstableEntity>(); if(e) EmptyCandidates.Remove(e); }
        void Awake() => Instance = this;
        void LateUpdate()
        {
            if (!GameFlow.Playing || !eye || !settings) return;
            float dt = Time.deltaTime;

            foreach (var target in targets)
                if (target && target.enabled) target.Sample(IsVisible(target), dt, settings);
        }
        bool IsVisible(ObservationTarget target)=>MaySee(target.WorldBounds,target.transform);

        // Expanded side planes retain the final visible sliver. A missed ray is never
        // evidence of invisibility: only a convex occluder's complete shadow is proof.
        public bool MaySee(Bounds b,Transform owner=null,bool allowOcclusion=true)
        {
            var projection=eye.projectionMatrix;
            projection.m00/=1.08f; projection.m11/=1.08f;
            var viewProjection=projection*eye.worldToCameraMatrix;
            if(!hasPlanes || viewProjection!=lastViewProjection)
            { GeometryUtility.CalculateFrustumPlanes(viewProjection,planes); lastViewProjection=viewProjection; hasPlanes=true; }
            if(b.Contains(eye.transform.position)) return true;
            if(!GeometryUtility.TestPlanesAABB(planes,b)) return false;
            if(!allowOcclusion) return true;
            Vector3 origin=eye.transform.position,delta=b.center-origin;
            if(!Physics.Raycast(origin,delta.normalized,out var hit,delta.magnitude,
                ~((1<<2)|(1<<8)),QueryTriggerInteraction.Ignore)) return true;
            // A box is convex. If it intercepts every corner before the target, its
            // shadow contains the complete target AABB, including unsampled slivers.
            var blocker=hit.collider as BoxCollider;
            if(!blocker || (owner && blocker.transform.IsChildOf(owner))) return true;
            for(int i=0;i<8;i++)
            {
                var point=b.center+Vector3.Scale(b.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1));
                var ray=point-origin; float distance=ray.magnitude;
                if(!blocker.Raycast(new Ray(origin,ray.normalized),out var cornerHit,distance-.05f)) return true;
            }
            return false;
        }
    }
}
