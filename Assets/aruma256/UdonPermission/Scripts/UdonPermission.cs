using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[DefaultExecutionOrder(-1000)]
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class UdonPermission : UdonSharpBehaviour
{
    [Header("権限持ちのみ ON にするレンダラー（スタッフのみ見えるなど）")]
    [SerializeField] private Renderer[] restrictedRenderers;
    [Header("権限持ちのみ ON にするコライダー/トリガー（スタッフのみ乗れる/持てる・使えるなど）")]
    [SerializeField] private Collider[] restrictedColliders;
    [Header("権限持ちのみ ON にするオブジェクト")]
    [SerializeField] private GameObject[] restrictedObjects;
    [Header("権限持ちのみ OFF にするレンダラー（非スタッフ向けの表示など）")]
    [SerializeField] private Renderer[] invertedRestrictedRenderers;
    [Header("権限持ちのみ OFF にするコライダー/トリガー（非スタッフの立入禁止など）")]
    [SerializeField] private Collider[] invertedRestrictedColliders;
    [Header("権限持ちのみ OFF にするオブジェクト")]
    [SerializeField] private GameObject[] invertedRestrictedObjects;
    //
    [Header("権限を与える対象アカウントの名前リスト")]
    [SerializeField] private string[] targetAccountNames;

    private bool[] _permissionContainer = new bool[1] { false };

    void Start()
    {
        foreach (string name in targetAccountNames)
        {
            if (Networking.LocalPlayer.displayName == name) {
                GivePermission();
                break;
            }
        }
        Apply();
    }

    private void Apply()
    {
        // ON
        foreach (Renderer renderer in restrictedRenderers)
        {
            if (renderer != null) renderer.enabled = _HasPermission();
        }
        foreach (Collider collider in restrictedColliders)
        {
            if (collider != null) collider.enabled = _HasPermission();
        }
        foreach (GameObject obj in restrictedObjects)
        {
            if (obj != null) obj.SetActive(_HasPermission());
        }
        // OFF
        foreach (Renderer renderer in invertedRestrictedRenderers)
        {
            if (renderer != null) renderer.enabled = !_HasPermission();
        }
        foreach (Collider collider in invertedRestrictedColliders)
        {
            if (collider != null) collider.enabled = !_HasPermission();
        }
        foreach (GameObject obj in invertedRestrictedObjects)
        {
            if (obj != null) obj.SetActive(!_HasPermission());
        }
    }

    private bool _HasPermission()
    {
        return _permissionContainer[0];
    }

    public bool HasPermission()
    {
        return _HasPermission();
    }

    public void GivePermission()
    {
        _permissionContainer[0] = true;
        Apply();
    }

    public bool[] GetPermissionContainer()
    {
        return _permissionContainer;
    }
}
