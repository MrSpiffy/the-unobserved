using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unobserved
{
    public enum FlowState { Title, Playing, Paused, Ending }
    public sealed class GameFlow : MonoBehaviour
    {
        public static GameFlow Instance { get; private set; }
        public static bool Playing => Instance && Instance.State==FlowState.Playing;
        public FlowState State { get; private set; } = FlowState.Title;
        public FirstPersonController Player { get; set; }
        public AnchorManager Anchors { get; set; }
        public string Section { get; private set; }="01 / ARRIVAL";
        public string Message { get; private set; }
        public float Elapsed { get; private set; }
        public float Fade { get; private set; }=1;
        public int ConnectedCircuits {get;private set;}
        public string Objective {get;private set;}="The exterior doors are sealed. Restore the departure line.";
        static readonly string[] objectives={
            "The exterior doors are sealed. Find the archive's departure circuit.",
            "Restore all three archive circuits to reopen the route.",
            "Bring both ventilation towers online.",
            "Recover the instrument. Complete the three calibration tests.",
            "Restore Intake, then Filter. A connected circuit survives release.",
            "Align both lenses to recover the exterior signal.",
            "Preserve the empty passages. Keep the last route open.",
            "Connect Feed, reclaim your anchors, then restore Return. Daylight is close."};
        float messageTime;
        public static bool Verifying => System.Array.IndexOf(System.Environment.GetCommandLineArgs(),"-smokeTest")>=0;
        void Awake() { Instance=this; Time.timeScale=1; if(Verifying) Application.runInBackground=true; }
        void Start() { AudioListener.volume=PlayerPrefs.GetFloat("volume",.7f); UnlockCursor(); }
        void Update()
        {
            Fade=Mathf.MoveTowards(Fade,0,Time.unscaledDeltaTime*.7f);
            if(Playing)
            {
                Elapsed+=Time.deltaTime;
                if(Input.GetKeyDown(KeyCode.Escape)) Pause();
                if(Input.GetKeyDown(KeyCode.Backspace)) Respawn();
            }
            else if(State==FlowState.Paused && Input.GetKeyDown(KeyCode.Escape)) Resume();
            messageTime-=Time.unscaledDeltaTime; if(messageTime<=0) Message="";
        }
        public void Begin()
        {
            State=FlowState.Playing; Time.timeScale=1; LockCursor(); Fade=1;
            Toast("WASD to move · Mouse to look · Space to jump",7);
        }
        public void Pause() { State=FlowState.Paused; Time.timeScale=0; UnlockCursor(); }
        public void Resume() { State=FlowState.Playing; Time.timeScale=1; LockCursor(); }
        public void Restart() { Time.timeScale=1; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
        public void Respawn() { Player.Teleport(Player.Checkpoint); Player.transform.rotation=Quaternion.Euler(0,Player.CheckpointYaw,0); Player.eye.clearFlags=CameraClearFlags.SolidColor; Soundscape.Instance.Outside=false; Fade=1; Soundscape.Instance.PlayAt(Player.Checkpoint,SoundCue.Release,.25f); }
        public void Finish() { State=FlowState.Ending; Time.timeScale=0; UnlockCursor(); }
        public void Toast(string message,float seconds=4) { Message=message; messageTime=seconds; }
        public void EnterSection(int index,string title) { Section=index.ToString("00")+" / "+title.ToUpperInvariant(); Objective=objectives[Mathf.Clamp(index-1,0,objectives.Length-1)]; Toast(Section,4); }
        public void RecordCircuit() { ConnectedCircuits++; }
        public void Quit() { Application.Quit(); }
        static void LockCursor() { Cursor.lockState=CursorLockMode.Locked; Cursor.visible=false; }
        static void UnlockCursor() { Cursor.lockState=CursorLockMode.None; Cursor.visible=true; }
        void OnApplicationFocus(bool focus) { if(!focus && Playing && !Application.isBatchMode && !Verifying) Pause(); }
    }
}
