using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class CatAI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private CatJumpOffMesh jumper; // opcional, para saltos

    [Header("Roaming")]
    [SerializeField] private float roamRadius = 4f;
    [SerializeField] private float minRoamTime = 5f;
    [SerializeField] private float maxRoamTime = 10f;
    [SerializeField] private float minIdleTime = 8f;
    [SerializeField] private float maxIdleTime = 12f;
    [SerializeField] private float maxRoamStepHeight = 0.3f;

    [Header("Actions")]
    [SerializeField, Range(0f, 1f)] private float actionChancePerCycle = 0.4f;

    private static List<CatActionPoint> allPoints; // compartido por todos
    private CatActionPoint currentPoint;
    private Coroutine loop;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!jumper) jumper = GetComponent<CatJumpOffMesh>();

        if (allPoints == null)
            allPoints = new List<CatActionPoint>(FindObjectsOfType<CatActionPoint>());
    }

    void OnEnable()
    {
        loop = StartCoroutine(BehaviorLoop());
    }

    void OnDisable()
    {
        if (loop != null) StopCoroutine(loop);
        if (currentPoint != null) currentPoint.Release(this);
    }

    private void ForceIdle()
    {
        if (!animator) return;

        // Apaga todo excepto ReturnToIdle
        string[] triggers =
        {
        "Idle",
        "Eat",
        "Drink",
        "Sleep",
        "Groom",
        "JumpUp",
        "JumpDown",
        "ReturnToIdle"
    };

        foreach (var t in triggers)
        {
            if (t != "ReturnToIdle")
                animator.ResetTrigger(t);
        }

        animator.SetTrigger("ReturnToIdle");
    }
    private IEnumerator BehaviorLoop()
    {
        while (true)
        {
            // 1. Decidir si moverse o quedarse quieto
            bool willRoam = Random.value > 0.4f; // 60% chance de caminar
            if (willRoam)
                yield return RoamFor(Random.Range(minRoamTime, maxRoamTime));
            else
                yield return StayIdleFor(Random.Range(minIdleTime, maxIdleTime)); // quieto un rato

            // 2. Si está quieto, chance de Groom espontáneo
            if (!willRoam && Random.value < 0.5f)
                yield return DoSpontaneousAction(CatActionType.Groom);

            // 3. Acciones de puntos
            if (Random.value <= actionChancePerCycle)
            {
                var point = GetAvailableActionPoint();
                if (point != null && point.TryReserve(this))
                {
                    currentPoint = point;
                    yield return GoToPoint(point);

                    CatActionType chosen = point.GetRandomAction();
                    yield return DoPointAction(point, chosen);

                    point.Release(this);
                    currentPoint = null;
                }
            }
        }
    }

    // Caminar libremente dentro de un radio
    private IEnumerator RoamFor(float duration)
    {
        ForceIdle();             
        agent.isStopped = false;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (!agent.pathPending && (agent.remainingDistance <= 0.2f || !agent.hasPath))
            {
                Vector3 randomDir = Random.insideUnitSphere * roamRadius;
                randomDir += transform.position;
                randomDir.y = transform.position.y;

                if (NavMesh.SamplePosition(randomDir, out var hit, roamRadius, NavMesh.AllAreas))
                {
                    // Si el punto está demasiado alto/bajo, lo ignoramos
                    float dy = hit.position.y - transform.position.y;
                    if (Mathf.Abs(dy) <= maxRoamStepHeight)
                    {
                        agent.SetDestination(hit.position);
                    }
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }



    // Acción espontánea (por ejemplo, Groom) en cualquier lugar
    private IEnumerator DoSpontaneousAction(CatActionType action)
    {
        ForceIdle();

        float duration = Random.Range(8f, 16f);

        agent.ResetPath();
        agent.isStopped = true;

        if (animator)
        {
            // Limpiamos cualquier trigger viejo que pudiera molestar
            animator.ResetTrigger("ReturnToIdle");

            switch (action)
            {
                case CatActionType.Groom:
                    ResetAllActionTriggersExcept("Groom");
                    animator.SetTrigger("Groom");
                    break;

                case CatActionType.Sleep:
                    ResetAllActionTriggersExcept("Sleep");
                    animator.SetTrigger("Sleep");
                    break;

                case CatActionType.Idle:
                default:
                    animator.SetTrigger("ReturnToIdle");
                    break;
            }
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;

        if (animator)
            animator.SetTrigger("ReturnToIdle");
    }

    private IEnumerator StayIdleFor(float duration)
    {
        // Nos aseguramos de no tener path activo
        agent.ResetPath();
        agent.isStopped = true;

        // Forzar a Idle si estabas en otra cosa
        if (animator)
        {
            animator.ResetTrigger("Eat");
            animator.ResetTrigger("Drink");
            animator.ResetTrigger("Sleep");
            animator.ResetTrigger("Groom");
            animator.ResetTrigger("JumpUp");
            animator.ResetTrigger("JumpDown");
            animator.SetTrigger("ReturnToIdle");
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
    }

    // Ir hacia un CatActionPoint concreto
    private IEnumerator GoToPoint(CatActionPoint point)
    {
        if (point == null) yield break;

        agent.isStopped = false;
        agent.SetDestination(point.transform.position);

        while (true)
        {
            if (!agent.pathPending && agent.remainingDistance <= 0.15f)
            {
                // Si tienes lógica de salto, no dispares la llegada mientras está a mitad del salto
                if (jumper == null || !jumper.IsMidJump)
                    break;
            }

            yield return null;
        }

        // orientar hacia adelante del punto
        Vector3 dir = point.transform.forward;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }

    // Ejecutar la acción del punto
    private IEnumerator DoPointAction(CatActionPoint point, CatActionType action)
    {
        if (point == null) yield break;

        ForceIdle();   // nos aseguramos de empezar desde Idle

        float dur = point.GetDuration();

        if (animator)
        {
            animator.ResetTrigger("ReturnToIdle");

            switch (action)
            {
                case CatActionType.Sit:
                case CatActionType.Idle:
                    animator.SetTrigger("Idle");
                    break;

                case CatActionType.Eat:
                    ResetAllActionTriggersExcept("Eat");
                    animator.SetTrigger("Eat");
                    break;

                case CatActionType.Drink:
                    ResetAllActionTriggersExcept("Drink");
                    animator.SetTrigger("Drink");
                    break;

                case CatActionType.Scratch:
                    animator.SetTrigger("Groom"); // o Scratch si tienes anim
                    break;

                case CatActionType.Sleep:
                    ResetAllActionTriggersExcept("Sleep");
                    animator.SetTrigger("Sleep");
                    break;

                case CatActionType.Groom:
                    ResetAllActionTriggersExcept("Groom");
                    animator.SetTrigger("Groom");
                    break;
            }
        }

        // Espera la duración de la acción
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // Volvemos a Idle
        if (animator)
            animator.SetTrigger("ReturnToIdle");

        float postRoamDuration;

        if (point.forbidIdleHere)
        {
            // En puntos especiales (rascador, etc.) lo alejamos más
            postRoamDuration = Random.Range(minRoamTime * 0.7f, maxRoamTime * 0.9f);
        }
        else
        {
            // En puntos normales, paseo cortico solo para que no se quede plantado ahí
            postRoamDuration = Random.Range(minRoamTime * 0.3f, minRoamTime * 0.7f);
        }

        yield return RoamFor(postRoamDuration);
    }

    private CatActionPoint GetAvailableActionPoint()
    {
        if (allPoints == null || allPoints.Count == 0) return null;

        // Filtrar solo puntos libres
        var candidates = new List<CatActionPoint>();
        foreach (var p in allPoints)
        {
            if (p != null && p.IsAvailable)
                candidates.Add(p);
        }

        if (candidates.Count == 0) return null;

        // Elegir uno aleatorio
        int idx = Random.Range(0, candidates.Count);
        return candidates[idx];
    }

    private void SetMove(bool moving)
    {
        if (animator) animator.SetBool("IsMoving", moving);
    }

    private void ResetAllActionTriggersExcept(string keep)
    {
        if (!animator) return;

        // Lista de TODOS los triggers que usamos en el controller
        string[] triggers =
        {
            "Idle",
            "Eat",
            "Drink",
            "Sleep",
            "Groom",
            "JumpUp",
            "JumpDown",
            "ReturnToIdle"
        };

        foreach (var t in triggers)
        {
            if (t != keep)
                animator.ResetTrigger(t);
        }
    }

    // ---------------- DEBUG CONTEXT MENUS ----------------

    [ContextMenu("DEBUG/Force Groom Here")]
    private void Debug_ForceGroom()
    {
        if (!animator) return;

        ResetAllActionTriggersExcept("Groom");
        animator.SetTrigger("Groom");
    }

    [ContextMenu("DEBUG/Go To Random NavMesh Point")]
    private void Debug_GoRandom()
    {
        if (NavMesh.SamplePosition(transform.position + Random.insideUnitSphere * roamRadius,
                                   out var hit, roamRadius, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
    }

    [ContextMenu("DEBUG/Go To Nearest ActionPoint")]
    private void Debug_GoNearestPoint()
    {
        float best = float.MaxValue;
        CatActionPoint bestP = null;
        foreach (var p in FindObjectsOfType<CatActionPoint>())
        {
            if (!p || !p.IsAvailable) continue;
            float d = Vector3.SqrMagnitude(p.transform.position - transform.position);
            if (d < best) { best = d; bestP = p; }
        }
        if (bestP)
        {
            agent.isStopped = false;
            agent.SetDestination(bestP.transform.position);
        }
    }
}
