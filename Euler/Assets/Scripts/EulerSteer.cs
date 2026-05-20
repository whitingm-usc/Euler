using UnityEngine;
using UnityEngine.InputSystem;

public class EulerSteer : MonoBehaviour
{
    public Transform m_yaw;
    public Transform m_pitch;
    public Transform m_roll;
    public float m_steerSpeed = 360.0f;
    public Vector3[] m_presets;

    Vector3 m_euler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_euler = Vector3.zero;
        m_euler.y = m_yaw.localEulerAngles.y;
        m_euler.x = m_pitch.localEulerAngles.x;
        m_euler.z = m_roll.localEulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        float p = 0.0f;
        float q = 0.0f;
        float r = 0.0f;
        if (Keyboard.current != null)
        { 
            if (Keyboard.current.leftArrowKey.isPressed)
                r -= m_steerSpeed * Time.deltaTime;
            if (Keyboard.current.rightArrowKey.isPressed)
                r += m_steerSpeed * Time.deltaTime;
            if (Keyboard.current.upArrowKey.isPressed)
                q -= m_steerSpeed * Time.deltaTime;
            if (Keyboard.current.downArrowKey.isPressed)
                q += m_steerSpeed * Time.deltaTime;
            if (Keyboard.current.qKey.isPressed)
                p -= m_steerSpeed * Time.deltaTime;
            if (Keyboard.current.eKey.isPressed)
                p += m_steerSpeed * Time.deltaTime;

            EulerRotate(p, q, r);

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                DoPreset(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
                DoPreset(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
                DoPreset(2);
        }

        m_yaw.localEulerAngles = new Vector3(0.0f, m_euler.y, 0.0f);
        m_pitch.localEulerAngles = new Vector3(m_euler.x, 0.0f, 0.0f);
        m_roll.localEulerAngles = new Vector3(0.0f, 0.0f, m_euler.z);
    }

    void EulerRotate(float p, float q, float r)
    {
#if false   // this is the "naive" way to do it, which doesn't handle the poles well.  It can be used as a reference to check the more complex code below.
        float sinphi = Mathf.Sin(m_euler.z * Mathf.Deg2Rad);
        float cosphi = Mathf.Cos(m_euler.z * Mathf.Deg2Rad);
        float sintheta = Mathf.Sin(m_euler.x * Mathf.Deg2Rad);
        float costheta = Mathf.Cos(m_euler.x * Mathf.Deg2Rad);
        float sinpsi = Mathf.Sin(m_euler.y * Mathf.Deg2Rad);
        float cospsi = Mathf.Cos(m_euler.y * Mathf.Deg2Rad);
        float tantheta = Mathf.Tan(m_euler.x * Mathf.Deg2Rad);

        float phi_dot = p + q * sinphi * tantheta + r * cosphi * tantheta;
        float theta_dot = q * cosphi - r * sinphi;
        float psi_dot = q * sinphi / costheta + r * cosphi / costheta;

        m_euler.y += psi_dot;
        m_euler.x += theta_dot;
        m_euler.z += phi_dot;
#else
        float sinphi = Mathf.Sin(m_euler.z * Mathf.Deg2Rad);
        float cosphi = Mathf.Cos(m_euler.z * Mathf.Deg2Rad);
        float theta_dot = q * cosphi - r * sinphi;
        m_euler.x += theta_dot;

        float phi_dot = 0.0f;
        float psi_dot = 0.0f;

        float sintheta = Mathf.Sin(m_euler.x * Mathf.Deg2Rad);
        float costheta = Mathf.Cos(m_euler.x * Mathf.Deg2Rad);
        if (Mathf.Abs(costheta) < 0.001f)
        {   // at the poles, just put it all into phi_dot, and ignore psi_dot.
            costheta = 0.001f * Mathf.Sign(costheta);
            phi_dot = p + r;
            psi_dot = 0.0f;
        }
        else
        {

            float tantheta = Mathf.Tan(m_euler.x * Mathf.Deg2Rad);
            float sinpsi = Mathf.Sin(m_euler.y * Mathf.Deg2Rad);
            float cospsi = Mathf.Cos(m_euler.y * Mathf.Deg2Rad);
            phi_dot = p + q * sinphi * tantheta + r * cosphi * tantheta;
            psi_dot = q * sinphi / costheta + r * cosphi / costheta;
        }

        m_euler.y += psi_dot;
        m_euler.z += phi_dot;
#endif
    }

    void DoPreset(int index)
    {
        if (index < 0 || index >= m_presets.Length)
            return;
        m_euler = m_presets[index];
    }
}
