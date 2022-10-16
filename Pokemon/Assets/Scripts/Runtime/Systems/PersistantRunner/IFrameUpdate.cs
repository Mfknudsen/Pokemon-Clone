namespace Runtime.Systems.PersistantRunner
{
    public interface IFrameUpdate
    {
        public void FrameStart();
        public void FrameUpdate();
        public void FrameLateUpdate();
    }
}
