using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Aruma256.UdonPermission
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PermissionBasedSpawnChanger : UdonSharpBehaviour
    {
        [Header("UdonPermissionへのリンク")]
        [SerializeField] UdonPermission udonPermission;

        void Start()
        {
            if (udonPermission == null) {
                Debug.Log("ワールドエラー: UdonPermission がリンクされていません。");
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
            if (udonPermission == null) return;
            if (!player.isLocal) return;
            if (!udonPermission.HasPermission()) return;
            player.TeleportTo(
                transform.position,
                transform.rotation,
                VRC_SceneDescriptor.SpawnOrientation.Default,
                false
            );
        }
    }
}
