namespace Runtime.Systems.PersistantRunner
{
    public interface IFrameUpdate
    {
        public void Start();
        public void Update();
        public void LateUpdate();
    }
}
