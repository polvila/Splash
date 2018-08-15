
using System.Collections.Generic;
using UnityEngine;

public interface ICardGeneratorService
{
    CardView GetRandomCard();
    CardView GetPseudoRandomCard(HashSet<int> excludedNumbers);
}
