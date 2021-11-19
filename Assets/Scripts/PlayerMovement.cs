using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] float speed = 500.0f;
    [SerializeField] float acceleration = 500.0f;

    [SerializeField] float jumpForce = 250.0f;
    [SerializeField] float dashForce = 15000.0f;
    [SerializeField] float dashCooldown = 1.0f;
    [SerializeField] bool faceRight = true;

    [SerializeField] GameObject slideDust;

    private Animator animator;
    private Rigidbody2D body2d;
    private Sensor groundSensor;
    private Sensor wallSensorR1;
    private Sensor wallSensorR2;
    private bool grounded = false;
    private bool blocking = false;
    private bool dashing = false;
    private float horizontalValue = 0;
    private int currentAttack = 0;
    private float timeSinceAttack = 0.0f;
    private float timeSinceDash = 0.0f;
    private float delayToIdle = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor>();
        wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor>();
        wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeSinceAttack += Time.deltaTime;
        timeSinceDash += Time.deltaTime;

        if (timeSinceDash > dashCooldown)
        {
            dashing = false;
        }

        horizontalValue = Input.GetAxis("Horizontal");
        flip();
        move();
        evaluateGrounded();
        jump();
        dash();
        animator.SetFloat("AirSpeedY", body2d.velocity.y);
        animator.SetBool("WallSlide", (wallSensorR1.State() && wallSensorR2.State()));
    }

    void move()
    {
        float forceX = speed * horizontalValue;
        if (forceX > 0 || forceX < 0)
        {
            if (body2d.velocity.x < 1)
            {
                forceX *= 2;
            }
            body2d.AddForce(new Vector2(forceX, 0));
            animator.SetInteger("AnimState", 1);
        }
        if (horizontalValue == 0)
        {
            animator.SetInteger("AnimState", 0);
        }

    }

    void flip()
    {
        if ((horizontalValue < 0 && faceRight) || (horizontalValue > 0 && !faceRight))
        {
            faceRight = !faceRight;
            transform.Rotate(new Vector3(0, 180, 0));
        }
    }

    void jump()
    {
        if (grounded && Input.GetAxis("Vertical") > 0)
        {
            animator.SetTrigger("Jump");
            body2d.AddForce(new Vector2(0, jumpForce));
        }
    }

    void dash()
    {
        if (!dashing && Input.GetButton("Dash"))
        {
            int direction = faceRight ? 1 : -1;
            float forceX = direction * dashForce;
            _animator.SetTrigger("Roll");
            body2d.AddForce(new Vector2(forceX, 0));
            dashing = true;
            timeSinceDash = 0.0f;
        }
    }

    void evaluateGrounded()
    {
        grounded = groundSensor.State();
        animator.SetBool("Grounded", grounded);
    }
}
