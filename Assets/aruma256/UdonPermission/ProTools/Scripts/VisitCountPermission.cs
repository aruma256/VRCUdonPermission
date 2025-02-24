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
        [Header("同日の再訪問をカウントしない")]
        [SerializeField] private bool ignoreSameDayVisits = false;

        [UdonSynced(UdonSyncMode.None)] private int _visitCount = 0;
        [UdonSynced(UdonSyncMode.None)] private string _lastVisitDateTime = "";

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
            // 「他者がロード完了したよ」は無視
            if (!player.isLocal) return;
            // 他者のデータは無視
            if (!Networking.IsOwner(gameObject)) return;

            string now = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string today = now.Substring(0, 10); // "yyyy-MM-dd" part
            bool shouldCount = !ignoreSameDayVisits || !_lastVisitDateTime.StartsWith(today);

            if (shouldCount)
            {
                _visitCount++;
                if (_visitCount >= requiredVisitCount)
                {
                    udonPermission.GivePermission();
                }
            }

            _lastVisitDateTime = now;
            RequestSerialization();
        }

        public int GetVisitCount()
        {
            return _visitCount;
        }
    }
}
