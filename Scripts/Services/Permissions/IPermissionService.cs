namespace TheOneStudio.UITemplate.UITemplate.Services.Permissions
{
    using Cysharp.Threading.Tasks;

    public interface IPermissionService
    {
        UniTask<bool> RequestPermission(PermissionRequest request);
    }
}