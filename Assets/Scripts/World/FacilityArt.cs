using UnityEngine;
namespace Unobserved
{
    // Reusable art compositions on the existing authored footprint. These models supply appearance;
    // the stable structural shell and functional-state colliders continue to own traversal/occlusion.
    public static class FacilityArt
    {
        const string F="KenneyFurniture/", K="KenneyFactory/", Q="Quaternius/";
        public static void Proxy(Transform parent,Vector3 center,Vector3 size)
        { var go=new GameObject("Authored furniture collision"); go.transform.SetParent(parent,false); go.transform.localPosition=center; go.AddComponent<BoxCollider>().size=size; }
        public static void Bench(Transform room,Vector3 p)
        {
            ImportedArt.Place(room,F+"benchCushion",p,new Vector3(2.8f,1.05f,.95f),180);
            Proxy(room,p+new Vector3(0,.43f,0),new Vector3(2.8f,.86f,.95f));
        }
        public static void Shelf(Transform room,Vector3 p,int seed)
        {
            for(int i=0;i<3;i++) ImportedArt.Place(room,F+"bookcaseClosedDoors",p+new Vector3((i-1)*1.62f,0,0),new Vector3(1.57f,3.45f,.7f),180);
            Proxy(room,p+Vector3.up*1.72f,new Vector3(4.9f,3.44f,.7f));
        }
        public static void Plant(Transform room,Vector3 p,float scale)
        {
            float height=Mathf.Min(3.2f,1.45f*scale);
            ImportedArt.Place(room,F+"pottedPlant",p,new Vector3(height*.42f,height,height*.42f));
            Proxy(room,p+Vector3.up*.2f*scale,new Vector3(.42f,.4f,.42f)*scale);
        }
        public static void Workbench(Transform room,Vector3 p)
        {
            ImportedArt.Place(room,F+"desk",p,new Vector3(3,1.05f,1.25f),180);
            ImportedArt.Place(room,F+"computerScreen",p+new Vector3(.55f,1.05f,.18f),new Vector3(.85f,.63f,.23f),180);
            ImportedArt.Place(room,F+"computerKeyboard",p+new Vector3(.55f,1.05f,-.36f),new Vector3(.57f,.055f,.24f),180);
            ImportedArt.Place(room,K+"box-large",p+new Vector3(-.7f,1.05f,0),new Vector3(.52f,.26f,.42f));
            Proxy(room,p+Vector3.up*.49f,new Vector3(3,.98f,1.25f));
        }
        public static void ReceptionDesk(Transform room)
        {
            for(int i=0;i<3;i++) ImportedArt.Place(room,F+"kitchenBar",new Vector3(-7.65f+i*2.65f,0,11),new Vector3(2.65f,1.25f,1.8f),180);
            ImportedArt.Place(room,F+"computerScreen",new Vector3(-5,1.25f,11),new Vector3(.8f,.6f,.22f),180);
            ImportedArt.Place(room,F+"chairDesk",new Vector3(-5,0,12.8f),new Vector3(.7f,1.13f,.7f));
            Proxy(room,new Vector3(-5,.55f,12.8f),new Vector3(.7f,1.1f,.7f));
            Proxy(room,new Vector3(-5,.6f,11),new Vector3(8,1.2f,1.8f));
        }
        public static void WaitingRoom(Transform room)
        {
            foreach(float z in new[]{8f,14f})
            {
                ImportedArt.Place(room,F+"loungeSofa",new Vector3(8,0,z),new Vector3(3.2f,1.05f,1.1f),90);
                Proxy(room,new Vector3(8,.5f,z),new Vector3(1.1f,1,3.2f));
            }
            ImportedArt.Place(room,F+"tableCoffee",new Vector3(6.5f,0,11),new Vector3(1.4f,.48f,.85f));
            Proxy(room,new Vector3(6.5f,.24f,11),new Vector3(1.4f,.48f,.85f));
        }
        public static void Beam(Transform root,Vector3 start,float length,bool alongZ,int reality=0)
        {
            var beam=ImportedArt.Place(root,Q+"Column_Simple",start,new Vector3(.18f,length,.18f),0,reality,true);
            beam.transform.localRotation=alongZ?Quaternion.Euler(90,0,0):Quaternion.Euler(0,0,-90);
        }
        public static void LandingFrame(Transform room,Vector3 p,Vector2 size)
        {
            foreach(int side in new[]{-1,1})
            {
                Beam(room,p+new Vector3(side*(size.x/2-.1f),-.12f,-size.y/2),size.y,true);
                Beam(room,p+new Vector3(-size.x/2,-.12f,side*(size.y/2-.1f)),size.x,false);
            }
        }
        public static void ObservatoryCore(Transform room)
        {
            foreach(int side in new[]{-1,1})
            {
                var face=new GameObject("Instrument housing elevation").transform; face.SetParent(room,false);
                face.localPosition=new Vector3(side*1.01f,0,20); face.localRotation=Quaternion.Euler(0,side*90,0);
                Wall(face,new Vector3(0,3.5f,0),new Vector3(10,7,.12f),5);
                ImportedArt.Place(room,Q+"Prop_Vent_Big",new Vector3(0,2,20+side*5.02f),new Vector3(1.8f,3,.12f),side==1?180:0,0,true);
            }
        }
        public static void Wall(Transform parent,Vector3 center,Vector3 size,int wing)
        {
            string key=wing==0||wing==1||wing==6?F+"wall":wing==2||wing==5?Q+"WallAstra_Straight_Flat":Q+"WallBand_Straight";
            int columns=Mathf.Max(1,Mathf.CeilToInt(size.x/4)); int rows=Mathf.Max(1,Mathf.CeilToInt(size.y/4));
            for(int x=0;x<columns;x++) for(int y=0;y<rows;y++)
                ImportedArt.Place(parent,key,center+new Vector3(-size.x/2+(x+.5f)*size.x/columns,-size.y/2+y*size.y/rows,0),new Vector3(size.x/columns,size.y/rows,size.z),0,0,true);
        }
        public static void FinishRoom(Transform room,float width,float length,float height,int wing)
        {
            // Each side is a continuous modular elevation, not props placed in front of a visible cube.
            foreach(var r in room.GetComponentsInChildren<Renderer>()) if(r.name=="Wall surface") r.enabled=false;
            foreach(int side in new[]{-1,1})
            {
                var elevation=new GameObject("Modular side elevation"); elevation.transform.SetParent(room,false);
                elevation.transform.localPosition=new Vector3(side*(width/2-.12f),0,length/2); elevation.transform.localRotation=Quaternion.Euler(0,side==1?-90:90,0);
                Wall(elevation.transform,new Vector3(0,height/2,0),new Vector3(length,height,.25f),wing);
                if(wing>=2 && wing!=6) for(float z=4;z<length-1;z+=8)
                    ImportedArt.Place(room,Q+(wing==4||wing==7?"Column_Pipes":"Column_Simple"),new Vector3(side*(width/2-.55f),0,z),new Vector3(.72f,height,.65f),0,0,true);
            }
            foreach(float z in new[]{0f,length})
                ImportedArt.Place(room,Q+"Door_Frame_Square",new Vector3(0,0,z),new Vector3(7.05f,5.12f,.65f),0,0,true);
            for(float z=5;z<length;z+=9)
                ImportedArt.Place(room,wing==0||wing==1?F+"lampSquareCeiling":Q+"Prop_Light_Wide",new Vector3(0,height-.18f,z),new Vector3(2.5f,.18f,.48f));
            if(wing==2)
            {
                // Window bays around the tall winter garden retain its architectural scale.
                foreach(int side in new[]{-1,1}) for(float z=7;z<length-3;z+=8)
                    ImportedArt.Place(room,Q+"WallWindow_Straight",new Vector3(side*(width/2-.31f),4,z),new Vector3(6,4,.3f),side==1?-90:90,0,true);
            }
            if(wing==3) foreach(int side in new[]{-1,1})
            {
                ImportedArt.Place(room,K+"machine-fortified",new Vector3(side*11,0,35),new Vector3(3,2.2f,2.6f),180);
                Proxy(room,new Vector3(side*11,1.1f,35),new Vector3(3,2.2f,2.6f));
            }
            if(wing==4)
            {
                foreach(int side in new[]{-1,1})
                {
                    for(int i=0;i<4;i++) ImportedArt.Place(room,K+"hopper-high-round",new Vector3(side*16,-2.6f,10+i*7),new Vector3(2.1f,5.2f,2.1f));
                    for(float z=5;z<length-3;z+=4) ImportedArt.Place(room,K+"pipe-large-long",new Vector3(side*16,1.6f,z),new Vector3(4,.8f,.8f),90);
                    ImportedArt.Place(room,K+"pipe-large-valve",new Vector3(side*15.6f,1.4f,22),new Vector3(1.2f,1.2f,1.2f));
                }
            }
            if(wing==5)
            {
                foreach(int side in new[]{-1,1}) ImportedArt.Place(room,Q+"Prop_Computer",new Vector3(side*12,0,8),new Vector3(1.05f,1.75f,.8f),180);
            }
            if(wing==6)
            {
                // Display familiar furniture as exhibits; later functional forms can become simplified.
                for(int i=0;i<6;i++) ImportedArt.Place(room,F+(i%2==0?"loungeChair":"chairDesk"),new Vector3(i%2==0?-13:13,1.1f,4+i*5),new Vector3(1.05f,1.3f,1.05f),i%2==0?90:-90);
            }
            if(wing==7)
            {
                ImportedArt.Place(room,K+"machine-connection-pipe",new Vector3(0,0,20),new Vector3(4,7.8f,5),180);
                foreach(int side in new[]{-1,1}) ImportedArt.Place(room,Q+"Column_Pipes",new Vector3(side*2.7f,-2.6f,20),new Vector3(1,11.5f,1));
            }
        }
        public static void DeckTile(Transform room,Vector3 p,Vector2 size,int wing)
        {
            ImportedArt.Place(room,Q+(wing==2||wing==5?"Platform_Simple":"Platform_Metal"),p+Vector3.up*.006f,new Vector3(size.x,1,size.y),0,0,true);
        }
        public static void HeroCore(Transform root)
        {
            for(int side=-1;side<=1;side+=2)
            {
                ImportedArt.Place(root,Q+"Door_Frame_Square",new Vector3(side*2.05f,-2,18.28f),new Vector3(3.65f,5.5f,.45f),0,0,true);
                ImportedArt.Place(root,Q+"Door_Simple",new Vector3(side*2.05f,-2,18.22f),new Vector3(2.9f,5.1f,.15f),0,0,true);
                ImportedArt.Place(root,Q+"Column_Pipes",new Vector3(side*4.2f,-2,18.65f),new Vector3(.65f,9,.6f),0,0,true);
            }
            ImportedArt.Place(root,Q+"Prop_Vent_Big",new Vector3(0,4.3f,18.28f),new Vector3(6.7f,1.8f,.16f),0,0,true);
        }
    }
}
