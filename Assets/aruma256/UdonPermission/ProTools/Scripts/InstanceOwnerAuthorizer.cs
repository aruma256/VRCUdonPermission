using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class InstanceOwnerAuthorizer : UdonSharpBehaviour
{
    [SerializeField] UdonPermission udonPermission;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(instanceOwnerName))] private string _instanceOwnerName = "";
    private string instanceOwnerName
    {
        get => _instanceOwnerName;
        set
        {
            _instanceOwnerName = value;
            OnInstanceOwnerNameSet();
        }
    }

    void Start()
    {
        if (udonPermission == null) {
            Debug.Log("[InstanceOwnerAuthorizer] UdonPermission がリンクされていません。");
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (udonPermission == null) return;
        if (!player.isLocal) return;
        if (player.playerId != 1) return;
        //
        BecomeOwner();
        instanceOwnerName = Networking.LocalPlayer.displayName;
        RequestSerialization();
    }

    private void OnInstanceOwnerNameSet()
    {
        if (udonPermission == null) return;
        if (instanceOwnerName != Networking.LocalPlayer.displayName) return;
        udonPermission.GivePermission();
    }

    // Utility Functions

    private bool IsOwner() => Networking.IsOwner(gameObject);
    private void BecomeOwner()
    {
        if (IsOwner()) return;
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }
}
