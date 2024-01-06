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
            statusText.text = "ワールドエラー: UdonEventStaff がリンクされていません。";
        }
    }

    public void OnClick()
    {
        if (udonPermission == null) return;
        if (udonPermission.AmIStaff()) {
            statusText.text = "すでにスタッフです。";
            button.interactable = false;
            return;
        }
        statusText.text = "";
        udonPermission.BecomeStaff();
        statusText.text = "スタッフになりました。";
        button.interactable = false;
    }
}
