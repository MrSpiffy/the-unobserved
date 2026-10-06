using UnityEngine;
namespace Unobserved
{
    // Visual construction only. UnstableEntity owns the independently tested collision envelope.
    public static class FormArchitecture
    {
        static readonly Material[,] palette=new Material[3,3];
        static Material M(int reality,int role)
        {
            if(palette[reality,role]) return palette[reality,role];
            Color[,] colors={{new Color(.56f,.53f,.44f),new Color(.26f,.16f,.09f),new Color(.42f,.34f,.19f)},
                {new Color(.42f,.45f,.44f),new Color(.22f,.25f,.25f),new Color(.14f,.17f,.18f)},
                {new Color(.27f,.32f,.31f),new Color(.12f,.15f,.16f),new Color(.66f,.51f,.24f)}};
            var m=WorldBuilder.Material(colors[reality,role]);
            SurfaceLibrary.Apply(m,reality==0?(role==1?3:6):reality==1?0:4);
            palette[reality,role]=m; return m;
        }
        public static string Name(Form form,int reality)
        {
            string[,] names={{"stone passage","terraced stair","archive cabinet","garden plinths","open threshold"},
                {"concrete causeway","folded concrete stair","structural pier","foundation caps","missing bay"},
                {"service catwalk","maintenance stair","ventilation housing","inspection platforms","retracted assembly"}};
            return names[Mathf.Clamp(reality,0,2),(int)form];
        }
        public static void Build(Transform parent,Form form,float w,float l,float rise,int reality)
        {
            reality=Mathf.Clamp(reality,0,2);
            if(ImportedForms.Build(parent,form,w,l,rise,reality)) return;
            var root=new GameObject(Name(form,reality)); root.transform.SetParent(parent,false);
            Transform t=root.transform; Material body=M(reality,0),dark=M(reality,1),detail=M(reality,2);
            switch(form)
            {
                case Form.Bridge:
                    Part(t,new Vector3(0,-.12f,0),new Vector3(w,.24f,l),body);
                    foreach(int side in new[]{-1,1})
                    {
                        Part(t,new Vector3(side*(w/2-.17f),-.43f,0),new Vector3(.28f,.62f,l),dark);
                        Part(t,new Vector3(side*(w/2-.06f),.012f,0),new Vector3(.1f,.02f,l),detail);
                    }
                    if(reality==0) for(float z=-l/2+.75f;z<l/2;z+=1.5f) Part(t,new Vector3(0,.005f,z),new Vector3(w,.012f,.015f),dark);
                    if(reality==2) for(float z=-l/2+.12f;z<l/2;z+=.45f) Part(t,new Vector3(0,-.015f,z),new Vector3(w-.3f,.05f,.035f),dark);
                    break;
                case Form.Steps:
                    for(int i=0;i<10;i++)
                    {
                        float h=rise*(i+1)/10,z=-l/2+(i+.5f)*l/10;
                        float thickness=reality==2?.09f:h+.35f;
                        Part(t,new Vector3(0,h-thickness/2,z),new Vector3(w,thickness,l/10+.012f),body);
                        Part(t,new Vector3(0,h+.005f,z-l/20+.045f),new Vector3(w-.12f,.012f,.07f),detail);
                    }
                    foreach(int side in new[]{-1,1})
                        Beam(t,new Vector3(side*(w/2-.09f),-.15f,-l/2),new Vector3(side*(w/2-.09f),rise-.15f,l/2),.14f,reality==2?.22f:.35f,dark);
                    break;
                case Form.Pillar:
                    Part(t,new Vector3(0,1.87f,0),new Vector3(w,3.74f,l),body);
                    Part(t,new Vector3(0,.12f,0),new Vector3(w+.02f,.24f,l+.02f),dark);
                    Part(t,new Vector3(0,3.72f,0),new Vector3(w+.02f,.12f,l+.02f),detail);
                    if(reality==0)
                    {
                        int doors=Mathf.Max(1,Mathf.FloorToInt(w/.8f));
                        for(int i=0;i<doors;i++)
                        {
                            float x=-w/2+(i+.5f)*w/doors;
                            Part(t,new Vector3(x,1.91f,-l/2-.008f),new Vector3(w/doors-.055f,3.28f,.025f),dark);
                            Part(t,new Vector3(x+w/doors*.25f,1.7f,-l/2-.045f),new Vector3(.035f,.23f,.065f),detail);
                            Part(t,new Vector3(x,2.9f,-l/2-.027f),new Vector3(Mathf.Min(.3f,w/doors*.6f),.13f,.012f),body);
                        }
                    }
                    else if(reality==1)
                    {
                        for(int i=1;i<4;i++) Part(t,new Vector3(0,i*.9f,-l/2-.009f),new Vector3(w,.016f,.018f),dark);
                        foreach(float x in new[]{-w*.32f,w*.32f}) for(int i=0;i<3;i++) Part(t,new Vector3(x,.65f+i*1.1f,-l/2-.01f),new Vector3(.06f,.06f,.018f),dark);
                    }
                    else
                    {
                        Part(t,new Vector3(0,2,-l/2-.013f),new Vector3(w*.82f,2.9f,.03f),dark);
                        for(int i=0;i<15;i++) Part(t,new Vector3(0,.66f+i*.18f,-l/2-.045f),new Vector3(w*.8f,.065f,.06f),body);
                        Part(t,new Vector3(0,3.47f,-l/2-.02f),new Vector3(w*.55f,.08f,.03f),detail);
                    }
                    break;
                case Form.Islands:
                    for(int i=0;i<3;i++)
                    {
                        Vector3 p=new Vector3(i%2==0?-.5f:.5f,0,-l/3+i*l/3);
                        Part(t,p+Vector3.down*.13f,new Vector3(w*.65f,.26f,l*.23f),body);
                        Part(t,p+Vector3.down*.75f,new Vector3(w*(reality==1?.55f:.42f),1.25f,l*.16f),dark);
                        if(reality==2) Part(t,p+Vector3.up*.005f,new Vector3(w*.42f,.012f,l*.15f),detail);
                    }
                    break;
                case Form.Empty:
                    foreach(int side in new[]{-1,1})
                    {
                        if(l<1.2f) Part(t,new Vector3(side*(w/2-.035f),1.6f,0),new Vector3(.07f,3.2f,l),dark);
                        else Part(t,new Vector3(side*(w/2-.09f),-.04f,-l/2+.12f),new Vector3(.18f,.08f,.24f),detail);
                    }
                    break;
            }
        }
        public static GameObject Part(Transform parent,Vector3 p,Vector3 size,Material material,bool solid=false)
        {
            var go=GameObject.CreatePrimitive(PrimitiveType.Cube); go.name="Architectural detail"; go.transform.SetParent(parent,false); go.transform.localPosition=p; go.transform.localScale=size;
            go.GetComponent<Renderer>().sharedMaterial=material; go.GetComponent<Collider>().enabled=solid; return go;
        }
        public static void Beam(Transform parent,Vector3 a,Vector3 b,float width,float depth,Material material)
        {
            var go=Part(parent,(a+b)/2,new Vector3(width,depth,Vector3.Distance(a,b)),material); go.transform.localRotation=Quaternion.LookRotation(b-a);
        }
    }
}
