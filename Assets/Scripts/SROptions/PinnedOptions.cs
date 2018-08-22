using UnityEngine;

public class PinnedOptions : MonoBehaviour 
{
    void Start()
    {
        SRDebug.Instance.PinOption("Reset Scene");
        SRDebug.Instance.PinOption("God Mode");
    }
}
