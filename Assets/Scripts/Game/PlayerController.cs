using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PolarTransform))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float rotateSpeed = 90f;

    [Header("Attack")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float bulletLife = 5f;

    public bool spedUp;

    private float shootBuffer = 0.8f;
    private float lastShootTime = 0f;
    private bool canAttack = false;

    private PolarTransform polar;
    private Vector2 moveInput;
    private bool isAttacking;
    


    // SOUND
    private AudioSource src;
    public AudioClip turnSFX;
    public AudioClip bulletSFX;

    void Awake()
    {
        polar = GetComponent<PolarTransform>();
        GameManager.Instance.SetPlayer(this.gameObject);
    }

    void Start()
    {
        src = GetComponent<AudioSource>();
    }

    void Update()
    {
        HandleMovement();
        HandleAttack();
    }

    void HandleMovement()
    {
        float horizontal = moveInput.x;

        // A (left): clockwise ➜ angle--
        // D (right): counter-clockwise ➜ angle++

        

        if (horizontal < 0)
        {
            polar.angleDeg -= rotateSpeed * Time.deltaTime;
        }
        else if (horizontal > 0)
        {
            polar.angleDeg += rotateSpeed * Time.deltaTime;
        }

        // W/S do nothing
    }

    void HandleAttack()
    {
        // check buffer
        if (lastShootTime >= shootBuffer)
        {
            canAttack = true;
            lastShootTime = 0;
        }
        else
        {
            canAttack = false;
        }
        lastShootTime += Time.deltaTime;

        // check to attack
        if (isAttacking && canAttack)
        {
            FireBullet();
            isAttacking = false;
        }
    }

    void FireBullet()
    {
        if (bulletPrefab == null) return;
        GameObject.Find("Shooting Effect").GetComponent<ShootingFXController>().Shoot();
        // bullet sfx
        src.PlayOneShot(bulletSFX);

        GameObject bullet = Instantiate(bulletPrefab, Vector3.Normalize(transform.position) * 10f, Quaternion.identity);
        PolarTransform playerPol = GetComponent<PolarTransform>();
        PolarTransform bulPol = bullet.GetComponent<PolarTransform>();
        bulPol.center = playerPol.center;
        bulPol.FromWorldPosition(transform.position);
        bulPol.radius = 4f;
        
        BulletBehaviour bb = bullet.GetComponent<BulletBehaviour>();
        // bb.source = "player";
        bb.Init(BulletBehaviour.Motion.RadialIn, bulletSpeed, bulletLife);
    }

    #region Input System

    public void OnMove(InputValue value)
    {
        Vector2 moveInput = value.Get<Vector2>();
        this.moveInput = moveInput;

        // plays turnSFX when moving
        src.clip = turnSFX;
        src.Play();

        bool leftPressed = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

        if (!leftPressed && !rightPressed)
        {
            // stops turnSFX if arrow keys not pressed, not if the car is actually still - might need to change
            src.Stop();
        }
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
            isAttacking = true;
    }

    #endregion
}