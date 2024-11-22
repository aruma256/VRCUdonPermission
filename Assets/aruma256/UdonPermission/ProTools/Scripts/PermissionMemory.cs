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
            // Playerの数だけ複製され実行される。他人のデータは無視し、コールバック登録もしない
            if (!IsOwnerOfThisObject()) return; 

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

        private bool IsOwnerOfThisObject() => Networking.IsOwner(gameObject);
    }
}
