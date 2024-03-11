using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootCollision : MonoBehaviour
{
    private PhysicalAnimationsController controller;

    private void Start()
    {
        controller = FindObjectOfType<PhysicalAnimationsController>();
    }

}
