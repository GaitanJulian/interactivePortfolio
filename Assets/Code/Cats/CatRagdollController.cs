using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class CatRagdollController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform hips;   // hueso central
    [SerializeField] private Transform root;   // raíz del gato (este objeto)

    [Header("Timings")]
    [SerializeField] private float ragdollTime = 1.5f;
    [SerializeField] private float standUpBlendTime = 0.4f;

    private readonly List<Rigidbody> ragdollBodies = new List<Rigidbody>();
    private readonly List<Collider> ragdollColliders = new List<Collider>();
    private bool isRagdoll;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!root) root = transform;

        GetComponentsInChildren(true, ragdollBodies);
        GetComponentsInChildren(true, ragdollColliders);

        var rootRb = GetComponent<Rigidbody>();
        var rootCol = GetComponent<Collider>();
        if (rootRb) ragdollBodies.Remove(rootRb);
        if (rootCol) ragdollColliders.Remove(rootCol);

        SetRagdoll(false, true);
    }

    public bool IsRagdollActive()
    {
        return isRagdoll;
    }

    // Llamado sin impulso específico (por ejemplo, clic)
    public void HitAndRagdoll()
    {
        if (isRagdoll) return;
        StartCoroutine(RagdollRoutine(Vector3.zero, Vector3.zero, 0f, false));
    }

    // Llamado con impulso (por colisión con el jugador)
    public void HitWithImpulse(Vector3 hitPoint, Vector3 direction, float force)
    {
        if (isRagdoll) return;
        direction.y = Mathf.Max(direction.y, 0.1f); // un poco hacia arriba para que sea más divertido
        StartCoroutine(RagdollRoutine(hitPoint, direction.normalized, force, true));
    }

    private IEnumerator RagdollRoutine(Vector3 hitPoint, Vector3 dir, float force, bool useImpulse)
    {
        SetRagdoll(true, false);

        if (useImpulse && hips != null && force > 0.01f)
        {
            var hipsRb = hips.GetComponent<Rigidbody>();
            if (hipsRb != null)
            {
                hipsRb.AddForceAtPosition(dir * force, hitPoint, ForceMode.Impulse);
            }
        }

        yield return new WaitForSeconds(ragdollTime);
        yield return StandUpFromRagdoll();
    }

    private void SetRagdoll(bool active, bool instantAlign)
    {
        isRagdoll = active;

        if (agent) agent.enabled = !active;
        if (animator) animator.enabled = !active;

        foreach (var rb in ragdollBodies)
        {
            if (!rb) continue;
            rb.isKinematic = !active;
        }

        foreach (var col in ragdollColliders)
        {
            if (!col) continue;
            col.enabled = active;
        }

        if (!active && instantAlign && hips && root)
        {
            root.position = hips.position;
        }
    }

    private IEnumerator StandUpFromRagdoll()
    {
        if (!hips || !root)
        {
            SetRagdoll(false, true);
            yield break;
        }

        // Reposicionar raíz en función del cuerpo
        Vector3 targetPos = hips.position;
        targetPos.y = root.position.y;
        root.position = targetPos;

        Quaternion startRot = root.rotation;
        Quaternion endRot = Quaternion.LookRotation(root.forward, Vector3.up);

        float t = 0f;
        while (t < standUpBlendTime)
        {
            t += Time.deltaTime;
            float k = t / Mathf.Max(standUpBlendTime, 0.01f);
            root.rotation = Quaternion.Slerp(startRot, endRot, k);
            yield return null;
        }

        SetRagdoll(false, false);

        if (animator)
        {
            // Si no tienes anim "StandUp", reemplaza por Idle base
            animator.Play("Idle", 0, 0f);
        }

        if (agent)
        {
            yield return null;
            agent.enabled = true;
        }
    }
}
