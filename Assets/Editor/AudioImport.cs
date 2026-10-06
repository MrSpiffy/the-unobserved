using UnityEditor;
using UnityEngine;
public sealed class AudioImport : AssetPostprocessor
{
    void OnPreprocessAudio()
    {
        if(!assetPath.StartsWith("Assets/Resources/Audio/")) return;
        var importer=(AudioImporter)assetImporter;
        bool loop=assetPath.Contains("ambience")||assetPath.Contains("score")||assetPath.Contains("outside");
        importer.forceToMono=!loop;
        var settings=importer.defaultSampleSettings; settings.compressionFormat=AudioCompressionFormat.Vorbis; settings.quality=.8f;
        settings.loadType=loop?AudioClipLoadType.Streaming:AudioClipLoadType.DecompressOnLoad; importer.defaultSampleSettings=settings;
    }
}
