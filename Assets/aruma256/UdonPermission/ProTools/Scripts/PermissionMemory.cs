using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Aruma256.UdonPermission
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PermissionMemory : PermissionCallbackBase
    {
        [Header("UdonPermissionへのリンク")]
        [SerializeField] UdonPermission udonPermission;
        [UdonSynced(UdonSyncMode.None)] private bool _hasPermission = false;

        void Start()
        {
            if (udonPermission == null) {
                Debug.Log("[PermissionRequestUI] UdonPermission がリンクされていません。");
            }
        }

        /*
        * User Data の読み書きは OnPlayerRestored が呼ばれるまで待つ必要がある
        * https://vrc-beta-docs.netlify.app/worlds/udon/persistence/player-data/#events
        */
        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            // 「他者がロード完了したよ」は無視
            if (!player.isLocal) return;
            // 他者のデータは無視
            if (!Networking.IsOwner(gameObject)) return;

            if (_hasPermission) {
                udonPermission.GivePermission();
            } else {
                udonPermission.RevokePermission();
            }
            udonPermission.RegisterCallback(this);
        }

        public override void OnPermissionGiven()
        {
            if (_hasPermission) return;
            _hasPermission = true;
            RequestSerialization();
        }

        public override void OnPermissionRevoked()
        {
            if (!_hasPermission) return;
            _hasPermission = false;
            RequestSerialization();
        }
    }
}
