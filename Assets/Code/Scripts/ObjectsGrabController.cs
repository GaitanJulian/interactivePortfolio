using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ObjectsGrabController : MonoBehaviour
{
    [SerializeField] private HandCollider leftHand;
    [SerializeField] private HandCollider rightHand;

    [SerializeField] private Transform[] IKTransforms;
    [SerializeField] private Transform[] animatedArms;
    [SerializeField] private ConfigurableJoint[] armsJointsArray;
    private Quaternion[] initialRotations;

    private StarterAssetsInputs _input;

    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();

        initialRotations = new Quaternion[armsJointsArray.Length];

        // Iterate through the jointsArray using an index variable
        for (int i = 0; i < armsJointsArray.Length; i++)
        {
            // Store the initial local rotation of the current joint in initialRotations array
            initialRotations[i] = armsJointsArray[i].transform.localRotation;
        }
    }

    private void Update()
    {
        if (_input.leftGrab)
        {
            FollowLeftHand();
            GrabObj(leftHand);
        }
        else
        {
            AnimatedLeftHand();
            DropObj(leftHand);
        }

        if (_input.rightGrab) 
        {
            FollowRightHand();
            GrabObj(rightHand);
        }
        else
        {
            AnimatedRightHand();
            DropObj(rightHand);
        }

    }

    private void GrabObj(HandCollider hand)
    {
        if (hand.grabbedObj != null && !hand.isGrabbing)
        {
            FixedJoint fj = hand.grabbedObj.AddComponent<FixedJoint>();
            fj.connectedBody = hand.rb;
            fj.breakForce = Mathf.Infinity;
            hand.isGrabbing = true;
        }
    }

    private void DropObj(HandCollider hand)
    {
        if (hand.grabbedObj != null)
        {
            Destroy(hand.grabbedObj.GetComponent<FixedJoint>());
            hand.isGrabbing = false;
        }
            
    }

    private void FollowLeftHand()
    {
        for (int i = 0; i < 4;  i++) 
        {
           ConfigurableJointExtensions.SetTargetRotationLocal(armsJointsArray[i], IKTransforms[i].localRotation, initialRotations[i]);
        }
    }

    private void FollowRightHand()
    {
        for (int i = 4; i < armsJointsArray.Length; i++)
        {
            ConfigurableJointExtensions.SetTargetRotationLocal(armsJointsArray[i], IKTransforms[i].localRotation, initialRotations[i]);
        }
    }

    private void AnimatedLeftHand()
    {
        for (int i = 0; i < 4; i++)
        {
            ConfigurableJointExtensions.SetTargetRotationLocal(armsJointsArray[i], animatedArms[i].localRotation, initialRotations[i]);
        }
    }

    private void AnimatedRightHand() 
    {
        for (int i = 4; i < armsJointsArray.Length; i++)
        {
            ConfigurableJointExtensions.SetTargetRotationLocal(armsJointsArray[i], animatedArms[i].localRotation, initialRotations[i]);
        }
    }
}
