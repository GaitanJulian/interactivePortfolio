using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshLink))]
public class NavMeshLinkSpline : MonoBehaviour
{
    public Spline spline;
    public float traversalDuration = 0.6f;
    public bool visualizeSpline = true;

    NavMeshLink link;

    void Awake()
    {
        link = GetComponent<NavMeshLink>();

        if (!spline)
        {
            var splineObj = new GameObject("Spline");
            splineObj.transform.SetParent(transform);
            spline = splineObj.AddComponent<Spline>();

            spline.start = new GameObject("Start").transform;
            spline.middle = new GameObject("Middle").transform;
            spline.end = new GameObject("End").transform;

            spline.start.SetParent(splineObj.transform);
            spline.middle.SetParent(splineObj.transform);
            spline.end.SetParent(splineObj.transform);

            // valores iniciales básicos, luego tú los mueves en escena
            spline.start.position = link.startPoint;
            spline.end.position = link.endPoint;
            spline.middle.position = (spline.start.position + spline.end.position) * 0.5f + Vector3.up * 0.5f;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!visualizeSpline || spline == null) return;

        Gizmos.color = Color.magenta;
        Vector3 prev = spline.start.position;
        for (int i = 1; i <= 20; i++)
        {
            float t = i / 20f;
            Vector3 next = spline.GetPoint(t);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
#endif

    // forward = true  -> start->middle->end
    // forward = false -> end->middle->start (misma curva invertida)
    public IEnumerator TraverseSpline(NavMeshAgent agent, bool forward)
    {
        if (spline == null || spline.start == null || spline.middle == null || spline.end == null)
            yield break;

        Transform t = agent.transform;

        // Direcciones objetivo según el sentido
        Vector3 start = forward ? spline.start.position : spline.end.position;
        Vector3 end = forward ? spline.end.position : spline.start.position;
        Vector3 lookDir = (end - start).normalized;

        float time = Mathf.Max(traversalDuration, 0.01f);
        float elapsed = 0f;

        while (elapsed < time)
        {
            float k = elapsed / time;
            float u = 1f - k;

            // Bezier cuadrático
            Vector3 p0 = start;
            Vector3 p1 = spline.middle.position;
            Vector3 p2 = end;
            Vector3 pos = u * u * p0 + 2f * u * k * p1 + k * k * p2;

            agent.Warp(pos);

            // Rotar suavemente hacia la dirección del salto
            if (lookDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
                t.rotation = Quaternion.Slerp(t.rotation, targetRot, 0.25f);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Asegura posición final exacta
        agent.Warp(end);
        agent.CompleteOffMeshLink();
    }
}
