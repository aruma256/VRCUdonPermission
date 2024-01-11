using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PermissionSwitch : UdonSharpBehaviour
{
    [Header("UdonPermissionへのリンク")]
    [SerializeField] UdonPermission udonPermission;
    [Header("Useで権限を付与する")]
    [SerializeField] bool givePermissionOnUse = true;
    [Header("Useで権限を剥奪する")]
    [SerializeField] bool revokePermissionOnUse = true;

    void Start()
    {
        if (udonPermission == null) {
            Debug.Log("ワールドエラー: UdonPermission がリンクされていません。");
        }
    }

    public override void Interact()
    {
        if (udonPermission == null) return;
        if (revokePermissionOnUse && udonPermission.HasPermission()) {
            udonPermission.RevokePermission();
        } else if (givePermissionOnUse && !udonPermission.HasPermission()) {
            udonPermission.GivePermission();
        }
    }
}
