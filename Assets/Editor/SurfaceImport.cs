using UnityEditor;
using UnityEngine;
public sealed class SurfaceImport:AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if(!assetPath.StartsWith("Assets/Resources/Surfaces/")) return;
        var importer=(TextureImporter)assetImporter;
        if(assetPath.Contains("_normal")) importer.textureType=TextureImporterType.NormalMap;
        importer.sRGBTexture=assetPath.Contains("_albedo"); importer.wrapMode=TextureWrapMode.Repeat;
        importer.mipmapEnabled=true; importer.anisoLevel=8; importer.maxTextureSize=1024;
        importer.textureCompression=TextureImporterCompression.CompressedHQ;
    }
}
