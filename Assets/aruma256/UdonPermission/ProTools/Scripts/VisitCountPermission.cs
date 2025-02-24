using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Aruma256.UdonPermission
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class VisitCountPermission : UdonSharpBehaviour
    {
        [Header("UdonPermissionへのリンク")]
        [SerializeField] private UdonPermission udonPermission;
        [Header("◯回以上の訪問で権限を付与")]
        [SerializeField] private int requiredVisitCount = 3;

        [UdonSynced(UdonSyncMode.None)] private int _visitCount = 0;

        void Start()
        {
            if (udonPermission == null)
            {
                Debug.Log("[VisitCountPermission] UdonPermission がリンクされていません。");
                return;
            }
        }

        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;

            _visitCount++;
            RequestSerialization();

            if (_visitCount >= requiredVisitCount)
            {
                udonPermission.GivePermission();
            }
        }

        public int GetVisitCount()
        {
            return _visitCount;
        }
    }
}
