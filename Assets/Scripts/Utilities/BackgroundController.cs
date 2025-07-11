using UnityEngine;

public class BackgroundController : MonoBehaviour
{

    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("randomDecision", Random.value);
    }

    public void gameEnd()
    {
        animator.SetBool("gameEnd", true);

    }
}
