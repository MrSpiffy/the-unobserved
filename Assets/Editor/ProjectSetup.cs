using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unobserved;

public static class ProjectSetup
{
    [MenuItem("Unobserved/Create or refresh authored scene")]
    public static void Setup()
    {
        Directory.CreateDirectory("Assets/Data"); Directory.CreateDirectory("Assets/Scenes"); Directory.CreateDirectory("Assets/Resources");
        if(!AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/WorldSurface.mat"))
            AssetDatabase.CreateAsset(new Material(Shader.Find("Standard")),"Assets/Resources/WorldSurface.mat");
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        var root=new GameObject("The Unobserved / Authored World"); var builder=root.AddComponent<WorldBuilder>();
        builder.realityStates=new RealityState[5];
        string[] names={"Span","Fold","Column","Absence","Stepping stones"};
        Form[] forms={Form.Bridge,Form.Steps,Form.Pillar,Form.Empty,Form.Islands};
        for(int i=0;i<5;i++)
        {
            var state=Asset<RealityState>("State_"+i); state.form=forms[i]; state.displayName=names[i];
            state.color=new Color(.37f+i*.025f,.48f+i*.015f,.46f); builder.realityStates[i]=state; EditorUtility.SetDirty(state);
        }
        builder.transitions=Asset<TransitionSettings>("Transitions");
        builder.transitions.unseenDelay=0; builder.transitions.observationDwell=0; builder.transitions.sampleInterval=0; builder.transitions.maxDistance=80; EditorUtility.SetDirty(builder.transitions);
        builder.themes=new EnvironmentThemeData[3];
        Color[] walls={new Color(.62f,.60f,.52f),new Color(.44f,.48f,.49f),new Color(.46f,.40f,.32f)};
        Color[] floors={new Color(.27f,.29f,.25f),new Color(.33f,.35f,.35f),new Color(.30f,.26f,.22f)};
        Color[] accents={new Color(.88f,.89f,.77f),new Color(.74f,.82f,.87f),new Color(.93f,.73f,.49f)};
        for(int i=0;i<3;i++)
        {
            var t=Asset<EnvironmentThemeData>("Theme_"+i); t.walls=walls[i]; t.floor=floors[i]; t.accent=accents[i];
            t.fog=walls[i]*.23f; t.ambiencePitch=1+i*.17f; t.lightIntensity=i==2?1.8f:1.4f;
            builder.themes[i]=t; EditorUtility.SetDirty(t);
        }
        PlayerSettings.companyName="Continuity Studies"; PlayerSettings.productName="The Unobserved";
        PlayerSettings.bundleVersion="0.8.0";
        PlayerSettings.defaultScreenWidth=1280; PlayerSettings.defaultScreenHeight=720;
        PlayerSettings.fullScreenMode=FullScreenMode.Windowed; PlayerSettings.runInBackground=false;
        PlayerSettings.colorSpace=ColorSpace.Linear;
        // The legacy Input Manager keeps this small project independent of extra packages.
        var serialized=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
        var input=serialized.FindProperty("activeInputHandler"); if(input!=null) { input.intValue=0; serialized.ApplyModifiedProperties(); }
        QualitySettings.vSyncCount=1; QualitySettings.pixelLightCount=8; QualitySettings.antiAliasing=4; QualitySettings.shadowDistance=35;
        EditorSceneManager.SaveScene(scene,"Assets/Scenes/TheUnobserved.unity");
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/TheUnobserved.unity",true)};
        AssetDatabase.SaveAssets();
        ValidateDeck();
        Debug.Log("UNOBSERVED_SETUP_SUCCESS");
    }
    static T Asset<T>(string name) where T:ScriptableObject
    {
        string path="Assets/Data/"+name+".asset";
        var asset=AssetDatabase.LoadAssetAtPath<T>(path);
        if(!asset) { asset=ScriptableObject.CreateInstance<T>(); AssetDatabase.CreateAsset(asset,path); } return asset;
    }
    public static void Build()
    {
        AssetPreparation.Prepare(); Setup(); Directory.CreateDirectory("Builds/Windows");
        var report=BuildPipeline.BuildPlayer(EditorBuildSettings.scenes,"Builds/Windows/The Unobserved.exe",BuildTarget.StandaloneWindows64,BuildOptions.None);
        if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded) throw new Exception("Build failed: "+report.summary.result);
        Debug.Log("UNOBSERVED_BUILD_SUCCESS");
    }
    [MenuItem("Unobserved/Validate state deck")]
    public static void ValidateDeck()
    {
        for(int seed=0;seed<100;seed++)
        {
            var deck=new StateDeck(seed); int current=0; var last=new int[]{-10,-10,-10};
            for(int i=0;i<100;i++)
            {
                int next=deck.Next(3,current);
                if(next==current) throw new Exception("State repeated");
                last[next]=i;
                for(int s=0;s<3;s++) if(i>4 && i-last[s]>4) throw new Exception("State starvation");
                current=next;
            }
        }
        Debug.Log("State deck: 10,000 transitions passed repeat/starvation checks.");
    }
}
