using Random = UnityEngine.Random;

namespace Modules.Game
{
    public class FTUEGenerator : RandomGenerator
    {
        private readonly int[] _firstNumbers;
        private int _index;

        public FTUEGenerator()
        {
            _firstNumbers = new[]
            {
                1, 10, 11, 11, 8, 3, 8, 11, 13, 7, //initial hand
                10, //7 moved
                2, //8 moved
                6, //2 moved
                4, //1 moved (enemy) 
                2, //13 moved
                6, //unblock with 6:7
                7,
                1, //6 moved
                1, //splash with 1:4
                4,
            };
        }

        public override int GenerateNumber(int minRange, int maxRange)
        {
            if (_index < _firstNumbers.Length)
            {
                var result = _firstNumbers[_index];
                ++_index;
                return result;
            }

            return Random.Range(minRange, maxRange + 1);
        }
    }
}