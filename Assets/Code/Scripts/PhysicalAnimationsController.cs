using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PhysicalAnimationsController : MonoBehaviour
{
    [Header("Target Rotation animation")]
    [SerializeField] private Transform[] animatedTransformms;
    [SerializeField] private ConfigurableJoint[] jointsArray;
    private Quaternion[] initialRotations;

    /*
    [SerializeField] private Transform lookAt;
    [SerializeField] private ConfigurableJoint leftShoulder;
    [SerializeField] private Quaternion initialShoulderRotation;
    [SerializeField] private GameObject representation;
    */
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

        // leftShoulderLook();
    }

    /*
    private void leftShoulderLook()
    {
        // Calculate the direction vector from the shoulder to the target point in world space
        Vector3 direction = lookAt.position - leftShoulder.transform.position;

        // Project the direction vector onto the plane defined by the shoulder's local Y-axis
        Vector3 projectedDirection = Vector3.ProjectOnPlane(direction, leftShoulder.transform.up);

        // Calculate the rotation needed to align the projected direction with the local Y-axis
        // Quaternion targetRotation = Quaternion.LookRotation(projectedDirection, leftShoulder.transform.up);
         
        // Quaternion targetRotation = Quaternion.LookRotation(direction, leftShoulder.transform.right);

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Set the rotation of the representation (optional, for visualization)
        representation.transform.rotation = targetRotation;

        // Set the target rotation of the shoulder joint
        ConfigurableJointExtensions.SetTargetRotationLocal(leftShoulder, targetRotation, initialRotations[2]);
    }
    */
}
