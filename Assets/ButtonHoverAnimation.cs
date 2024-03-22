using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScaleFactor = 1.1f; // Factor by which the button will scale on hover
    public float animationDuration = 0.2f; // Duration of the scale animation

    private Vector3 originalScale; // Original scale of the button

    private void Start()
    {
        // Record the original scale of the button
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Scale up the button on hover using DOTween
        transform.DOScale(originalScale * hoverScaleFactor, animationDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Scale down the button when the mouse exits using DOTween
        transform.DOScale(originalScale, animationDuration);
    }
}
