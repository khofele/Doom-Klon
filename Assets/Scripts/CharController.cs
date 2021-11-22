using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    [SerializeField] private CharacterController charController = null;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.81f * 2;
    [SerializeField] private Transform groundCheck = null;
    [SerializeField] private float groundDistance = 0.4f; 
    [SerializeField] private LayerMask groundMask = default;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float trampolineHeight = 8f;
    [SerializeField] private float dashSpeed = 10000f;
    [SerializeField] private float dashTime = 150f;
    [SerializeField] private float maxDistance = 0;
    [SerializeField] private LayerMask environment = default;
    [SerializeField] private static float health = 100;
    [SerializeField] private UI uiManager = null;

    private Vector3 velocity;
    private bool isGrounded = false;
    private bool doubleJump = false;
    private Vector3 moveDir;
    private Camera cam = null;
    private LineRenderer lineRenderer = null;
    private Vector3 hookPos = Vector3.zero;
    private bool hook = false;
    private bool dash = false;
    private bool climbing = false;
    private float startTime;

    public static float Health
    {
        get => health;
        set
        {
            health = value;
        }
    }

    private void Start()
    {
        cam = gameObject.GetComponentInChildren<Camera>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");

        moveDir = transform.right * xAxis + transform.forward * zAxis;

        // Jump + Doublejump
        JumpCheck();

        // Dash
        Dash();

        // Grappling Hook
        GrapplingHook();

        // Climbing
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1f))
            {
                if (hit.collider.GetComponent<Climbable>() != null)
                {
                    StartCoroutine(Climb(hit.collider));
                }
            }
        }


        // Shoot

        if (hook == false && climbing == false)
        {
            charController.Move(moveDir * speed * Time.deltaTime);
            velocity.y += gravity * Time.deltaTime;
            charController.Move(velocity * Time.deltaTime);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.transform.tag == "Trampoline")
        {
            velocity.y = Mathf.Sqrt(trampolineHeight * -2f * gravity);
            doubleJump = false;
        }
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        doubleJump = true;
    }

    private void DoubleJump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        doubleJump = false;
    }

    private void JumpCheck()
    {
        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            Jump();
        }
        // Double Jump
        else if (Input.GetButtonDown("Jump") && doubleJump == true)
        {
            DoubleJump();
        }
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            startTime = Time.time;
            dash = true;
        }

        if (dash == true && Time.time < startTime + dashTime)
        {
            charController.Move(moveDir.normalized * dashSpeed * Time.deltaTime);
            Debug.Log(moveDir.normalized);
        }
        else
        {
            dash = false;
        }
    }

    private void GrapplingHook()
    {
        if (Input.GetKeyDown(KeyCode.E) && hook == false)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, maxDistance, environment))
            {
                hookPos = hit.point;
                lineRenderer.enabled = true;
                startTime = Time.time;
                hook = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E) && hook == true)
        {
            hook = false;
        }

        if (hook == true && Time.time < startTime + 0.5f)
        {
            Vector3 hookDir = (hookPos - transform.position).normalized;
            charController.Move(hookDir * 20 * Time.deltaTime);
            lineRenderer.SetPositions(new Vector3[] { transform.position, hookPos });
        }
        else
        {
            hook = false;
            lineRenderer.enabled = false;
        }
    }

    private IEnumerator Climb(Collider climbCollider)
    {
        climbing = true;
        doubleJump = true;
        while (Input.GetKey(KeyCode.Space))
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1f))
            {
                if(hit.collider == climbCollider)
                {
                    int xDir;
                    if(cam.transform.localRotation.x <= 0)
                    {
                        xDir = 1;
                    }
                    else
                    {
                        xDir = -1;
                    }

                    int zDir;
                    if (cam.transform.localRotation.z <= 0)
                    {
                        zDir = 1;
                    }
                    else
                    {
                        zDir = -1;
                    }

                    if (climbCollider.gameObject.CompareTag("WallTop"))
                    {
                        charController.Move(new Vector3(Input.GetAxis("Horizontal") * zDir * Time.deltaTime, Input.GetAxis("Vertical") * xDir * Time.deltaTime, 0));
                    }
                    else if (climbCollider.gameObject.CompareTag("WallLeft"))
                    {
                        charController.Move(new Vector3(0, Input.GetAxis("Vertical") * xDir * Time.deltaTime, Input.GetAxis("Horizontal") * zDir * Time.deltaTime));
                    }
                    else if (climbCollider.gameObject.CompareTag("WallRight"))
                    {
                        charController.Move(new Vector3(0, Input.GetAxis("Vertical") * xDir * Time.deltaTime, Input.GetAxis("Horizontal") * -zDir * Time.deltaTime));
                    }
                    else if (climbCollider.gameObject.CompareTag("WallDown"))
                    {
                        charController.Move(new Vector3(Input.GetAxis("Horizontal") * -zDir * Time.deltaTime, Input.GetAxis("Vertical") * xDir * Time.deltaTime, 0));
                    }

                    yield return null;
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }

        climbing = false;
    }

    public void TakeDamage(float damage)
    {
        if(health > 0)
        {
            health -= damage;
            uiManager.SetColor(health * 0.01f);
        }
        else
        {
            Debug.Log("Player died!");
            uiManager.DisplayDeathMessage();
        }
    }
}
