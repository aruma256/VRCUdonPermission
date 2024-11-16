using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Aruma256.UdonPermission
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RevokePermissionUI : UdonSharpBehaviour
    {
        [Header("UdonPermissionへのリンク")]
        [SerializeField] UdonPermission udonPermission;
        [Header("文言 - 正しく権限を剥奪された場合")]
        [SerializeField] string statusTextWhenRevoked = "権限を削除しました。";
        [Header("文言 - すでに権限を持っている場合")]
        [SerializeField] string statusTextWhenAlreadyRevoked = "すでに権限がありません。";
        [Header("内部的に利用するリンク")]
        [SerializeField] Button button;
        [SerializeField] Text statusText;

        void Start()
        {
            if (udonPermission == null) {
                statusText.text = "ワールドエラー: UdonPermission がリンクされていません。";
            }
        }

        public void OnClick()
        {
            if (udonPermission == null) return;
            if (!udonPermission.HasPermission()) {
                statusText.text = statusTextWhenAlreadyRevoked;
                return;
            }
            statusText.text = "";
            udonPermission.RevokePermission();
            statusText.text = statusTextWhenRevoked;
        }
    }
}
