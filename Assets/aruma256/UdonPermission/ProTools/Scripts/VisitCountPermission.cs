using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Aruma256.UdonPermission
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VisitCountPermission : UdonSharpBehaviour
    {
        [Header("UdonPermissionへのリンク")]
        [SerializeField] private UdonPermission udonPermission;
        [Header("◯回以上の訪問で権限を付与")]
        [SerializeField] private ulong requiredVisitCount = 3;

        void Start()
        {
            if (udonPermission == null)
            {
                Debug.Log("[VisitCountPermission] UdonPermission がリンクされていません。");
            }
        }

        public void ReceiveVisitCount(ulong visitCount)
        {
            if (udonPermission == null) return;

            // 訪問回数が条件を満たしているかチェック
            if (visitCount >= requiredVisitCount)
            {
                udonPermission.GivePermission();
            }
        }
    }
}
