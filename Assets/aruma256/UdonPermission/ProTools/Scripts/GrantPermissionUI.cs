using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class GrantPermissionUI : UdonSharpBehaviour
{
    [SerializeField] UdonEventStaff udonEventStaff;
    [SerializeField] Text statusText;
    [SerializeField] Button button;

    void Start()
    {
        if (udonEventStaff == null) {
            statusText.text = "ワールドエラー: UdonEventStaff がリンクされていません。";
        }
    }

    public void OnClick()
    {
        if (udonEventStaff == null) return;
        if (udonEventStaff.AmIStaff()) {
            statusText.text = "すでにスタッフです。";
            button.interactable = false;
            return;
        }
        statusText.text = "";
        udonEventStaff.BecomeStaff();
        statusText.text = "スタッフになりました。";
        button.interactable = false;
    }
}
