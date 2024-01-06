using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[DefaultExecutionOrder(-1000)]
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class UdonPermission : UdonSharpBehaviour
{
    [Header("権限持ちのみ ON にするレンダラー（スタッフのみ見えるなど）")]
    [SerializeField] private Renderer[] restrictedRenderers;
    [Header("権限持ちのみ ON にするコライダー/トリガー（スタッフのみ乗れる/持てる・使えるなど）")]
    [SerializeField] private Collider[] restrictedColliders;
    [Header("権限持ちのみ ON にするオブジェクト")]
    [SerializeField] private GameObject[] restrictedObjects;
    [Header("権限持ちのみ OFF にするレンダラー（非スタッフ向けの表示など）")]
    [SerializeField] private Renderer[] invertedRestrictedRenderers;
    [Header("権限持ちのみ OFF にするコライダー/トリガー（非スタッフの立入禁止など）")]
    [SerializeField] private Collider[] invertedRestrictedColliders;
    [Header("権限持ちのみ OFF にするオブジェクト")]
    [SerializeField] private GameObject[] invertedRestrictedObjects;
    //
    [Header("権限を与える対象アカウントの名前リスト")]
    [SerializeField] private string[] targetAccountNames;
    [Header("スタッフの名前リスト(外部URLから取得)")]
    [SerializeField] private VRCUrl url;

    private bool hasPermission = false;

    /*
    * Start
    * v (v async)
    * v OnStringLoadSuccess/OnStringLoadError
    * v v
    * InitWithNameList
    *   - UpdatePermissionFlag
    *   - Apply
    */

    void Start()
    {
        if (url.Get() == "") {
            InitWithNameList(targetAccountNames);
        } else {
            VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
        }
    }

    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        string[] loadedNameList = ParseNameListJson(result.Result);
        bool succeeded = (loadedNameList != null);
        if (succeeded) {
            InitWithNameList(loadedNameList);
        } else {
            DebugLog("Unity内のリストを使用します。");
            InitWithNameList(targetAccountNames);
        }
    }

    public override void OnStringLoadError(IVRCStringDownload result)
    {
        DebugLog("指定のURLへアクセスできませんでした。");
        DebugLog("Unity内のリストを使用します。");
        DebugLog(result.Error);
        InitWithNameList(targetAccountNames);
    }

    private void InitWithNameList(string[] names)
    {
        UpdatePermissionFlag(names);
        Apply();
    }

    private void UpdatePermissionFlag(string[] names)
    {
        foreach (string name in names)
        {
            if (Networking.LocalPlayer.displayName == name) hasPermission = true;
        }
    }

    private void Apply()
    {
        // ON
        foreach (Renderer renderer in restrictedRenderers)
        {
            if (renderer != null) renderer.enabled = hasPermission;
        }
        foreach (Collider collider in restrictedColliders)
        {
            if (collider != null) collider.enabled = hasPermission;
        }
        foreach (GameObject obj in restrictedObjects)
        {
            if (obj != null) obj.SetActive(hasPermission);
        }
        // OFF
        foreach (Renderer renderer in invertedRestrictedRenderers)
        {
            if (renderer != null) renderer.enabled = !hasPermission;
        }
        foreach (Collider collider in invertedRestrictedColliders)
        {
            if (collider != null) collider.enabled = !hasPermission;
        }
        foreach (GameObject obj in invertedRestrictedObjects)
        {
            if (obj != null) obj.SetActive(!hasPermission);
        }
    }

    // Utils

    private static string[] ParseNameListJson(string json)
    {
        bool succeeded = VRCJson.TryDeserializeFromJson(json, out DataToken token);
        if (!succeeded) {
            DebugLog("JSONとして読み込めませんでした。");
            return null;
        }
        if (token.TokenType != TokenType.DataList) {
            DebugLog("読み込みに失敗しました。リスト形式ではありません。");
            DebugLog("[\"名前1\", \"名前2\", ...] のような形式である必要があります。");
            return null;
        }
        DataList dataList = token.DataList;
        string[] targetAccountNames = new string[dataList.Count];
        for (int i = 0; i < dataList.Count; i++)
        {
            targetAccountNames[i] = dataList[i].String;
            DebugLog(targetAccountNames[i]);
        }
        DebugLog("読み込みに成功しました (" + dataList.Count + "件)");
        return targetAccountNames;
    }

    private static void DebugLog(string message)
    {
        Debug.Log("[UdonPermission] " + message);
    }

    public bool HasPermission()
    {
        return hasPermission;
    }

    public void GivePermission()
    {
        hasPermission = true;
        Apply();
    }
}
