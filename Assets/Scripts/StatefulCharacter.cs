using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatefulCharacter : MonoBehaviour
{

    [SerializeField] float speed = 500.0f;
    // Speed is multiplied by this number if player is stationary
    [SerializeField] float stationaryBurst = 3.0f;

    [SerializeField] float jumpForce = 250.0f;
    [SerializeField] float dashForce = 15000.0f;
    [SerializeField] float dashCooldown = 1.0f;
    [SerializeField] bool faceRight = true;

    [SerializeField] GameObject slideDust;

    private Animator animator;
    private Rigidbody2D body2d;
    private Sensor groundSensor;
    private Sensor wallSensorRB;
    private Sensor wallSensorRT;

    private bool grounded = false;
    private bool wallgrabbing = false;
    private bool blocking = false;
    private bool rolling = false;
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
        wallSensorRB = transform.Find("WallSensor_RB").GetComponent<Sensor>();
        wallSensorRT = transform.Find("WallSensor_RT").GetComponent<Sensor>();

    }

    void FixedUpdate()
    {
        timeSinceAttack += Time.deltaTime;
        timeSinceDash += Time.deltaTime;

        if (timeSinceDash > dashCooldown)
        {
            rolling = false;
        }

        horizontalValue = Input.GetAxis("Horizontal");
        evaluateGrounded();
        evaluateWallgrabbing();
        flip();
        move();
        jump();
        dash();
    }

    void move()
    {
        float forceX = speed * horizontalValue;
        if (Mathf.Abs(forceX) > Mathf.Epsilon)
        {
            // Multiply speed if they're not moving, so they don't spend 100 years gaining speed
            if ((-1 <= body2d.velocity.x && body2d.velocity.x <= 1))
            {
                forceX *= stationaryBurst;
            }
            body2d.AddForce(new Vector2(forceX, 0));
            delayToIdle = 0.05f;
            animator.SetInteger("AnimState", 1);
        }
        delayToIdle -= Time.deltaTime;
        if (delayToIdle < 0)
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
        if (Input.GetAxis("Vertical") > 0)
        {
            if (grounded)
            {
                animator.SetTrigger("Jump");
                body2d.AddForce(new Vector2(0, jumpForce));
            }
            else if (wallgrabbing)
            {
                float horizontal = (horizontalValue * -1) * (jumpForce / 2);
                body2d.AddForce(new Vector2(horizontal, jumpForce / 2));
            }
            groundSensor.Disable(0.2f);
        }
        animator.SetFloat("AirSpeedY", body2d.velocity.y);
    }

    void dash()
    {
        if (!rolling && Input.GetButton("Dash"))
        {
            int direction = faceRight ? 1 : -1;
            float forceX = direction * dashForce;
            animator.SetTrigger("Roll");
            body2d.AddForce(new Vector2(forceX, 0));
            rolling = true;
            timeSinceDash = 0.0f;
        }
    }

    void evaluateGrounded()
    {
        grounded = groundSensor.State();
        animator.SetBool("Grounded", grounded);
    }

    void evaluateWallgrabbing()
    {
        wallgrabbing = !grounded && wallSensorRB.State() && wallSensorRT.State();
        animator.SetBool("WallSlide", wallgrabbing);
    }

    void AE_ResetRoll()
    {
        rolling = false;
    }

    void AE_SlideDust()
    {
        if (slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate<GameObject>(
                slideDust,
                wallSensorRT.transform.position,
                gameObject.transform.localRotation);
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
