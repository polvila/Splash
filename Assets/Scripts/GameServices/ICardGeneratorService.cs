
using System.Collections.Generic;
using UnityEngine;

public interface ICardGeneratorService
{
    int GetMaxRange { get; }
    int GetMinRange { get; }
    CardView GetRandomCard();
    CardView GetPseudoRandomCard(HashSet<int> excludedNumbers);
}
