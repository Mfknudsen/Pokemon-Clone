namespace Mfknudsen.Player.Camera
{
    public class CameraSettings
    {
        public readonly float FOV;

        private CameraSettings(float fov)
        {
            FOV = fov;
        }

        public static CameraSettings Default()
        {
            return new CameraSettings(70);
        }
    }

}
