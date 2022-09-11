#region Packages

using Cinemachine;

#endregion

namespace Runtime.Common
{
    public static class CinemachineExtensions
    {
        public static TComponent CinemachineComponent<TComponent>(this CinemachineVirtualCameraBase virtualCamera)
            where TComponent : CinemachineComponentBase =>
            (virtualCamera as CinemachineVirtualCamera)?.GetCinemachineComponent<TComponent>();
    }
}