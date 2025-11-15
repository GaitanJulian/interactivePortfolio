using UnityEngine;

[ExecuteInEditMode]
public class Spline : MonoBehaviour
{
    public Transform start;
    public Transform middle;
    public Transform end;
    public bool showGizmos = true;

    // Devuelve un punto en la curva de Bezier cuadrática
    public Vector3 GetPoint(float t)
    {
        Vector3 p0 = start.position;
        Vector3 p1 = middle.position;
        Vector3 p2 = end.position;

        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    // Dibuja el spline en el editor
    private void OnDrawGizmos()
    {
        if (!showGizmos || start == null || middle == null || end == null) return;

        Gizmos.color = Color.cyan;
        Vector3 prev = start.position;
        for (int i = 1; i <= 20; i++)
        {
            float t = i / 20f;
            Vector3 next = GetPoint(t);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(start.position, 0.05f);
        Gizmos.DrawSphere(middle.position, 0.05f);
        Gizmos.DrawSphere(end.position, 0.05f);
    }
}