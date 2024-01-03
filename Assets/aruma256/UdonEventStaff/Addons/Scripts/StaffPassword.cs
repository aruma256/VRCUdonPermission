using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class StaffPassword : UdonSharpBehaviour
{
    [SerializeField] UdonEventStaff udonEventStaff;
    [SerializeField] Text statusText;
    [SerializeField] InputField inputField;
    [SerializeField] string password;

    void Start()
    {
        if (udonEventStaff == null) {
            statusText.text = "ワールドエラー: UdonEventStaff がリンクされていません。";
        }
    }

    public void OnInput()
    {
        if (udonEventStaff == null) return;
        if (udonEventStaff.AmIStaff()) {
            statusText.text = "すでにスタッフです。";
            inputField.gameObject.SetActive(false);
            return;
        }
        statusText.text = "";
        string userInput = inputField.text;
        if (userInput == password) {
            udonEventStaff.BecomeStaff();
            statusText.text = "スタッフになりました。";
            inputField.gameObject.SetActive(false);
        } else {
            statusText.text = "<color=red>パスワードが違います。5秒後にリトライしてください。</color>";
            inputField.gameObject.SetActive(false);
            SendCustomEventDelayedSeconds(nameof(ShowInputFieldAgain), 5);
        }
        inputField.text = "";
    }

    public void ShowInputFieldAgain()
    {
        inputField.gameObject.SetActive(true);
        statusText.text = "";
    }
}
