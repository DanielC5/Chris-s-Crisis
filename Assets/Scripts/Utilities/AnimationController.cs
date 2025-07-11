using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    private bool isTurning;

    private int turnDirection; //-1 left, 0 none, 1 right

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        isTurning = false;
        turnDirection = 0;
    }

    // Update is called once per frame
    void Update()
    {
        bool leftPressed = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

        if (leftPressed && !rightPressed)
        {
            Turn(-1);
        }
        else if (rightPressed && !leftPressed)
        {
            Turn(1);
        }
        else if (isTurning)
        {
            // key released, turn back
            isTurning = false;
            turnDirection = 0;
            animator.SetBool("isTurning", false);
            animator.SetInteger("turnDirection", 0);
        }

    }

    private void Turn(int direction) //-1 left, 1 right
    {
        if (!isTurning || turnDirection != direction)
        {
            //start the turning
            isTurning = true;
            turnDirection = direction;
            animator.SetBool("isTurning", true);
            animator.SetInteger("turnDirection", turnDirection);
        }

    }

    public void TriggerDiedAnimation()
    {
        animator.SetTrigger("died");
    }

    public void TriggerRespawnAnimation()
    {
        animator.SetTrigger("respawn");
    }
}
