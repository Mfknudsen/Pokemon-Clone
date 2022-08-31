namespace Runtime.Player.Camera
{
    public class CameraSettings
    {
        public readonly float fov;

        private CameraSettings(float fov)
        {
            this.fov = fov;
        }

        public static CameraSettings Default()
        {
            return new CameraSettings(70);
        }
    }

}
