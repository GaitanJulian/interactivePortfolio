using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuAnimations : MonoBehaviour
{
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform closedTransform;
    [SerializeField] private Transform openTransform;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private float idleDuration = 2f;

    private Coroutine idleCoroutine;
    private Vector3 originalScale;
    private void Start()
    {
        originalScale = pivot.localScale;
        idleCoroutine = StartCoroutine(IdleAnimation());
    }

    [ContextMenu("Open Menu")]
    public void OpenMenu()
    {
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
            transform.localScale = originalScale;
        }
            

        //pivot.DOMove(openTransform.position, animationDuration);

        pivot.DOLocalRotate(new Vector3(0f, 180f, 0f), animationDuration);

    }

    [ContextMenu("Close Menu")]
    public void CloseMenu()
    {
        //pivot.DOMove(closedTransform.position, animationDuration);

        pivot.DOLocalRotate(new Vector3(0f, 0, 0f), animationDuration).OnComplete(() => idleCoroutine = StartCoroutine(IdleAnimation()));
    }

    private IEnumerator IdleAnimation()
    {
        while (true)
        {
            // Grow the menu
            transform.DOScale(originalScale * 1.1f, idleDuration);
            yield return new WaitForSeconds(idleDuration);

            // Shrink the menu back to original size
            transform.DOScale(originalScale, idleDuration);
            yield return new WaitForSeconds(idleDuration);
        }
    }
}
