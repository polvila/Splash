using System.Collections.Generic;
using UnityEngine;

namespace Modules.Game
{
    [CreateAssetMenu(fileName = "FTUESequenceAsset", menuName = "FTUE/Sequence", order = 1)]
    public class FTUESequence : ScriptableObject
    {
        public List<FTUEScreenConfig> ScreenSequence;
    }
}