using UnityEngine;
namespace Unobserved
{
    public sealed class LocalAmbience:MonoBehaviour
    {
        public int sound;
        AudioSource source;
        void Start()
        {
            source=gameObject.AddComponent<AudioSource>(); source.clip=Resources.Load<AudioClip>("Audio/ambience_"+sound); source.loop=true;
            source.spatialBlend=1; source.minDistance=1.5f; source.maxDistance=18; source.rolloffMode=AudioRolloffMode.Linear; source.dopplerLevel=0;
            source.Play(); if(source.clip) source.time=Mathf.Abs(transform.position.x+transform.position.z)%(source.clip.length-1);
        }
        void Update() { if(source) source.volume=Soundscape.Instance.AmbienceVolume*.08f; }
    }
}
