using DG.Tweening;
using UnityEngine;

public class FromAToB : MonoBehaviour
{
    public Transform pointB;
    public float moveDuration = 1.0f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Move the object from point A to point B and back in a loop
        MoveObject();
    }

    private void MoveObject()
    {
        // Apply DOTween to move the object from point A to point B and back in a loop with yoyo effect
        transform.DOMove(pointB.position, moveDuration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
