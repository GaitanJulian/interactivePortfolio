using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CatCollisionRagdollActivator : MonoBehaviour
{
    [SerializeField] private CatRagdollController ragdoll;
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private float minSpeedToRagdoll = 1.5f;
    [SerializeField] private float maxSpeedForMaxForce = 5f;
    [SerializeField] private float maxImpulseForce = 4f;

    private int playerLayer;

    void Awake()
    {
        if (!ragdoll) ragdoll = GetComponent<CatRagdollController>();
        playerLayer = LayerMask.NameToLayer(playerLayerName);

        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (ragdoll == null || ragdoll.IsRagdollActive())
            return;

        if (other.gameObject.layer != playerLayer)
            return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null)
        {
            rb = other.GetComponentInParent<Rigidbody>();
        }
        if (rb == null)
            return;

        float speed = rb.linearVelocity.magnitude;
        if (speed < minSpeedToRagdoll)
            return;

        float t = Mathf.InverseLerp(minSpeedToRagdoll, maxSpeedForMaxForce, speed);
        float force = Mathf.Lerp(0.8f * maxImpulseForce, maxImpulseForce, t);

        Vector3 hitPoint = other.ClosestPoint(transform.position);
        Vector3 dir = (transform.position - other.transform.position).normalized;

        ragdoll.HitWithImpulse(hitPoint, dir, force);
    }
}
