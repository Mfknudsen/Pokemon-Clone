namespace Mfknudsen.Settings
{
    public interface ISetup
    {
        public int Priority();
        public void Setup();
    }
}