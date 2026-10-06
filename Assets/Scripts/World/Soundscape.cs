using UnityEngine;
namespace Unobserved
{
    public enum SoundCue { Shift, Anchor, Release, Step, Door, Signal }
    public sealed class Soundscape : MonoBehaviour
    {
        public static Soundscape Instance {get; private set;}
        public float MusicVolume {get;set;}
        public float AmbienceVolume {get;set;}
        public float EffectsVolume {get;set;}
        public bool Outside {get;set;}
        readonly AudioSource[] voices=new AudioSource[24],rooms=new AudioSource[3];
        readonly AudioLowPassFilter[] filters=new AudioLowPassFilter[24];
        readonly AudioClip[,] steps=new AudioClip[3,4];
        readonly AudioClip[] shifts=new AudioClip[4],cues=new AudioClip[6];
        AudioSource score,outside;
        int voice,theme,stepVariant;
        float lastShift=-10,lastDecor=-10;
        void Awake()
        {
            Instance=this; MusicVolume=PlayerPrefs.GetFloat("music",.55f); AmbienceVolume=PlayerPrefs.GetFloat("ambience",.75f); EffectsVolume=PlayerPrefs.GetFloat("effects",.8f);
            string[] surfaces={"carpet","stone","metal"};
            for(int s=0;s<3;s++) for(int v=0;v<4;v++) steps[s,v]=Clip("step_"+surfaces[s]+"_"+v);
            for(int i=0;i<4;i++) shifts[i]=Clip("shift_"+i);
            cues[(int)SoundCue.Anchor]=Clip("anchor"); cues[(int)SoundCue.Release]=Clip("release"); cues[(int)SoundCue.Door]=Clip("door");
            cues[(int)SoundCue.Signal]=Clip("signal");
            for(int i=0;i<voices.Length;i++)
            {
                var go=new GameObject("Spatial voice "+i); go.transform.SetParent(transform);
                var a=go.AddComponent<AudioSource>(); a.spatialBlend=1; a.minDistance=2.5f; a.maxDistance=38; a.rolloffMode=AudioRolloffMode.Linear; a.dopplerLevel=0; voices[i]=a;
                filters[i]=go.AddComponent<AudioLowPassFilter>();
                var reverb=go.AddComponent<AudioReverbFilter>(); reverb.reverbPreset=AudioReverbPreset.User; reverb.dryLevel=0; reverb.room=-1600; reverb.decayTime=1.7f; reverb.reflectionsLevel=-2400; reverb.reverbLevel=-1300;
            }
            for(int i=0;i<3;i++) { rooms[i]=Loop("Room tone "+i,Clip("ambience_"+i)); rooms[i].volume=0; }
            score=Loop("Original score / Held Light",Clip("score")); score.volume=0;
            outside=Loop("Morning outside",Clip("outside")); outside.volume=0;
        }
        AudioClip Clip(string name)=>Resources.Load<AudioClip>("Audio/"+name);
        AudioSource Loop(string name,AudioClip clip)
        {
            var go=new GameObject(name); go.transform.SetParent(transform); var a=go.AddComponent<AudioSource>(); a.clip=clip; a.loop=true; a.spatialBlend=0; a.Play(); return a;
        }
        void Update()
        {
            float dt=Time.unscaledDeltaTime;
            for(int i=0;i<3;i++) rooms[i].volume=Mathf.MoveTowards(rooms[i].volume,!Outside&&i==theme?AmbienceVolume*.65f:0,dt*.18f);
            outside.volume=Mathf.MoveTowards(outside.volume,Outside?AmbienceVolume*.9f:0,dt*.2f);
            score.volume=Mathf.MoveTowards(score.volume,MusicVolume*(GameFlow.Instance && GameFlow.Instance.State==FlowState.Ending?.8f:.65f),dt*.12f);
        }
        public void Theme(float pitch)=>theme=Mathf.Clamp(Mathf.RoundToInt((pitch-1)/.17f),0,2);
        public void Footstep(Vector3 position)
        {
            int surface=1;
            if(Physics.Raycast(position+Vector3.up*.2f,Vector3.down,out var hit,1,~(1<<8),QueryTriggerInteraction.Ignore))
            { var tag=hit.collider.GetComponent<SurfaceTag>(); if(tag) surface=Mathf.Min((int)tag.kind,2); }
            Play(position,steps[surface,stepVariant++%4],.65f,false);
        }
        public void PlayAt(Vector3 position,SoundCue cue,float volume=.65f)
        {
            if(cue==SoundCue.Step) { Footstep(position); return; }
            Play(position,cue==SoundCue.Shift?shifts[Random.Range(0,4)]:cues[(int)cue],volume,cue==SoundCue.Shift||cue==SoundCue.Door);
        }
        public void Shift(Vector3 position,Form form)
        {
            if(Time.time-lastShift<.35f || Vector3.Distance(position,GameFlow.Instance.Player.transform.position)>22) return;
            lastShift=Time.time;
            int signature=form==Form.Empty?0:form==Form.Pillar?1:form==Form.Steps?2:3;
            Play(position,shifts[signature],.32f,true);
        }
        public void Decor(Vector3 position,string key)
        {
            if(Time.time-lastDecor<2.5f || Vector3.Distance(position,GameFlow.Instance.Player.transform.position)>8) return;
            lastDecor=Time.time;
            int surface=key.Contains("Plant")||key.Contains("Chair")?0:key.Contains("KenneyFurniture")?1:2;
            Play(position,steps[surface,stepVariant++%4],.085f,true);
        }
        void Play(Vector3 position,AudioClip clip,float volume,bool occlude)
        {
            if(!clip) return;
            int i=voice++%voices.Length; var a=voices[i]; a.transform.position=position; a.clip=clip; a.volume=volume*EffectsVolume; a.pitch=Random.Range(.96f,1.04f);
            bool blocked=false;
            if(occlude && GameFlow.Instance.Player)
            {
                Vector3 eye=GameFlow.Instance.Player.eye.transform.position,ray=position+Vector3.up-eye;
                blocked=Physics.Raycast(eye,ray.normalized,ray.magnitude*.85f,~(1<<8),QueryTriggerInteraction.Ignore);
            }
            filters[i].cutoffFrequency=blocked?1400:16000; if(blocked) a.volume*=.72f; a.Play();
        }
        public void SaveMix()
        {
            PlayerPrefs.SetFloat("music",MusicVolume); PlayerPrefs.SetFloat("ambience",AmbienceVolume); PlayerPrefs.SetFloat("effects",EffectsVolume);
        }
    }
}
