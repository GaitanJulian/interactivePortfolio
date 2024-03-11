using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ObjectsGrabController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private HandCollider leftHand;
    [SerializeField] private HandCollider rightHand;

    private StarterAssetsInputs _input;

    private int _animIDLeftGrab;
    private int _animIDRightGrab;

    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        AssignAnimationIDs();
    }

    private void Update()
    {
        if (_input.leftGrab)
        {
            animator.SetBool(_animIDLeftGrab, true);
            GrabObj(leftHand);
        }
        else
        {
            DropObj(leftHand);
            animator.SetBool(_animIDLeftGrab, false);
        }

        if (_input.rightGrab) 
        {
            animator.SetBool(_animIDRightGrab, true);
            GrabObj(rightHand);
        }
        else
        {
            DropObj(rightHand);
            animator.SetBool (_animIDRightGrab, false);
        }

    }

    private void AssignAnimationIDs()
    {
        _animIDLeftGrab = Animator.StringToHash("LeftHand");
        _animIDRightGrab = Animator.StringToHash("RightHand");
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
}
