using UnityEngine;

public class PinnedOptions : MonoBehaviour 
{
    void Start()
    {
        SRDebug.Instance.PinOption("Main Menu");
        SRDebug.Instance.PinOption("God Mode");
        SRDebug.Instance.PinOption("Card Generator Mode");
    }
}
