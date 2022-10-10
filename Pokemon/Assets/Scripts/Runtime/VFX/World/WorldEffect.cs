namespace Runtime.VFX.World
{
    public abstract class WorldEffect : EffectBase
    {
        #region Values

        protected bool isSwitching, isActive;

        public bool disableRules;

        #endregion

        #region Getters

        public bool IsActive => this.isActive;

        #endregion
        
        #region In

        public virtual void CheckRules()
        {
            this.isSwitching = true;
            
            if (this.isActive)
                this.Play();
            else
                this.Stop();
        }

        #endregion
    }
}
