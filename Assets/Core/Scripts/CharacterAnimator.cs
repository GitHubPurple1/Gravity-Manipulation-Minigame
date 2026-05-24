using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerMovement locomotion;

    private static readonly int WalkHash = Animator.StringToHash("isWalk");
    private static readonly int JumpHash = Animator.StringToHash("isJumping");

    private void Update()
    {
        if (locomotion == null || anim == null) return;

        anim.SetBool(WalkHash, locomotion.isMoving);
        anim.SetBool(JumpHash, !locomotion.isGrounded);
    }
}
