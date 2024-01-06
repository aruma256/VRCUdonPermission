using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class GrantPermissionUI : UdonSharpBehaviour
{
    [SerializeField] UdonPermission udonPermission;
    [SerializeField] Text statusText;
    [SerializeField] Button button;

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
            statusText.text = "すでにスタッフです。";
            button.interactable = false;
            return;
        }
        statusText.text = "";
        udonPermission.GivePermission();
        statusText.text = "スタッフになりました。";
        button.interactable = false;
    }
}
