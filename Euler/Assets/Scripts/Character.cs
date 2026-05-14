using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    public float m_walkSpeed = 4.0f;
    public float m_turnSpeed = 360.0f;  // degrees per second

    public class CharInput
    {
        public Vector3 m_direction;
        public float m_facingAngle;
        public bool m_attack;
    }

    Animator m_anim;
    CharacterController m_char;
    CharInput m_input;
    PlayerInput m_playerInput;

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

        // read attack input
#if false
        if (Input.GetMouseButtonDown(0))
            m_input.m_attack = true;
#endif

        // convert from camera-space to world-space movement
        Vector3 fwd = Camera.main.transform.forward;
        fwd.y = 0.0f;
        fwd.Normalize();
        Vector3 rt = Camera.main.transform.right;
        rt.y = 0.0f;
        rt.Normalize();
        Vector3 moveWorld = move.y * fwd + move.x * rt;
        m_input.m_direction = moveWorld;

#if false
        // face the way the camera faces
        m_input.m_facingAngle = Camera.main.transform.localEulerAngles.y;
#else
        // face the way we are moving
        float throttle = m_input.m_direction.magnitude;
        if (moveWorld.magnitude > 0.01f)
            m_input.m_facingAngle = Mathf.Atan2(m_input.m_direction.x, m_input.m_direction.z) * Mathf.Rad2Deg;
        else
            m_input.m_facingAngle = transform.localEulerAngles.y;
#endif

        // move in the direction of the input
        m_char.SimpleMove(m_input.m_direction * m_walkSpeed);

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
    }
}