using System.Collections;
using UnityEngine;

public class ShootingFXController : MonoBehaviour
{

    public Sprite[] straightShoot;
    public Sprite[] rightShoot;
    public Sprite[] contRightShoot;
    public Sprite[] leftShoot;
    public Sprite[] contLeftShoot;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GameObject.Find("CarAnimationSprite").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("CarIdle"))
        {
            GetComponent<SpriteRenderer>().sprite = straightShoot[Random.Range(0, straightShoot.Length)];
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("CarTurnRight") || animator.GetCurrentAnimatorStateInfo(0).IsName("CarUnturnRight"))
        {
            GetComponent<SpriteRenderer>().sprite = rightShoot[Random.Range(0, rightShoot.Length)];
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("CarTurnIdleRight"))
        {
            GetComponent<SpriteRenderer>().sprite = contRightShoot[Random.Range(0, contRightShoot.Length)];
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("CarTurnLeft") || animator.GetCurrentAnimatorStateInfo(0).IsName("CarUnturnLeft"))
        {
            GetComponent<SpriteRenderer>().sprite = leftShoot[Random.Range(0, leftShoot.Length)];
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("CarTurnIdleLeft"))
        {
            GetComponent<SpriteRenderer>().sprite = contLeftShoot[Random.Range(0, contLeftShoot.Length)];
        }
        StartCoroutine("Unshoot");
    }

    IEnumerator Unshoot()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().sprite = null;
    }
}
