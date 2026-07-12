using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    public float m_walkSpeed = 4.0f;
    public float m_turnSpeed = 360.0f;  // degrees per second
    public float m_jumpSpeed = 10.0f;
    public float m_gravity = 20.0f;

    public class CharInput
    {
        public Vector3 m_direction;
        public float m_facingAngle;
        public bool m_attack;
        public bool m_jump;
    }

    Animator m_anim;
    CharacterController m_char;
    CharInput m_input;
    PlayerInput m_playerInput;
    Vector3 m_velocity;

    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_char = GetComponent<CharacterController>();
        m_playerInput = GetComponent<PlayerInput>();
        m_input = new CharInput();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = m_playerInput.actions["Move"].ReadValue<Vector2>();
        m_input.m_jump = false;
        if (m_char.isGrounded)
        {
            m_input.m_jump = m_playerInput.actions["Jump"].triggered;
            if (m_input.m_jump)
            {
                m_velocity.y = m_jumpSpeed;
            }
            else
            {
                m_velocity.y = -0.5f;
            }
        }

        // convert from camera-space to world-space movement
        Vector3 fwd = Camera.main.transform.forward;
        fwd.y = 0.0f;
        fwd.Normalize();
        Vector3 rt = Camera.main.transform.right;
        rt.y = 0.0f;
        rt.Normalize();
        Vector3 moveWorld = move.y * fwd + move.x * rt;
        m_input.m_direction = moveWorld;

        // face the way we are moving
        float throttle = m_input.m_direction.magnitude;
        if (moveWorld.magnitude > 0.01f)
            m_input.m_facingAngle = Mathf.Atan2(m_input.m_direction.x, m_input.m_direction.z) * Mathf.Rad2Deg;
        else
            m_input.m_facingAngle = transform.localEulerAngles.y;

        // gravity
        m_velocity.y -= m_gravity * Time.deltaTime;
        // move in the direction of the input
        float yVel = m_velocity.y;
        m_velocity = m_input.m_direction * m_walkSpeed;
        m_velocity.y = yVel;
        m_char.Move(m_velocity * Time.deltaTime);

        // turn to face the angle in the input
        Vector3 ang = transform.localEulerAngles;
        float diff = m_input.m_facingAngle - ang.y;
        if (diff > 180.0f)
            diff -= 360.0f;
        if (diff < -180.0f)
            diff += 360.0f;
        float maxRate = throttle * m_turnSpeed * Time.deltaTime;
        diff = Mathf.Clamp(diff, -maxRate, maxRate);
        ang.y += diff;
        transform.localEulerAngles = ang;

        // update the animation
        Vector3 moveChar = transform.InverseTransformDirection(m_input.m_direction);
        m_anim.SetFloat("fwdSpeed", moveChar.z);
        m_anim.SetFloat("rightSpeed", moveChar.x);

        if (m_input.m_attack)
        {
            m_anim.SetTrigger("doAttack");
            m_input.m_attack = false;
        }
        if (m_input.m_jump)
        {
            m_anim.SetTrigger("doJump");
            m_input.m_jump = false;
        }
        m_anim.SetBool("onGround", m_char.isGrounded);
    }
}