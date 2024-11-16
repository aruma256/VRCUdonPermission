
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace Aruma256.UdonPermission
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class URLNameListLoader : UdonSharpBehaviour
    {
        [Header("UdonPermissionへのリンク")]
        [SerializeField] UdonPermission udonPermission;
        [Header("権限を与える対象アカウントの名前リストURL(JSON)")]
        [SerializeField] private VRCUrl url;

        void Start()
        {
            if (udonPermission == null) {
                Debug.Log("ワールドエラー: UdonPermission がリンクされていません。");
            }
            if (url.Get() == "") {
                Debug.Log("[URLNameListLoader] URLがセットされていません。");
                return;
            }
            VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            string[] loadedNameList = ParseNameListJson(result.Result);
            bool succeeded = (loadedNameList != null);
            if (!succeeded) return;
            foreach (string name in loadedNameList)
            {
                if (Networking.LocalPlayer.displayName == name) {
                    udonPermission.GivePermission();
                    break;
                }
            }
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            Debug.Log("[URLNameListLoader] 指定のURLへのアクセスに失敗しました。");
            Debug.Log(result.Error);
        }

        // Utils

        private static string[] ParseNameListJson(string json)
        {
            bool succeeded = VRCJson.TryDeserializeFromJson(json, out DataToken token);
            if (!succeeded) {
                Debug.Log("[URLNameListLoader] JSONとして読み込めませんでした。");
                return null;
            }
            if (token.TokenType != TokenType.DataList) {
                Debug.Log("[URLNameListLoader] 読み込みに失敗しました。リスト形式ではありません。[\"名前1\", \"名前2\", ...] のような形式である必要があります。");
                return null;
            }
            DataList dataList = token.DataList;
            string[] targetAccountNames = new string[dataList.Count];
            for (int i = 0; i < dataList.Count; i++)
            {
                targetAccountNames[i] = dataList[i].String;
            }
            Debug.Log("[URLNameListLoader] 読み込みに成功しました (" + dataList.Count + "件)");
            return targetAccountNames;
        }
    }
}
