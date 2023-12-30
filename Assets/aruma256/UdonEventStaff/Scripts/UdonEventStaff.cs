using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class UdonEventStaff : UdonSharpBehaviour
{
    [Header("スタッフのみ ON にするオブジェクト")]
    [SerializeField] private GameObject[] staffOnlyOnObjects;
    [Header("スタッフのみ ON にするコライダー/トリガー（スタッフのみ持てる など）")]
    [SerializeField] private Collider[] staffOnlyOnColliders;
    [Header("スタッフのみ OFF にするオブジェクト")]
    [SerializeField] private GameObject[] staffOnlyOffObjects;
    [Header("スタッフのみ OFF にするコライダー/トリガー（スタッフのみ入れる など）")]
    [SerializeField] private Collider[] staffOnlyOffColliders;
    //
    [Header("スタッフの名前リスト(Unityで設定)")]
    [SerializeField] private string[] staffList;
    [Header("スタッフの名前リスト(外部URLから取得)")]
    [SerializeField] private VRCUrl url;

    private bool amIStaff = false;

    /*
    * Start
    * v (v async)
    * v OnStringLoadSuccess/OnStringLoadError
    * v v
    * InitWithStaffList
    *   - UpdateStaffFlag
    *   - Apply
    */

    void Start()
    {
        if (url.Get() == "") {
            InitWithStaffList(staffList);
        } else {
            VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
        }
    }

    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        string[] loadedStaffList = ParseStaffListJson(result.Result);
        bool succeeded = (loadedStaffList != null);
        if (succeeded) {
            InitWithStaffList(loadedStaffList);
        } else {
            DebugLog("Unity内のリストを使用します。");
            InitWithStaffList(staffList);
        }
    }

    public override void OnStringLoadError(IVRCStringDownload result)
    {
        DebugLog("指定のURLへアクセスできませんでした。");
        DebugLog("Unity内のリストを使用します。");
        DebugLog(result.Error);
        InitWithStaffList(staffList);
    }

    private void InitWithStaffList(string[] staffs)
    {
        UpdateStaffFlag(staffs);
        Apply();
    }

    private void UpdateStaffFlag(string[] staffs)
    {
        amIStaff = false;
        foreach (string staff in staffs)
        {
            if (Networking.LocalPlayer.displayName == staff) amIStaff = true;
        }
    }

    private void Apply()
    {
        foreach (GameObject obj in staffOnlyOnObjects)
        {
            if (obj != null) obj.SetActive(amIStaff);
        }
        foreach (Collider collider in staffOnlyOnColliders)
        {
            if (collider != null) collider.enabled = amIStaff;
        }
        foreach (GameObject obj in staffOnlyOffObjects)
        {
            if (obj != null) obj.SetActive(!amIStaff);
        }
        foreach (Collider collider in staffOnlyOffColliders)
        {
            if (collider != null) collider.enabled = !amIStaff;
        }
    }

    // Utils

    private static string[] ParseStaffListJson(string json)
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
        string[] staffList = new string[dataList.Count];
        for (int i = 0; i < dataList.Count; i++)
        {
            staffList[i] = dataList[i].String;
            DebugLog(staffList[i]);
        }
        DebugLog("読み込みに成功しました (" + dataList.Count + "件)");
        return staffList;
    }

    private static void DebugLog(string message)
    {
        Debug.Log("[UdonEventStaff] " + message);
    }
}
