using UnityEngine;
using System.Collections.Generic;

public enum CatActionType
{
    Idle,
    Sit,
    Eat,
    Drink,
    Scratch,
    Sleep,
    Groom,
}

public class CatActionPoint : MonoBehaviour
{
    [Header("Behavior")]
    public bool forbidIdleHere = false; // marca true en los puntos especiales

    public List<CatActionType> actionOptions = new List<CatActionType> { CatActionType.Sleep };
    [Min(1f)] public float minDuration = 20f;
    [Min(1f)] public float maxDuration = 40f;
    [Min(1)] public int maxOccupants = 1;

    private readonly HashSet<object> occupants = new HashSet<object>();

    public float GetDuration()
    {
        return Random.Range(minDuration, maxDuration);
    }

    public bool IsAvailable => occupants.Count < maxOccupants;

    public bool TryReserve(object cat)
    {
        if (occupants.Count >= maxOccupants) return false;
        return occupants.Add(cat);
    }

    public void Release(object cat)
    {
        occupants.Remove(cat);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsAvailable ? Color.cyan : Color.red;
        Gizmos.DrawSphere(transform.position, 0.08f);
    }

    public CatActionType GetRandomAction()
    {
        if (actionOptions == null || actionOptions.Count == 0)
            return CatActionType.Idle;
        return actionOptions[Random.Range(0, actionOptions.Count)];
    }
}
