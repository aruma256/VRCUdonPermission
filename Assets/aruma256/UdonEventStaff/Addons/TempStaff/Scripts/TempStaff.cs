using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class TempStaff : UdonSharpBehaviour
{
    [SerializeField] UdonEventStaff udonEventStaff;
}
