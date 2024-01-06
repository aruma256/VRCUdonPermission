using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class RestrictedTeleporter : UdonSharpBehaviour
{
    [SerializeField] UdonPermission udonPermission;
    [SerializeField] Transform teleportTarget;
    [Header("非スタッフも使えるようにする（単なるテレポートスイッチにする）")]
    [SerializeField] bool allowNonStaff = false;

    void Start()
    {
        if (udonPermission == null) {
            Debug.Log("[StaffTeleport] UdonEventStaff がリンクされていません。");
            return;
        }
        if (teleportTarget == null) {
            Debug.Log("[StaffTeleport] TeleportTarget がセットされていません。");
            return;
        }
    }

    public override void Interact()
    {
        if (udonPermission == null) return;
        if (teleportTarget == null) return;
        //
        if (allowNonStaff || udonPermission.AmIStaff()) {
            Networking.LocalPlayer.TeleportTo(
                teleportTarget.position,
                teleportTarget.rotation,
                VRC_SceneDescriptor.SpawnOrientation.Default,
                false
            );
        }
    }
}
