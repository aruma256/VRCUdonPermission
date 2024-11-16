using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Aruma256.UdonPermission
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RestrictedTeleporter : UdonSharpBehaviour
    {
        [Header("UdonPermissionへのリンク")]
        [SerializeField] UdonPermission udonPermission;
        [Header("テレポート先")]
        [SerializeField] Transform teleportTarget;
        [Header("権限によらず使えるようにする（単なるテレポートスイッチにする）")]
        [SerializeField] bool allowEveryone = false;

        void Start()
        {
            if (udonPermission == null) {
                Debug.Log("[RestrictedTeleporter] UdonPermission がリンクされていません。");
                return;
            }
            if (teleportTarget == null) {
                Debug.Log("[RestrictedTeleporter] TeleportTarget がセットされていません。");
                return;
            }
        }

        public override void Interact()
        {
            if (udonPermission == null) return;
            if (teleportTarget == null) return;
            //
            if (allowEveryone || udonPermission.HasPermission()) {
                Networking.LocalPlayer.TeleportTo(
                    teleportTarget.position,
                    teleportTarget.rotation,
                    VRC_SceneDescriptor.SpawnOrientation.Default,
                    false
                );
            }
        }
    }
}
