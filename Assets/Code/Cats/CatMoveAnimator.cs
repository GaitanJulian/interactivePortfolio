using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CatMoveAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float speedThreshold = 0.02f; // umbral mínimo de movimiento
    [SerializeField] private string isMovingParam = "IsMoving";

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!agent.enabled)
        {
            SetMoving(false);
            return;
        }

        // Consideramos movimiento real: tiene path y cierta velocidad
        bool moving =
            agent.hasPath &&
            !agent.pathPending &&
            agent.remainingDistance > agent.stoppingDistance * 0.5f &&
            agent.velocity.sqrMagnitude > speedThreshold * speedThreshold;

        SetMoving(moving);
    }

    private void SetMoving(bool value)
    {
        if (animator && animator.GetBool(isMovingParam) != value)
        {
            animator.SetBool(isMovingParam, value);
        }
    }
}
