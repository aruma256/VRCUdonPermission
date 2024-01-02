using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class StaffRegister_Auto : UdonSharpBehaviour
{
    [SerializeField] UdonEventStaff udonEventStaff;
    [SerializeField] Text miniText;
    [SerializeField] Button button;

    void Start()
    {
        if (udonEventStaff == null) {
            miniText.text = "ワールドエラー: UdonEventStaff がリンクされていません。";
        }
    }

    public void OnClick()
    {
        if (udonEventStaff == null) return;
        if (udonEventStaff.AmIStaff()) {
            miniText.text = "すでにスタッフです。";
            button.interactable = false;
            return;
        }
        miniText.text = "";
        udonEventStaff.BecomeStaff();
        miniText.text = "スタッフになりました。";
        button.interactable = false;
    }
}
