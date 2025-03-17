using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using VRC.SDK3.Persistence;

namespace Aruma256.UdonPermission
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VisitCounter : UdonSharpBehaviour
    {
        [Header("通知先のVisitCountPermission")]
        [SerializeField] private VisitCountPermission[] visitCountPermissions;
        [Header("同日の再訪問をカウントしない")]
        [SerializeField] private bool ignoreSameDayVisits = false;
        [Header("訪問回数表示用のText（オプション）")]
        [SerializeField] private TextMeshProUGUI visitCountText;
        [Header("PlayerDataのキー設定（任意）")]
        [SerializeField] private string visitCountKey = "UdonPermission_VisitCount";
        [SerializeField] private string lastVisitDateKey = "UdonPermission_LastVisitDate";
        
        private ulong _visitCount = 0;
        private string _lastVisitDateTime = "";

        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            // 「他者がロード完了したよ」は無視
            if (!player.isLocal) return;

            // PlayerDataから訪問回数と最終訪問日時を取得
            _visitCount = PlayerData.GetULong(player, visitCountKey);
            _lastVisitDateTime = PlayerData.GetString(player, lastVisitDateKey) ?? "0000-00-00 00:00:00";

            string now = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string today = now.Substring(0, 10); // "yyyy-MM-dd" part
            bool shouldCount = !ignoreSameDayVisits || _lastVisitDateTime.CompareTo(today) < 0;

            if (shouldCount)
            {
                _visitCount++;
                // PlayerDataに訪問回数を保存
                PlayerData.SetULong(visitCountKey, _visitCount);
            }

            // PlayerDataに最終訪問日時を保存
            PlayerData.SetString(lastVisitDateKey, now);
            
            // 訪問回数を表示
            UpdateVisitCountDisplay();
            
            // 各VisitCountPermissionに訪問回数を通知
            NotifyVisitCountPermissions();
        }

        private void UpdateVisitCountDisplay()
        {
            if (visitCountText == null) return;
            visitCountText.text = _visitCount.ToString();
        }
        
        private void NotifyVisitCountPermissions()
        {
            if (visitCountPermissions == null) return;
            
            foreach (VisitCountPermission permission in visitCountPermissions)
            {
                if (permission != null)
                {
                    permission.ReceiveVisitCount(_visitCount);
                }
            }
        }
    }
}
