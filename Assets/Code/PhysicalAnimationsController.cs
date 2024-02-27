using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PhysicalAnimationsController : MonoBehaviour
{
    [Header("Target Rotation animation")]
    [SerializeField] private Transform[] animatedTransformms;
    [SerializeField] private ConfigurableJoint[] jointsArray;
    [SerializeField] public Animator animator;
    private Quaternion[] initialRotations;

    private void Start()
    {
        initialRotations = new Quaternion[jointsArray.Length];

        // Iterate through the jointsArray using an index variable
        for (int i = 0; i < jointsArray.Length; i++)
        {
            // Store the initial local rotation of the current joint in initialRotations array
            initialRotations[i] = jointsArray[i].transform.localRotation;
        }
    }

    private void Update()
    {
        for (int i = 0; i < jointsArray.Length; i++)
        {
            ConfigurableJointExtensions.SetTargetRotationLocal(jointsArray[i], animatedTransformms[i].localRotation, initialRotations[i]);
        }
    }
}
