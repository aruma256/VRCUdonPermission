using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class InstanceOwnerAuthorizer : UdonSharpBehaviour
{
    [SerializeField] UdonEventStaff udonEventStaff;

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
        if (udonEventStaff == null) {
            Debug.Log("[StaffRegister_Manual] UdonEventStaff がリンクされていません。");
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (udonEventStaff == null) return;
        if (!player.isLocal) return;
        if (player.playerId != 1) return;
        //
        BecomeOwner();
        instanceOwnerName = Networking.LocalPlayer.displayName;
        RequestSerialization();
    }

    private void OnInstanceOwnerNameSet()
    {
        if (udonEventStaff == null) return;
        if (instanceOwnerName != Networking.LocalPlayer.displayName) return;
        udonEventStaff.BecomeStaff();
    }

    // Utility Functions

    private bool IsOwner() => Networking.IsOwner(gameObject);
    private void BecomeOwner()
    {
        if (IsOwner()) return;
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }
}
