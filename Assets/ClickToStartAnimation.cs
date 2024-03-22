using UnityEngine;
using DG.Tweening;
using TMPro;

public class ClickToStartAnimation : MonoBehaviour
{
    public float scaleAmount = 1.2f;
    public float duration = 1f;

    private void Start()
    {
        // Start the pulse animation
        PulseAnimation();
    }

    private void PulseAnimation()
    {
        transform.DOScale(scaleAmount, duration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
    }
}
