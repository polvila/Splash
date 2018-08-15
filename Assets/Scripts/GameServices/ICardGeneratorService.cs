
using UnityEngine;

public interface ICardGeneratorService
{
    void GetRandomCard();
    void GetPseudoRandomCard(int[] excludedNumbers);
}
