using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingView : MonoBehaviour
{
    [SerializeField] private GameObject _loadingImage;
    
    void Start()
    {
        LeanTween.rotateAround(_loadingImage, Vector3.back, 360, 1f).setLoopClamp();
    }
}
