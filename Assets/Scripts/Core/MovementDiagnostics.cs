using System.Collections.Generic;
using UnityEngine;
namespace Unobserved
{
    public sealed class MovementDiagnostics:MonoBehaviour
    {
        readonly HashSet<string> names=new HashSet<string>();
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            string key=hit.collider.name+" / "+hit.collider.GetType().Name;
            if(names.Add(key)) Debug.Log("STAIR CONTACT: "+key+" normal="+hit.normal+" point="+hit.point+" bounds="+hit.collider.bounds);
        }
    }
}
