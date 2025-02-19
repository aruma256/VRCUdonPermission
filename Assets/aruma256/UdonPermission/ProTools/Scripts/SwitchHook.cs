using UdonSharp;
using UnityEngine;

namespace Aruma256.UdonPermission
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SwitchHook : UdonSharpBehaviour
    {
        [Header("UdonPermissionへのリンク")]
        [SerializeField] private UdonPermission udonPermission;

        private void OnEnable()
        {
            udonPermission.GivePermission();
        }

        private void OnDisable()
        {
            udonPermission.RevokePermission();
        }
    }
}
