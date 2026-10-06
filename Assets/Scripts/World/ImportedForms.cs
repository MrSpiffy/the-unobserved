using UnityEngine;
namespace Unobserved
{
    public static class ImportedForms
    {
        public static bool Build(Transform parent,Form form,float width,float length,float rise,int reality)
        {
            var entity=parent.GetComponentInParent<UnstableEntity>();
            // A small, explicit late-game exception: function survives when detail is forgotten.
            if(entity && entity.allowForgotten && reality==2) return false;
            var root=new GameObject("Constructed reserve").transform; root.SetParent(parent,false);
            string Q="Quaternius/",F="KenneyFurniture/",K="KenneyFactory/";
            switch(form)
            {
                case Form.Bridge:
                    int spans=Mathf.CeilToInt(length/3);
                    for(int i=0;i<spans;i++) ImportedArt.Place(root,Q+(reality==0?"Platform_Simple":"Platform_Metal"),new Vector3(0,.005f,-length/2+(i+.5f)*length/spans),new Vector3(width,1,length/spans),0,reality);
                    foreach(int side in new[]{-1,1}) FacilityArt.Beam(root,new Vector3(side*(width/2-.13f),-.12f,-length/2),length,true,reality);
                    break;
                case Form.Steps:
                    ImportedArt.Place(root,reality==0?F+"stairs":Q+"Platform_Stairs_4Wide",Vector3.zero,new Vector3(width,rise,length),0,reality);
                    break;
                case Form.Pillar:
                    if(reality==0 && length<1.3f)
                    {
                        int cabinets=Mathf.Max(1,Mathf.RoundToInt(width/1.2f));
                        for(int i=0;i<cabinets;i++) ImportedArt.Place(root,F+"bookcaseClosedDoors",new Vector3(-width/2+(i+.5f)*width/cabinets,0,0),new Vector3(width/cabinets,3.8f,length),180);
                    }
                    else ImportedArt.Place(root,reality==0?K+"machine-window":K+"machine-fortified",Vector3.zero,new Vector3(width,3.8f,length),180,reality);
                    break;
                case Form.Islands:
                    for(int i=0;i<3;i++)
                    {
                        var p=new Vector3(i%2==0?-.5f:.5f,0,-length/3+i*length/3);
                        ImportedArt.Place(root,Q+(reality==0?"Platform_Simple":"Platform_Metal"),p+Vector3.up*.005f,new Vector3(width*.65f,1,length*.23f),0,reality);
                        ImportedArt.Place(root,Q+"Column_Round",p+Vector3.down*1.3f,new Vector3(width*.45f,1.29f,length*.16f),0,reality);
                    }
                    break;
                case Form.Empty:
                    // Sockets stay at the threshold rather than explanatory floating text.
                    foreach(int side in new[]{-1,1}) ImportedArt.Place(root,K+"box-large",new Vector3(side*(width/2-.12f),-.02f,-length/2+.1f),new Vector3(.22f,.1f,.22f),0,reality);
                    break;
            }
            return true;
        }
    }
}
