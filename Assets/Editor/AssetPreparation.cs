using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Unobserved;
public static class AssetPreparation
{
    static readonly Dictionary<string,Material> materials=new Dictionary<string,Material>();
    public static void Prepare()
    {
        materials.Clear(); Directory.CreateDirectory("Assets/Art/Materials"); Directory.CreateDirectory("Assets/Art/Prefabs");
        var entries=new List<ImportedCatalog.Entry>();
        foreach(string guid in AssetDatabase.FindAssets("t:Model",new[]{"Assets/ThirdParty"}))
        {
            string path=AssetDatabase.GUIDToAssetPath(guid),pack=path.Split('/')[2],name=Path.GetFileNameWithoutExtension(path);
            var source=AssetDatabase.LoadAssetAtPath<GameObject>(path); var root=new GameObject(name); var model=Object.Instantiate(source,root.transform,false);
            if(pack=="Quaternius" && (name.StartsWith("Wall")||name.StartsWith("Top")||name.StartsWith("Bottom"))) model.transform.localRotation=Quaternion.Euler(0,90,0);
            if(pack=="Quaternius" && (name.StartsWith("Prop_Vent")||name=="Prop_Fan_Small"||name=="Column_MetalSupport")) model.transform.localRotation=Quaternion.Euler(90,0,0);
            if(pack=="Quaternius" && name.StartsWith("Platform_Stairs")) model.transform.localRotation=Quaternion.Euler(0,180,0);
            if(pack=="KenneyFurniture" && name.StartsWith("stairs")) model.transform.localRotation=Quaternion.Euler(0,-90,0);
            if(pack=="KenneyFactory" && name.StartsWith("catwalk")) model.transform.localRotation=Quaternion.Euler(0,90,0);
            var bounds=new Bounds(); bool first=true;
            foreach(var r in root.GetComponentsInChildren<Renderer>())
            {
                if(first) {bounds=r.bounds; first=false;} else bounds.Encapsulate(r.bounds);
                var mats=r.sharedMaterials; for(int i=0;i<mats.Length;i++) mats[i]=MaterialFor(pack,mats[i]); r.sharedMaterials=mats;
            }
            model.transform.localPosition-=new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
            foreach(var collider in root.GetComponentsInChildren<Collider>()) Object.DestroyImmediate(collider);
            root.AddComponent<ImportedVisual>().assetKey=pack+"/"+name;
            string folder="Assets/Art/Prefabs/"+pack; Directory.CreateDirectory(folder);
            var prefab=PrefabUtility.SaveAsPrefabAsset(root,folder+"/"+name+".prefab");
            entries.Add(new ImportedCatalog.Entry{key=pack+"/"+name,prefab=prefab,size=bounds.size}); Object.DestroyImmediate(root);
        }
        var catalog=AssetDatabase.LoadAssetAtPath<ImportedCatalog>("Assets/Resources/ImportedCatalog.asset");
        if(!catalog) {catalog=ScriptableObject.CreateInstance<ImportedCatalog>(); AssetDatabase.CreateAsset(catalog,"Assets/Resources/ImportedCatalog.asset");}
        catalog.entries=entries.ToArray(); EditorUtility.SetDirty(catalog); AssetDatabase.SaveAssets(); Debug.Log("PREPARED_IMPORTS: "+entries.Count);
    }
    static Material MaterialFor(string pack,Material source)
    {
        string name=source?source.name:"neutral",key=pack+"_"+name;
        if(materials.TryGetValue(key,out var cached)) return cached;
        string path="Assets/Art/Materials/"+key+".mat";
        var m=AssetDatabase.LoadAssetAtPath<Material>(path); if(!m) {m=new Material(Shader.Find("Unobserved/Imported Surface")); AssetDatabase.CreateAsset(m,path);}
        m.enableInstancing=true; m.shader=Shader.Find("Unobserved/Imported Surface"); m.color=Color.white; m.SetFloat("_Saturation",.35f); m.SetFloat("_Smoothness",.25f); m.SetFloat("_Metallic",0); m.SetFloat("_UseORM",0); m.SetColor("_EmissionColor",Color.black);
        if(pack=="KenneyFurniture")
        {
            string n=name.ToLowerInvariant();
            m.color=n.Contains("wood")?new Color(.39f,.27f,.16f):n.Contains("carpet")?new Color(.26f,.34f,.31f):n.Contains("plant")?new Color(.15f,.29f,.19f):n.Contains("metal")?new Color(.23f,.28f,.28f):n.Contains("glass")?new Color(.18f,.27f,.26f):new Color(.69f,.68f,.59f);
            if(n.Contains("dark")) m.color*=.6f;
            if(n.Contains("wood")) m.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/Surfaces/wood_albedo.png");
        }
        else if(pack=="KenneyFactory")
        {
            m.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ThirdParty/KenneyFactory/Textures/colormap.png"); m.SetFloat("_Saturation",.18f); m.color=new Color(.8f,.83f,.78f);
            if(name.Contains("glass")) {m.mainTexture=null; m.color=new Color(.14f,.23f,.24f); m.SetFloat("_Smoothness",.65f);}
        }
        else
        {
            string trim=name.Contains("Trim_01")?"01":name.Contains("Trim_02")?"02":"03";
            if(name.Contains("Trim") || name.Contains("Padded"))
            {
                string p="Assets/ThirdParty/Quaternius/Textures/T_Trim_"+trim;
                m.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(p+"_BaseColor.png");
                m.SetTexture("_BumpMap",AssetDatabase.LoadAssetAtPath<Texture2D>(p+"_Normal.png")); m.SetTexture("_ORM",AssetDatabase.LoadAssetAtPath<Texture2D>(p+"_ORM.png")); m.SetFloat("_UseORM",1);
                m.color=name.Contains("Dark")?new Color(.4f,.45f,.45f):new Color(.83f,.83f,.73f);
            }
            else {m.color=name.Contains("Black")||name.Contains("Screen")?new Color(.06f,.12f,.12f):name.Contains("Glass")?new Color(.2f,.32f,.33f):new Color(.58f,.61f,.56f);}
            if(name=="M_Light") m.SetColor("_EmissionColor",new Color(.55f,.65f,.53f));
        }
        materials[key]=m; EditorUtility.SetDirty(m); return m;
    }
}
