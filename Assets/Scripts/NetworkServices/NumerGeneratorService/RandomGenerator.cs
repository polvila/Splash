using System.Collections.Generic;
using Random = UnityEngine.Random;

public class RandomGenerator
{
    public virtual int GenerateNumber(int minRange, int maxRange)
    {
        return Random.Range(minRange, maxRange + 1);
    }
}