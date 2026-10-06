using System;
using System.Collections.Generic;
using UnityEngine;
namespace Unobserved
{
    public sealed class ImportedCatalog:ScriptableObject
    {
        [Serializable] public class Entry { public string key; public GameObject prefab; public Vector3 size; }
        public Entry[] entries;
    }
    public static class ImportedArt
    {
        static ImportedCatalog catalog;
        static readonly Dictionary<string,ImportedCatalog.Entry> entries=new Dictionary<string,ImportedCatalog.Entry>();
        static readonly Dictionary<string,Material> variants=new Dictionary<string,Material>();
        public static int Count { get { Load(); return entries.Count; } }
        static void Load()
        {
            if(catalog) return;
            catalog=Resources.Load<ImportedCatalog>("ImportedCatalog");
            if(!catalog) throw new InvalidOperationException("Prepared asset catalog is missing. Run AssetPreparation.Prepare.");
            entries.Clear(); foreach(var e in catalog.entries) entries[e.key]=e;
        }
        // Models have a prepared floor pivot and verified local axes. Dimensions describe their
        // visible envelope; collision is supplied separately by the authored level/functional state.
        public static GameObject PlaceFitted(Transform parent,string key,Vector3 envelope)
        {
            Load(); var entry=entries[key];
            float scale=Mathf.Min(envelope.x/entry.size.x,envelope.y/Mathf.Max(.001f,entry.size.y),envelope.z/entry.size.z);
            return Place(parent,key,Vector3.zero,entry.size*scale);
        }
        public static GameObject Place(Transform parent,string key,Vector3 floor,Vector3 size,float yaw=0,int reality=0,bool structural=false)
        {
            Load(); if(!entries.TryGetValue(key,out var e)) throw new InvalidOperationException("Uncatalogued art: "+key);
            var go=UnityEngine.Object.Instantiate(e.prefab,parent,false); go.name=key; go.transform.localPosition=floor;
            go.transform.localRotation=Quaternion.Euler(0,yaw,0); go.GetComponent<ImportedVisual>().structural=structural;
            go.transform.localScale=new Vector3(size.x/Mathf.Max(e.size.x,.001f),e.size.y<.001f?1:size.y/e.size.y,size.z/Mathf.Max(e.size.z,.001f));
            if(reality!=0) foreach(var r in go.GetComponentsInChildren<Renderer>())
            {
                var mats=r.sharedMaterials;
                for(int i=0;i<mats.Length;i++)
                {
                    string id=mats[i].GetInstanceID()+"/"+reality;
                    if(!variants.TryGetValue(id,out var m)) { m=new Material(mats[i]); m.color*=reality==1?new Color(.79f,.87f,.9f):new Color(.92f,.79f,.64f); variants[id]=m; }
                    mats[i]=m;
                }
                r.sharedMaterials=mats;
            }
            return go;
        }
    }
}
