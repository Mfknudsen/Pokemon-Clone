#region SDK

using System.Collections;

#endregion

namespace Mfknudsen.Pokémon.Conditions.Volatile
{
    public class Wrap : Condition, IVolatile
    {
        public override void Reset()
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerator ActivateCondition(ConditionOversight activator)
        {
            throw new System.NotImplementedException();
        }

        public bool CanIncrease()
        {
            return false;
        }

        public void Increase(Condition condition)
        {
            throw new System.NotImplementedException();
        }
    }
}
