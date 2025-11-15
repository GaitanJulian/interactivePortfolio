using DG.Tweening;
using UnityEngine;

public class CloudsAnimation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private bool randomStartOffset = true;

    private Tween rotationTween;

    void Start()
    {
        // Pequeña variación inicial para que no roten sincronizadas
        if (randomStartOffset)
        {
            float offset = Random.Range(0f, 360f);
            transform.Rotate(0f, offset, 0f);
        }

        // Creamos la rotación infinita
        rotationTween = transform
            .DORotate(new Vector3(0, 360, 0), rotationSpeed, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    void OnDestroy()
    {
        rotationTween?.Kill();
    }
}
