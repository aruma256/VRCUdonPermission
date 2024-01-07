using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class GrantPermissionUI : UdonSharpBehaviour
{
    [Header("UdonPermissionへのリンク")]
    [SerializeField] UdonPermission udonPermission;
    [Header("文言 - 正しく権限を付与された場合")]
    [SerializeField] string statusTextWhenGranted = "権限を付与されました。";
    [Header("文言 - すでに権限を持っている場合")]
    [SerializeField] string statusTextWhenAlreadyGranted = "すでに権限を持っています。";
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
        if (udonPermission.HasPermission()) {
            statusText.text = statusTextWhenAlreadyGranted;
            return;
        }
        statusText.text = "";
        udonPermission.GivePermission();
        statusText.text = statusTextWhenGranted;
    }
}
