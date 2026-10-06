using UnityEngine;
namespace Unobserved
{
    // Fixed architectural families support the authored room plan. Two hero replacements are
    // intentionally isolated from puzzle reserves and from the stable entrance/exit envelopes.
    public static class RoomArchitecture
    {
        static Material plaster,stone,wood,steel,light;
        static void Materials()
        {
            if(plaster) return;
            plaster=WorldBuilder.Material(new Color(.64f,.62f,.55f)); SurfaceLibrary.Apply(plaster,6);
            stone=WorldBuilder.Material(new Color(.39f,.43f,.44f)); SurfaceLibrary.Apply(stone,0);
            wood=WorldBuilder.Material(new Color(.25f,.16f,.10f)); SurfaceLibrary.Apply(wood,3);
            steel=WorldBuilder.Material(new Color(.12f,.16f,.17f)); SurfaceLibrary.Apply(steel,4);
            light=WorldBuilder.Material(new Color(.72f,.77f,.65f),true); light.SetColor("_EmissionColor",new Color(.72f,.77f,.65f)*.75f);
        }
        static GameObject Group(Transform parent,string name)
        { var go=new GameObject(name); go.transform.SetParent(parent,false); return go; }
        static GameObject Box(Transform parent,Vector3 p,Vector3 size,Material m,bool solid=false)=>FormArchitecture.Part(parent,p,size,m,solid);
        public static void Build(Transform room,float width,float length,float height,int wing)
        {
            Materials(); var root=Group(room,"Authored architectural finish").transform;
            // Remove the old universal decoration instead of stacking another ceiling below it.
            foreach(var renderer in room.GetComponentsInChildren<Renderer>())
            {
                string n=renderer.name;
                if(n=="Ceiling beam" || n=="Recessed luminaire" || n=="Picture rail" || n=="Pilaster" || n=="Wall sconce")
                { renderer.enabled=false; var c=renderer.GetComponent<Collider>(); if(c) c.enabled=false; }
            }
            if(wing==2 || wing==5) return; // Atrium skylight and observatory rings keep their tall volume.
            bool civic=wing==0||wing==1||wing==6;
            float soffit=civic?height-.65f:height-.35f;
            for(float z=3;z<length;z+=6)
            {
                if(civic)
                {
                    Box(root,new Vector3(0,soffit,z),new Vector3(width,.18f,.22f),wing==6?wood:plaster);
                    foreach(int side in new[]{-1,1})
                    {
                        Box(root,new Vector3(side*(width/2-.17f),soffit/2,z),new Vector3(.26f,soffit,.3f),plaster);
                        Box(root,new Vector3(side*width*.27f,soffit-.11f,z),new Vector3(2.1f,.035f,.32f),light);
                    }
                    if(wing==1) Vault(root,width-1,soffit-1.1f,1.1f,z,5.8f,plaster);
                }
                else
                {
                    Box(root,new Vector3(0,soffit-.25f,z),new Vector3(width,.32f,.22f),steel);
                    foreach(int side in new[]{-1,1})
                    {
                        FormArchitecture.Beam(root,new Vector3(side*(width/2-.4f),soffit-1.25f,z),new Vector3(side*(width/2-2.4f),soffit-.25f,z),.16f,.16f,steel);
                        Box(root,new Vector3(side*width*.28f,soffit-.45f,z),new Vector3(1.3f,.06f,.2f),light);
                    }
                }
            }
            foreach(int side in new[]{-1,1})
            {
                Box(root,new Vector3(side*(width/2-.06f),.48f,length/2),new Vector3(.1f,.96f,length),civic?wood:stone);
                Box(root,new Vector3(side*(width/2-.11f),.98f,length/2),new Vector3(.14f,.045f,length),civic?wood:steel);
            }
        }
        static void Vault(Transform parent,float width,float spring,float rise,float z,float depth,Material material)
        {
            // Continuous curved soffit, with shared normals and UVs, rather than visible box strips.
            const int count=32; var vertices=new Vector3[(count+1)*2]; var uv=new Vector2[vertices.Length]; var triangles=new int[count*6];
            for(int i=0;i<=count;i++)
            {
                float a=i*Mathf.PI/count;
                for(int side=0;side<2;side++) { vertices[i*2+side]=new Vector3(Mathf.Cos(a)*width/2,spring+Mathf.Sin(a)*rise,z+(side==0?-depth/2:depth/2)); uv[i*2+side]=new Vector2(i/(float)count,side); }
                if(i<count) { int k=i*6,v=i*2; triangles[k]=v; triangles[k+1]=v+1; triangles[k+2]=v+2; triangles[k+3]=v+1; triangles[k+4]=v+3; triangles[k+5]=v+2; }
            }
            var mesh=new Mesh{name="Continuous archive vault"}; mesh.vertices=vertices; mesh.uv=uv; mesh.triangles=triangles; mesh.RecalculateNormals(); mesh.RecalculateTangents(); mesh.RecalculateBounds();
            var go=Group(parent,"Archive plaster vault"); go.AddComponent<MeshFilter>().sharedMesh=mesh; go.AddComponent<MeshRenderer>().sharedMaterial=material;
        }
        public static void Atrium(Transform room,GameObject garden)
        {
            Materials(); var root=Group(room,"Hero / garden becomes service core"); garden.transform.SetParent(root.transform,false);
            var core=Group(root.transform,"Sealed municipal lift core").transform;
            foreach(int side in new[]{-1,1})
            {
                Box(core,new Vector3(side*4.2f,2.5f,23),new Vector3(.45f,9,8.5f),plaster,true);
                Box(core,new Vector3(side*2.05f,.7f,18.57f),new Vector3(3.5f,5.4f,.25f),steel,true);
                foreach(int doorSide in new[]{-1,1})
                {
                    Box(core,new Vector3(side*2.05f+doorSide*.77f,.6f,18.4f),new Vector3(1.49f,5,.09f),stone);
                    Box(core,new Vector3(side*2.05f+doorSide*.77f,2.3f,18.34f),new Vector3(.6f,.12f,.035f),steel);
                }
                Box(core,new Vector3(side*2.05f,3.7f,18.3f),new Vector3(.7f,.13f,.05f),light);
            }
            Box(core,new Vector3(0,4.9f,18.65f),new Vector3(8.5f,3.3f,.5f),plaster,true);
            Box(core,new Vector3(0,2.5f,27.2f),new Vector3(8.5f,9,.45f),plaster,true);
            Box(core,new Vector3(0,7,23),new Vector3(10,.3f,10),stone,true);
            for(int i=0;i<9;i++) Box(core,new Vector3(-3.6f+i*.9f,5.4f,18.37f),new Vector3(.42f,1.5f,.05f),steel);
            FacilityArt.HeroCore(core);
            root.AddComponent<SpatialRevision>().Initialize(new[]{garden,core.gameObject},new Bounds(new Vector3(0,2.7f,23),new Vector3(11,10,11)));
        }
        public static void GalleryThreshold(Transform room,float width,float z,float opening,float gap,float height)
        {
            Materials(); var root=Group(room,"Hero / opening becomes deep vestibule"); root.transform.localPosition=Vector3.forward*z;
            var sets=new GameObject[2];
            for(int state=0;state<2;state++)
            {
                sets[state]=Group(root.transform,state==0?"Shallow exhibition wall":"Recessed service vestibule"); var t=sets[state].transform;
                float left=opening-gap/2,right=opening+gap/2;
                Box(t,new Vector3((-width/2+left)/2,height/2,0),new Vector3(left+width/2,height,.6f),state==0?plaster:stone,true);
                Box(t,new Vector3((right+width/2)/2,height/2,0),new Vector3(width/2-right,height,.6f),state==0?plaster:stone,true);
                Box(t,new Vector3(opening,height-.3f,0),new Vector3(gap,.6f,.6f),state==0?wood:steel,true);
                ImportedArt.Place(t,"Quaternius/Door_Frame_Square",new Vector3(opening,0,state==0?-.32f:-3.45f),new Vector3(gap*1.22f,5.03f,.35f),0,0,true);
                if(state==1)
                {
                    foreach(int side in new[]{-1,1})
                    {
                        Box(t,new Vector3(opening+side*(gap/2+.15f),2.35f,-1.7f),new Vector3(.3f,4.7f,3.4f),stone,true);
                        Box(t,new Vector3(opening+side*(gap/2-.035f),1,-1.7f),new Vector3(.07f,.12f,3.4f),steel);
                    }
                    Box(t,new Vector3(opening,4.7f,-1.7f),new Vector3(gap+.6f,.25f,3.4f),stone,true);
                    for(int i=0;i<3;i++) Box(t,new Vector3(opening,4.54f,-.5f-i*1.05f),new Vector3(gap-.3f,.035f,.12f),light);
                }
            }
            root.AddComponent<SpatialRevision>().Initialize(sets,new Bounds(new Vector3(0,height/2,-1.65f),new Vector3(width,height+.5f,4)));
        }
    }
}
