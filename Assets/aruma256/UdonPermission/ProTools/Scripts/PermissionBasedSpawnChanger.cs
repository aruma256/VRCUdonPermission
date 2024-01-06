using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PermissionBasedSpawnChanger : UdonSharpBehaviour
{
    [SerializeField] UdonEventStaff udonEventStaff;

    void Start()
    {
        if (udonEventStaff == null) {
            Debug.Log("ワールドエラー: UdonEventStaff がリンクされていません。");
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        OnPlayerSpawn(player);
    }

    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        OnPlayerSpawn(player);
    }

    private void OnPlayerSpawn(VRCPlayerApi player)
    {
        if (udonEventStaff == null) return;
        if (!player.isLocal) return;
        if (!udonEventStaff.AmIStaff()) return;
        player.TeleportTo(
            transform.position,
            transform.rotation,
            VRC_SceneDescriptor.SpawnOrientation.Default,
            false
        );
    }
}
