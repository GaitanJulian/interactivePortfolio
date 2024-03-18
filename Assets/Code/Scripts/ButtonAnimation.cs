using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour
{
    Button btn;
    Vector3 upScale = new Vector3(1.2f, 1.2f, 1);
    Vector3 originalScale;
    private float animationDuration = 0.1f;


    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(Anim);
        originalScale = transform.localScale; // Store the original scale
    }

    private void OnDestroy()
    {
        btn.onClick.RemoveListener(Anim);
    }

    private void Anim()
    {
        // Scale up animation
        transform.DOScale(upScale, animationDuration)
            .OnComplete(() => // Once the scaling up animation is complete, scale back down
            {
                transform.DOScale(originalScale, animationDuration);
            });
    }
}
