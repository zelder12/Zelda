using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimacionesUI : MonoBehaviour
{
    [SerializeField] private GameObject logo;

    private void Start()
    {
        CanvasGroup canvasGroup = logo.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = logo.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0; 

        LeanTween.alphaCanvas(canvasGroup, 1, 20f); 
    }
}
