using UnityEngine;
namespace Unobserved
{
    public sealed class OutsideAudio:MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if(!other.GetComponent<FirstPersonController>()) return;
            Soundscape.Instance.Outside=true; GameFlow.Instance.Player.eye.clearFlags=CameraClearFlags.Skybox;
            RenderSettings.fogColor=new Color(.65f,.75f,.74f); RenderSettings.fogDensity=.003f;
        }
        void OnTriggerExit(Collider other)
        {
            if(!other.GetComponent<FirstPersonController>() || Vector3.Dot(other.transform.position-transform.position,transform.forward)>=0) return;
            Soundscape.Instance.Outside=false; GameFlow.Instance.Player.eye.clearFlags=CameraClearFlags.SolidColor;
        }
    }
}
