using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class TempStaff : UdonSharpBehaviour
{
    [SerializeField] UdonEventStaff udonEventStaff;
    [SerializeField] Text miniText;
    [SerializeField] InputField inputField;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(tempStaffNameListJson))] private string _tempStaffNameListJson = "[]";
    private string tempStaffNameListJson
    {
        get => _tempStaffNameListJson;
        set
        {
            _tempStaffNameListJson = value;
            OnTempStaffListChange();
        }
    }

    void Start()
    {
        if (udonEventStaff == null) {
            miniText.text = "ワールドエラー: UdonEventStaff が空欄です。";
        }
    }

    public void OnInputName()
    {
        miniText.text = "";
        if (udonEventStaff == null) return;
        if (!udonEventStaff.GetAmIStaff()) {
            miniText.text = "あなたはスタッフではないため、スタッフを追加できません。";
            return;
        }
        string name = inputField.text;
        if (name == "") return;
        inputField.text = "";
        if (!PlayerExistsInInstance(name)) {
            miniText.text = name + " はインスタンスに存在しません。";
            return;
        }
        if (PlayerExistsInTempStaffList(name)) {
            miniText.text = name + " は追加済みです。";
            return;
        }
        miniText.text = name + " をスタッフに追加します。";
        BecomeOwner();
        AddToTempStaffList(name);
        RequestSerialization();
    }

    private void OnTempStaffListChange()
    {
        if (udonEventStaff == null) return;
        if (tempStaffNameListJson == "") return;
        if (PlayerExistsInTempStaffList(Networking.LocalPlayer.displayName)) {
            udonEventStaff.BecomeStaff();
        }
    }

    // Utility Functions

    private bool IsOwner() => Networking.IsOwner(gameObject);
    private void BecomeOwner()
    {
        if (IsOwner()) return;
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    private static bool PlayerExistsInInstance(string name)
    {
        VRCPlayerApi[] players = VRCPlayerApi.GetPlayers(new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()]);
        foreach (VRCPlayerApi player in players) {
            if (player.displayName == name) {
                return true;
            }
        }
        return false;
    }

    private bool PlayerExistsInTempStaffList(string name)
    {
        VRCJson.TryDeserializeFromJson(tempStaffNameListJson, out DataToken token);
        DataList dataList = token.DataList;
        return dataList.Contains(new DataToken(name));
    }

    private void AddToTempStaffList(string name)
    {
        VRCJson.TryDeserializeFromJson(tempStaffNameListJson, out DataToken token);
        DataList dataList = token.DataList;
        dataList.Add(new DataToken(name));
        VRCJson.TrySerializeToJson(dataList, JsonExportType.Minify, out DataToken json);
        tempStaffNameListJson = json.String;
    }
}
