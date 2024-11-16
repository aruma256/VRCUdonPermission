
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Aruma256.UdonPermission
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AreaBasedPermission : UdonSharpBehaviour
    {
        [Header("UdonPermissionへのリンク")]
        [SerializeField] UdonPermission udonPermission;
        [Header("領域に入ったら権限を付与する")]
        [SerializeField] bool givePermissionOnEnter = true;
        [Header("領域から出たら権限を剥奪する")]
        [SerializeField] bool revokePermissionOnExit = true;

        void Start()
        {
            if (udonPermission == null) {
                Debug.Log("ワールドエラー: UdonPermission がリンクされていません。");
            }
        }

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (udonPermission == null) return;
            if (!player.isLocal) return;
            if (!givePermissionOnEnter) return;
            udonPermission.GivePermission();
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (udonPermission == null) return;
            if (!player.isLocal) return;
            if (!revokePermissionOnExit) return;
            udonPermission.RevokePermission();
        }
    }
}
