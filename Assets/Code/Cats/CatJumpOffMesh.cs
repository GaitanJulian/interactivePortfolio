using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class CatJumpOffMesh : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Fallback Jump (sin spline)")]
    [SerializeField] private float fallbackJumpDuration = 0.35f;

    [Header("Animator Parameters")]
    [SerializeField] private string jumpUpTrigger = "JumpUp";
    [SerializeField] private string jumpDownTrigger = "JumpDown";

    public bool IsMidJump { get; private set; }

    private NavMeshAgent agent;

    // Para evitar repetir el mismo link y bucles
    private int currentLinkInstanceID = -1;
    private bool isJumping;
    private float lastJumpEndTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();

        // Nosotros controlamos el cruce de OffMeshLinks
        agent.autoTraverseOffMeshLink = false;
    }

    void Update()
    {
        // Si no estamos en un link
        if (!agent.isOnOffMeshLink)
        {
            if (!isJumping)
            {
                currentLinkInstanceID = -1;
            }
            return;
        }

        // Pequeño cooldown por seguridad para que no re-use el link inmediatamente
        if (Time.time - lastJumpEndTime < 0.1f)
            return;

        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Component owner = data.owner as Component;
        int linkId = owner ? owner.GetInstanceID() : 0;

        // Si ya estamos saltando en este mismo link, no hagas nada
        if (isJumping && linkId == currentLinkInstanceID)
            return;

        // Si aún no hemos iniciado el salto en este link, empezamos la rutina
        if (!isJumping && linkId != 0 && linkId != currentLinkInstanceID)
        {
            StartCoroutine(JumpRoutine(data, owner, linkId));
        }
    }

    private IEnumerator JumpRoutine(OffMeshLinkData data, Component owner, int linkId)
    {
        isJumping = true;
        IsMidJump = true;
        currentLinkInstanceID = linkId;

        if (animator)
            animator.SetBool("IsJumping", true);

        Vector3 startPos = data.startPos;
        Vector3 endPos = data.endPos;
        float dy = endPos.y - startPos.y;

        bool goingUp = dy > 0.01f;
        bool goingDown = dy < -0.01f;

        // Elegir animación según la dirección real
        if (animator)
        {
            animator.ResetTrigger(jumpUpTrigger);
            animator.ResetTrigger(jumpDownTrigger);
            animator.ResetTrigger("ReturnToIdle");
            animator.ResetTrigger("Groom");
            animator.ResetTrigger("Sleep");

            if (goingUp && !string.IsNullOrEmpty(jumpUpTrigger))
            {
                animator.SetTrigger(jumpUpTrigger);
            }
            else if (goingDown && !string.IsNullOrEmpty(jumpDownTrigger))
            {
                animator.SetTrigger(jumpDownTrigger);
            }
            else
            {
                // Si la diferencia de altura es casi cero, forzamos JumpUp
                if (!string.IsNullOrEmpty(jumpUpTrigger))
                    animator.SetTrigger(jumpUpTrigger);
            }
        }

        // Buscar el NavMeshLinkSpline asociado a este link (el que tú ya tienes)
        NavMeshLinkSpline splineLink = owner ? owner.GetComponent<NavMeshLinkSpline>() : null;

        if (splineLink != null && splineLink.spline != null)
        {
            // Decidir sentido del spline: desde el punto más cercano al startPos
            float dToStart = (startPos - splineLink.spline.start.position).sqrMagnitude;
            float dToEnd = (startPos - splineLink.spline.end.position).sqrMagnitude;
            bool forward = dToStart <= dToEnd;

            // Usamos TU TraverseSpline que ya funciona
            yield return StartCoroutine(splineLink.TraverseSpline(agent, forward));
        }
        else
        {
            // Fallback lineal si por alguna razón no hay spline
            yield return StartCoroutine(LinearJump(agent, startPos, endPos));
            agent.CompleteOffMeshLink();
        }

        // Esperar a que Unity diga que ya salimos del link
        while (agent.isOnOffMeshLink)
            yield return null;

        IsMidJump = false;
        isJumping = false;
        lastJumpEndTime = Time.time;

        if (animator)
            animator.SetBool("IsJumping", false);
    }

    private IEnumerator LinearJump(NavMeshAgent agent, Vector3 startPos, Vector3 endPos)
    {
        Transform t = agent.transform;

        Vector3 flatDir = endPos - startPos;
        flatDir.y = 0f;
        if (flatDir.sqrMagnitude > 0.0001f)
        {
            t.rotation = Quaternion.LookRotation(flatDir.normalized, Vector3.up);
        }

        float duration = Mathf.Max(fallbackJumpDuration, 0.01f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float k = elapsed / duration;
            Vector3 pos = Vector3.Lerp(startPos, endPos, k);
            agent.Warp(pos);

            elapsed += Time.deltaTime;
            yield return null;
        }

        agent.Warp(endPos);
    }
}
