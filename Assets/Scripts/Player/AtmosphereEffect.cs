using UnityEngine;
namespace Unobserved
{
    [RequireComponent(typeof(Camera))]
    public sealed class AtmosphereEffect : MonoBehaviour
    {
        Material material;
        void Awake()
        {
            var shader=Resources.Load<Shader>("Atmosphere");
            if(shader && shader.isSupported) material=new Material(shader);
        }
        void OnRenderImage(RenderTexture source,RenderTexture destination)
        {
            if(!material) { Graphics.Blit(source,destination); return; }
            int w=Mathf.Max(1,source.width/4),h=Mathf.Max(1,source.height/4);
            var a=RenderTexture.GetTemporary(w,h,0,RenderTextureFormat.ARGBHalf);
            var b=RenderTexture.GetTemporary(w,h,0,RenderTextureFormat.ARGBHalf);
            Graphics.Blit(source,a,material,1);
            material.SetVector("_BlurDirection",new Vector4(1f/w,0,0,0)); Graphics.Blit(a,b,material,2);
            material.SetVector("_BlurDirection",new Vector4(0,1f/h,0,0)); Graphics.Blit(b,a,material,2);
            material.SetTexture("_Bloom",a); Graphics.Blit(source,destination,material,0);
            RenderTexture.ReleaseTemporary(a); RenderTexture.ReleaseTemporary(b);
        }
        void OnDestroy() { if(material) Destroy(material); }
    }
}
