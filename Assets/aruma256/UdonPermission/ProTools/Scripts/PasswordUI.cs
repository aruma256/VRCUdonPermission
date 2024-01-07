using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PasswordUI : UdonSharpBehaviour
{
    [Header("UdonPermissionへのリンク")]
    [SerializeField] UdonPermission udonPermission;
    [Header("パスワード")]
    [SerializeField] string password;
    [Header("文言 - 正しいパスワードの場合")]
    [SerializeField] string statusTextWhenGranted = "権限を付与されました。";
    [Header("文言 - 正しくないパスワードの場合")]
    [SerializeField] string statusTextWhenWrongPassword = "<color=red>パスワードが違います。5秒後にリトライしてください。</color>";
    [Header("文言 - すでに権限を持っている場合")]
    [SerializeField] string statusTextWhenAlreadyGranted = "すでに権限を持っています。";
    [Header("内部的に利用するリンク")]
    [SerializeField] Text statusText;
    [SerializeField] InputField inputField;

    void Start()
    {
        if (udonPermission == null) {
            statusText.text = "ワールドエラー: UdonPermission がリンクされていません。";
        }
    }

    public void OnInput()
    {
        if (udonPermission == null) return;
        if (udonPermission.HasPermission()) {
            statusText.text = statusTextWhenAlreadyGranted;
            inputField.gameObject.SetActive(false);
            return;
        }
        statusText.text = "";
        string userInput = inputField.text;
        if (userInput == password) {
            udonPermission.GivePermission();
            statusText.text = statusTextWhenGranted;
            inputField.gameObject.SetActive(false);
        } else {
            statusText.text = statusTextWhenWrongPassword;
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
