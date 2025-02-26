using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using VRC.SDK3.Persistence;

namespace Aruma256.UdonPermission
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VisitCountPermission : UdonSharpBehaviour
    {
        [Header("UdonPermissionへのリンク")]
        [SerializeField] private UdonPermission udonPermission;
        [Header("◯回以上の訪問で権限を付与")]
        [SerializeField] private int requiredVisitCount = 3;
        [Header("同日の再訪問をカウントしない")]
        [SerializeField] private bool ignoreSameDayVisits = false;
        [Header("訪問回数表示用のText（オプション）")]
        [SerializeField] private TextMeshProUGUI visitCountText;
        [Header("PlayerDataのキー設定（任意）")]
        [SerializeField] private string visitCountKey = "UdonPermission_VisitCount";
        [SerializeField] private string lastVisitDateKey = "UdonPermission_LastVisitDate";
        
        private int _visitCount = 0;
        private string _lastVisitDateTime = "";

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

            // PlayerDataから訪問回数と最終訪問日時を取得
            _visitCount = PlayerData.GetInt(player, visitCountKey);
            _lastVisitDateTime = PlayerData.GetString(player, lastVisitDateKey) ?? "";

            string now = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string today = now.Substring(0, 10); // "yyyy-MM-dd" part
            bool shouldCount = !ignoreSameDayVisits || !_lastVisitDateTime.StartsWith(today);

            if (shouldCount)
            {
                _visitCount++;
                // PlayerDataに訪問回数を保存
                PlayerData.SetInt(visitCountKey, _visitCount);

                if (_visitCount >= requiredVisitCount)
                {
                    udonPermission.GivePermission();
                }
            }

            // PlayerDataに最終訪問日時を保存
            PlayerData.SetString(lastVisitDateKey, now);
            
            UpdateVisitCountDisplay();
        }

        private void UpdateVisitCountDisplay()
        {
            if (visitCountText == null) return;
            visitCountText.text = _visitCount.ToString();
        }

        public int GetVisitCount()
        {
            return _visitCount;
        }
    }
}
