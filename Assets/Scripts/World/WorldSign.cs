using UnityEngine;
namespace Unobserved
{
    public sealed class WorldSign : MonoBehaviour
    {
        Material material;
        Font font;
        void Start()
        {
            var text=GetComponent<TextMesh>(); font=text.font;
            font.RequestCharactersInTexture(text.text,text.fontSize);
            material=new Material(Resources.Load<Shader>("WorldText"));
            material.mainTexture=font.material.mainTexture;
            GetComponent<MeshRenderer>().sharedMaterial=material;
            Font.textureRebuilt+=Rebuilt;
        }
        void Rebuilt(Font changed) { if(changed==font && material) material.mainTexture=font.material.mainTexture; }
        void OnDestroy() { Font.textureRebuilt-=Rebuilt; if(material) Destroy(material); }
    }
}
