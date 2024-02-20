using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform[] animatedTransfomrsArrays; // This array will reference the transform of the animated body of the player
    [SerializeField] private ConfigurableJoint[] jointsArray; // This array will contain all the joints of the player

    private Quaternion[] initialRotations;

    private void Start()
    {
        initialRotations = new Quaternion[jointsArray.Length];

        for (int i = 0; i < jointsArray.Length; i++)
        {
            initialRotations[i] = jointsArray[i].transform.localRotation; // Store the initial rotations of each joint
        }
    }

    private void Update()
    {
        for (int i = 0; i < jointsArray.Length; i++)
        {
            ConfigurableJointExtensions.SetTargetRotationLocal(jointsArray[i], animatedTransfomrsArrays[i].localRotation, initialRotations[i]);
        }
    }
}
