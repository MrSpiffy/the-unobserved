using UnityEngine;
namespace Unobserved
{
    // Treads are visual; a continuous collision wedge avoids capsule snagging on
    // many small riser edges. The surface reaches the exact adjoining landing height.
    public sealed class StairCollision : MonoBehaviour
    {
        Mesh mesh;
        public static void Build(GameObject parent,float width,float length,float rise,bool descending=false)
        {
            var owner=parent.AddComponent<StairCollision>(); float x=width/2,z=length/2;
            float a=descending?rise:0,b=descending?0:rise;
            owner.mesh=new Mesh {name="Continuous stair collision"};
            owner.mesh.vertices=new[]{new Vector3(-x,-.4f,-z),new Vector3(x,-.4f,-z),new Vector3(-x,-.4f,z),new Vector3(x,-.4f,z),new Vector3(-x,a,-z),new Vector3(x,a,-z),new Vector3(-x,b,z),new Vector3(x,b,z)};
            owner.mesh.triangles=new[]{4,6,5,5,6,7,0,1,2,1,3,2,0,2,4,2,6,4,1,5,3,5,7,3,0,4,1,1,4,5,2,3,6,3,7,6};
            owner.mesh.RecalculateBounds(); owner.mesh.RecalculateNormals(); parent.AddComponent<MeshCollider>().sharedMesh=owner.mesh;
            parent.AddComponent<SurfaceTag>().kind=SurfaceKind.Metal;
        }
        void OnDestroy() { if(mesh) Destroy(mesh); }
    }
}
