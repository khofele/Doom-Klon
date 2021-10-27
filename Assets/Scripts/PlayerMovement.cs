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

    private Vector3 velocity = Vector3.zero;
    private bool isGrounded = false;
    private bool doubleJump = false;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");

        Vector3 move = transform.right * xAxis + transform.forward * zAxis;

        charController.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded == true)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            doubleJump = true;
            Debug.Log(velocity.y);
        } 
        else 
        if(Input.GetButtonDown("Jump") && doubleJump == true)
        {
            Debug.Log("DJ");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            //velocity.y += jumpHeight;
            Debug.Log(velocity.y);
            doubleJump = false;
        }

        velocity.y += gravity * Time.deltaTime;

        charController.Move(velocity * Time.deltaTime);
    }
}
