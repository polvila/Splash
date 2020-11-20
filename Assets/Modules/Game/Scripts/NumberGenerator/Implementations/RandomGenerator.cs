using Random = UnityEngine.Random;

namespace Modules.Game
{
    public class RandomGenerator
    {
        public virtual void Reset()
        {
        }
        
        public virtual int GenerateNumber(int minRange, int maxRange)
        {
            return Random.Range(minRange, maxRange + 1);
        }
    }
}