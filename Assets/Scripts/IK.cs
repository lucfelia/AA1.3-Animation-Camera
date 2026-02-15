using UnityEngine;

public class IK : MonoBehaviour
{
    Animator anim;
    public float lookWeight = 1.0f;
    public Transform look;
    public float rightHandWeight = 1.0f;
    public Transform rightHand;
    public float leftHandWeight = 1.0f;
    public Transform leftHand;

    private void Start()
    {
        anim = GetComponent<Animator>();
        Debug.Log(anim);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        //Head Look
        anim.SetLookAtWeight(lookWeight, lookWeight);
        anim.SetLookAtPosition(look.position);

        //Right Hand
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
        anim.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
        anim.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);

        //Left Hand
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
    }

}
