using UdonSharp;

namespace Aruma256.UdonPermission
{
    public abstract class PermissionCallbackBase : UdonSharpBehaviour
    {
        public virtual void OnPermissionGiven() { }
        public virtual void OnPermissionRevoked() { }
    }
}
