using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class RandomExcludingGenerator : RandomGenerator
{
    private HashSet<int> excludedNumbers;
    
    public override int GenerateNumber(int minRange, int maxRange)
    {
        //TODO: REDO 
        
        if (excludedNumbers == null)
        {
            excludedNumbers = new HashSet<int>();
        }

        var range = Enumerable.Range(minRange, maxRange);
        var index = Random.Range(minRange - 1, maxRange - excludedNumbers.Count);
        return range.ElementAt(index);
    }
}