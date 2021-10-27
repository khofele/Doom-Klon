using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController charController = null;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform groundCheck = null;
    [SerializeField] private float groundDistance = 0.4f; 
    [SerializeField] private LayerMask groundMask = default;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float dashSpeed = 14;
    [SerializeField] private float dashTime = 0.1f;

    private Vector3 velocity = Vector3.zero;
    private bool isGrounded = false;
    private bool doubleJump = false;
    private Vector3 moveDir = Vector3.zero;


    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");

        moveDir = transform.right * xAxis + transform.forward * zAxis;

        charController.Move(moveDir * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded == true)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            doubleJump = true;
        } 
        else 
        if(Input.GetButtonDown("Jump") && doubleJump == true)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            doubleJump = false;
        }

        if(Input.GetKey(KeyCode.V))
        {
            StartCoroutine(Dash());
        }

        velocity.y += gravity * Time.deltaTime;

        charController.Move(velocity * Time.deltaTime);
    }

    private IEnumerator Dash()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            charController.Move(moveDir * dashSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
