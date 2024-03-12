using UnityEngine.InputSystem;
using UnityEngine;

public class MousePosition3D : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMaskToIgnore;
    [SerializeField] private float defaultDistance = 200f;

    private void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~layerMaskToIgnore))
        {
            transform.position = hit.point;
        }
        else
        {
            // If no hit, project the ray to the default distance
            Vector3 position = ray.origin + ray.direction * defaultDistance;
            transform.position = position;
        }
    }
}