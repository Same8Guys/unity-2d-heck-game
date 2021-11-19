using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour
{

    [SerializeField] float speed = 4.0f;
    [SerializeField] float jumpForce = 7.5f;
    [SerializeField] float rollForce = 6.0f;
    [SerializeField] bool noBlood = false;
    [SerializeField] GameObject slideDust;

    private Animator _animator;
    private Rigidbody2D _body2d;
    private Sensor _groundSensor;
    private Sensor _wallSensorR1;
    private Sensor _wallSensorR2;
    private bool _grounded = false;
    private bool _blocking = false;
    private bool _rolling = false;
    private bool _facingRight = true;
    private int _currentAttack = 0;
    private float _timeSinceAttack = 0.0f;
    private float _delayToIdle = 0.0f;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        _body2d = GetComponent<Rigidbody2D>();
        _groundSensor = transform.Find("GroundSensor").GetComponent<Sensor>();
        _wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor>();
        _wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor>();
    }

    // Update is called once per frame
    void Update()
    {
        // Increase timer that controls attack combo
        _timeSinceAttack += Time.deltaTime;

        //Check if character just landed on the ground
        if (!_grounded && _groundSensor.State())
        {
            _grounded = true;
            _animator.SetBool("Grounded", _grounded);
        }

        //Check if character just started falling
        if (_grounded && !_groundSensor.State())
        {
            _grounded = false;
            _animator.SetBool("Grounded", _grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            transform.rotation.Set(0, 0, 0, transform.rotation.w);
        }
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }


        //Set AirSpeed in animator
        _animator.SetFloat("AirSpeedY", _body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        _animator.SetBool("WallSlide", (_wallSensorR1.State() && _wallSensorR2.State()));

        //Death
        if (Input.GetKeyDown("e") && !_rolling)
        {
            _animator.SetBool("noBlood", noBlood);
            _animator.SetTrigger("Death");
        }

        //Hurt
        else if (Input.GetKeyDown("q") && !_rolling)
        {
            _animator.SetTrigger("Hurt");
        }

        //Attack
        else if (Input.GetMouseButtonDown(0) && _timeSinceAttack > 0.25f && !_rolling)
        {
            _currentAttack++;

            // Loop back to one after third attack
            if (_currentAttack > 3)
            {
                _currentAttack = 1;
            }

            // Reset Attack combo if time since last attack is too large
            if (_timeSinceAttack > 1.0f)
            {
                _currentAttack = 1;
            }

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            _animator.SetTrigger("Attack" + _currentAttack);

            // Reset timer
            _timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !_rolling)
        {
            _blocking = true;
            _animator.SetTrigger("Block");
            _animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
        {
            _blocking = false;
            _animator.SetBool("IdleBlock", false);
        }

        // Roll
        else if (Input.GetKeyDown("left shift") && !_rolling)
        {
            _rolling = true;
            _animator.SetTrigger("Roll");
            _body2d.velocity = new Vector2(inputX * rollForce, _body2d.velocity.y);
        }


        //Jump
        else if (Input.GetKeyDown("space") && _grounded && !_rolling)
        {
            _animator.SetTrigger("Jump");
            _grounded = false;
            _animator.SetBool("Grounded", _grounded);
            _body2d.velocity = new Vector2(_body2d.velocity.x, jumpForce);
            _groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            _delayToIdle = 0.05f;
            _animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            _delayToIdle -= Time.deltaTime;
            if (_delayToIdle < 0)
            {
                _animator.SetInteger("AnimState", 0);
            }
        }
    }

    // Animation Events
    // Called in end of roll animation.
    void AE_ResetRoll()
    {
        _rolling = false;
    }

    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

     
            spawnPosition = _wallSensorR2.transform.position;
        

        if (slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
