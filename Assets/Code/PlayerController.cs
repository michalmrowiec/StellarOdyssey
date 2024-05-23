using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public Camera sceneCamera;
    public float moveSpeed;
    public Rigidbody2D rb;
    public WeaponOwner weaponOwner;
    public int healthPoints = 1;

    private Vector2 _moveDirection;
    private Vector2 _mousePosition;
    public Vector2 weaponOffset;
    public bool drawWeaponOffsetGizmo = false;
    public Vector2 playerVelocity;

    private void Start()
    {
        weaponOwner = GetComponentInChildren<WeaponOwner>();
    }

    void Update()
    {
        if(!sceneCamera.GetComponent<Menu>().pauseMenu.activeInHierarchy)
        {
            PrecessInputs();

            animator.SetFloat("Walk", Mathf.Abs(_moveDirection.x + _moveDirection.y));
            animator.SetFloat("Speed", _moveDirection.sqrMagnitude);
        }
    }

    private void FixedUpdate()
    {
        playerVelocity = rb.velocity;
        Move();
    }

    public void TakeDamage(int damage = 1)
    {
        healthPoints -= damage;

        if (healthPoints <= 0)
        {
            //gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }

    void PrecessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButtonDown(0) && weaponOwner.weapon != null)
        {
            weaponOwner.weapon.Fire(Color.blue);
        }

        _moveDirection = new Vector2(moveX, moveY);
        _mousePosition = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    void Move()
    {
        rb.velocity = new Vector2(_moveDirection.x * moveSpeed, _moveDirection.y * moveSpeed);

        Vector2 playerPositionWithOffset = transform.TransformPoint(weaponOffset); // Przekształcamy offset do globalnego układu współrzędnych
        Vector2 aimDirection = _mousePosition - playerPositionWithOffset; // Używamy nowej pozycji z offsetem
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = aimAngle;
    }

    void OnDrawGizmos()
    {
        if (drawWeaponOffsetGizmo)
        {
            Gizmos.color = Color.red;
            Vector3 vector3 = transform.TransformPoint(weaponOffset); // Przekształcamy offset do globalnego układu współrzędnych
            Gizmos.DrawSphere(vector3, 0.1f);
        }
    }

}
