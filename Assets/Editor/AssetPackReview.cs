using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
public static class AssetPackReview
{
    public static void Inspect()
    {
        var log=new StringBuilder();
        foreach(string guid in AssetDatabase.FindAssets("t:Model",new[]{"Assets/ThirdParty"}))
        {
            string path=AssetDatabase.GUIDToAssetPath(guid); var source=AssetDatabase.LoadAssetAtPath<GameObject>(path); var go=Object.Instantiate(source);
            var bounds=new Bounds(); bool first=true;
            foreach(var r in go.GetComponentsInChildren<Renderer>()) { if(first) {bounds=r.bounds; first=false;} else bounds.Encapsulate(r.bounds); }
            log.AppendLine(path+" SIZE="+bounds.size.ToString("F3")+" MIN="+bounds.min.ToString("F3"));
            foreach(var r in go.GetComponentsInChildren<Renderer>()) foreach(var m in r.sharedMaterials) if(m) log.AppendLine("  MATERIAL "+m.name+" COLOR "+m.color+" TEX "+(m.mainTexture?m.mainTexture.name:"none"));
            Object.DestroyImmediate(go);
        }
        File.WriteAllText("Documentation/AssetReview/unity-model-report.txt",log.ToString()); Debug.Log("ASSET_REVIEW_COMPLETE");
    }
}
