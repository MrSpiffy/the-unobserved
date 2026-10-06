using UnityEngine;

namespace Unobserved
{
    public sealed class GameUI : MonoBehaviour
    {
        GUIStyle title, subtitle, body, button, small;
        Texture2D pixel;
        bool settings;
        void Init()
        {
            if(pixel) return;
            pixel=new Texture2D(1,1); pixel.SetPixel(0,0,Color.white); pixel.Apply();
            title=new GUIStyle(GUI.skin.label){fontSize=54,fontStyle=FontStyle.Bold}; title.normal.textColor=new Color(.87f,.91f,.85f);
            subtitle=new GUIStyle(GUI.skin.label){fontSize=15}; subtitle.normal.textColor=new Color(.4f,.81f,.75f);
            body=new GUIStyle(GUI.skin.label){fontSize=18,wordWrap=true}; body.normal.textColor=new Color(.78f,.83f,.82f);
            small=new GUIStyle(body){fontSize=13};
            button=new GUIStyle(GUI.skin.button){fontSize=18,alignment=TextAnchor.MiddleLeft,padding=new RectOffset(20,10,8,8)};
        }
        void Rect(Rect r,Color color) { GUI.color=color; GUI.DrawTexture(r,pixel); GUI.color=Color.white; }
        void OnGUI()
        {
            Init(); var flow=GameFlow.Instance;
            GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(Screen.width/1280f,Screen.height/720f,1));
            if(flow.State==FlowState.Playing)
            {
                Rect(new Rect(639,357,2,6),new Color(.8f,.95f,.9f,.7f));
                GUI.Label(new Rect(38,30,600,30),flow.Section,small);
                GUI.Label(new Rect(38,59,1000,25),flow.Objective,small);
                GUI.Label(new Rect(1010,30,240,26),"DEPARTURE  "+flow.ConnectedCircuits+" / 17",subtitle);
                if(!string.IsNullOrEmpty(PlayerInteraction.Prompt)) GUI.Label(new Rect(440,445,750,34),PlayerInteraction.Prompt,body);
                if(flow.Anchors.Capacity>0)
                {
                    GUI.Label(new Rect(38,650,500,28),"HELD  "+flow.Anchors.Used+" / "+flow.Anchors.Capacity+"     ·     R  release all",subtitle);
                    var target=flow.Anchors.Target;
                    if(target) GUI.Label(new Rect(460,405,550,32),target.Anchored?"E  release held form":"E  hold "+target.ArchitecturalName+" ("+target.CurrentForm.ToString().ToLowerInvariant()+")",body);
                }
                if(!string.IsNullOrEmpty(flow.Message)) GUI.Label(new Rect(260,585,820,60),flow.Message,body);
            }
            else
            {
                Rect(new Rect(0,0,1280,720),new Color(.018f,.035f,.044f,.94f));
                Rect(new Rect(76,78,52,4),new Color(.3f,.83f,.73f));
                GUI.Label(new Rect(76,104,1050,35),"A STUDY IN CONTINUITY",subtitle);
                GUI.Label(new Rect(72,144,1100,85),flow.State==FlowState.Ending?"DEPARTURE RECORDED.":flow.State==FlowState.Paused?"AN INTERVAL.":"THE UNOBSERVED",title);
                GUI.Label(new Rect(78,238,640,85),flow.State==FlowState.Ending?"You are no longer inside.\nThe morning outside has kept its shape.":flow.State==FlowState.Paused?flow.Objective:"You are the last visitor still inside.\nThe exterior doors have lost their departure signal.\nRestore the line. Get back to daylight.",body);
                if(settings) DrawSettings();
                else if(flow.State==FlowState.Ending)
                {
                    GUI.Label(new Rect(78,340,700,30),"Escaped in "+Mathf.FloorToInt(flow.Elapsed/60)+"m "+Mathf.FloorToInt(flow.Elapsed%60)+"s",subtitle);
                    if(GUI.Button(new Rect(78,405,330,48),"Begin again",button)) flow.Restart();
                    if(GUI.Button(new Rect(78,466,330,48),"Leave",button)) flow.Quit();
                }
                else
                {
                    if(GUI.Button(new Rect(78,345,330,48),flow.State==FlowState.Paused?"Continue":"Enter",button)) { if(flow.State==FlowState.Paused) flow.Resume(); else flow.Begin(); }
                    if(GUI.Button(new Rect(78,404,330,48),"Settings",button)) settings=true;
                    if(flow.State==FlowState.Paused && GUI.Button(new Rect(78,463,330,48),"Restart from arrival",button)) flow.Restart();
                    if(GUI.Button(new Rect(78,522,330,48),"Leave",button)) flow.Quit();
                }
                GUI.Label(new Rect(78,633,1000,22),"WORLD REVISION / "+Application.version,small);
                GUI.Label(new Rect(78,658,1110,28),"WASD  move   /   MOUSE  look   /   SPACE  jump   /   F  interact   /   E  hold   /   ESC  pause   /   BACKSPACE  checkpoint",small);
                // Minimal architectural motif, echoing the exit portals.
                for(int i=0;i<5;i++)
                {
                    float x=850+i*24,y=300+i*18,w=300-i*48,h=300-i*36;
                    var c=new Color(.25f,.62f,.57f,.5f-i*.06f);
                    Rect(new Rect(x,y,2,h),c); Rect(new Rect(x+w,y,2,h),c); Rect(new Rect(x,y,w,2),c);
                }
            }
            if(flow.Fade>0) Rect(new Rect(0,0,1280,720),new Color(.02f,.035f,.04f,flow.Fade));
        }
        void DrawSettings()
        {
            var p=GameFlow.Instance.Player;
            GUI.Label(new Rect(78,330,400,28),"Sound  "+Mathf.RoundToInt(AudioListener.volume*100)+"%",body);
            AudioListener.volume=GUI.HorizontalSlider(new Rect(78,368,330,22),AudioListener.volume,0,1);
            GUI.Label(new Rect(78,400,400,28),"Look sensitivity  "+p.sensitivity.ToString("F1"),body);
            p.sensitivity=GUI.HorizontalSlider(new Rect(78,438,330,22),p.sensitivity,.3f,4);
            GUI.Label(new Rect(78,470,400,28),"Field of view  "+Mathf.RoundToInt(p.eye.fieldOfView),body);
            p.eye.fieldOfView=GUI.HorizontalSlider(new Rect(78,508,330,22),p.eye.fieldOfView,60,100);
            var mix=Soundscape.Instance;
            GUI.Label(new Rect(520,330,300,28),"Music",body); mix.MusicVolume=GUI.HorizontalSlider(new Rect(520,368,280,22),mix.MusicVolume,0,1);
            GUI.Label(new Rect(520,400,300,28),"Room ambience",body); mix.AmbienceVolume=GUI.HorizontalSlider(new Rect(520,438,280,22),mix.AmbienceVolume,0,1);
            GUI.Label(new Rect(520,470,300,28),"Effects",body); mix.EffectsVolume=GUI.HorizontalSlider(new Rect(520,508,280,22),mix.EffectsVolume,0,1);
            if(GUI.Button(new Rect(78,550,330,45),"Done",button))
            {
                mix.SaveMix(); PlayerPrefs.SetFloat("volume",AudioListener.volume); PlayerPrefs.SetFloat("sensitivity",p.sensitivity); PlayerPrefs.SetFloat("fov",p.eye.fieldOfView); PlayerPrefs.Save(); settings=false;
            }
        }
    }
}
