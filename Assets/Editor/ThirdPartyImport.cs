using UnityEditor;
using UnityEngine;
public sealed class ThirdPartyImport : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        if(!assetPath.StartsWith("Assets/ThirdParty/")) return;
        var m=(ModelImporter)assetImporter; m.importAnimation=false; m.importCameras=false; m.importLights=false; m.addCollider=false;
        m.isReadable=false; m.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;
    }
    void OnPreprocessTexture()
    {
        if(!assetPath.StartsWith("Assets/ThirdParty/")) return;
        var t=(TextureImporter)assetImporter; t.maxTextureSize=2048; t.mipmapEnabled=true; t.anisoLevel=4;
        if(assetPath.Contains("_Normal")) t.textureType=TextureImporterType.NormalMap;
        t.sRGBTexture=!assetPath.Contains("_Normal")&&!assetPath.Contains("_ORM");
    }
}
