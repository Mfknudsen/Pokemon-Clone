using System.Collections;
using Mfknudsen.Battle.Systems;

namespace Mfknudsen.Pokémon.Conditions.Volatiles
{
    public class Wrap : VolatileCondition, IOperation
    {
        public override bool CanIncrease()
        {
            return false;
        }

        public override void Increase(Condition condition)
        {
            throw new System.NotImplementedException();
        }

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }

        public bool Done()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Operation()
        {
            throw new System.NotImplementedException();
        }

        public void End()
        {
        }
    }
}
