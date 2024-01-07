using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PermissionRequestUI : UdonSharpBehaviour
{
    [Header("UdonPermissionへのリンク")]
    [SerializeField] UdonPermission udonPermission;
    [Header("文言 - 権限をリクエストしている場合")]
    [SerializeField] string statusTextWhenRequesting = "以下のプレイヤーがスタッフ権限をリクエストしています";
    [Header("内部的に利用するリンク")]
    [SerializeField] Text statusText;
    [SerializeField] Button requestButton;
    [SerializeField] Button rejectButton;
    [SerializeField] Button acceptButton;

    private const int UNSET = -1;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(requestingPlayerId))] private int _requestingPlayerId = UNSET;
    private int requestingPlayerId
    {
        get => _requestingPlayerId;
        set
        {
            _requestingPlayerId = value;
            OnRequestingPlayerIdChanged();
        }
    }
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(acceptedPlayerId))] private int _acceptedPlayerId = UNSET;
    private int acceptedPlayerId
    {
        get => _acceptedPlayerId;
        set
        {
            _acceptedPlayerId = value;
            OnAcceptedPlayerIdChanged();
        }
    }

    void Start()
    {
        if (udonPermission == null) {
            Debug.Log("[PermissionRequestUI] UdonPermission がリンクされていません。");
        }
    }

    public void OnRequestButtonClicked()
    {
        if (udonPermission == null) return;
        if (requestingPlayerId != UNSET) return;
        if (udonPermission.HasPermission()) {
            return;
        }
        //
        BecomeOwner();
        requestingPlayerId = Networking.LocalPlayer.playerId;
        RequestSerialization();
    }

    public void OnRejectButtonClicked()
    {
        if (udonPermission == null) return;
        if (!udonPermission.HasPermission()) return;
        //
        BecomeOwner();
        requestingPlayerId = UNSET;
        RequestSerialization();
    }

    public void OnAcceptButtonClicked()
    {
        if (udonPermission == null) return;
        if (!udonPermission.HasPermission()) return;
        //
        BecomeOwner();
        acceptedPlayerId = requestingPlayerId;
        requestingPlayerId = UNSET;
        RequestSerialization();
    }

    private void OnRequestingPlayerIdChanged()
    {
        if (requestingPlayerId != UNSET) {
            VRCPlayerApi requestingPlayer = VRCPlayerApi.GetPlayerById(requestingPlayerId);
            if (requestingPlayer != null) {
                UpdateUIAsRequestingMode(requestingPlayer);
            } else {
                UpdateUIAsIdleMode();
            }
        } else {
            UpdateUIAsIdleMode();
        }
    }

    private void OnAcceptedPlayerIdChanged()
    {
        if (acceptedPlayerId == Networking.LocalPlayer.playerId) {
            udonPermission.GivePermission();
        }
    }

    private void UpdateUIAsIdleMode()
    {
        statusText.text = "";
        requestButton.interactable = true;
        acceptButton.interactable = false;
        rejectButton.interactable = false;
    }

    private void UpdateUIAsRequestingMode(VRCPlayerApi requestingPlayer)
    {
        statusText.text = statusTextWhenRequesting + "\n" + requestingPlayer.displayName;
        requestButton.interactable = false;
        acceptButton.interactable = true;
        rejectButton.interactable = true;
    }

    // Utility Functions

    private bool IsOwner() => Networking.IsOwner(gameObject);
    private void BecomeOwner()
    {
        if (IsOwner()) return;
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }
}
