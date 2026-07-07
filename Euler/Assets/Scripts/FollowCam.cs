using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class FollowCam : MonoBehaviour
{
    public GameObject m_target;
    public Vector3 m_targetOffset;
    public float m_mouseSensitivity = 0.1f;
    public float m_panSpeed = 180.0f;   // degrees per second
    public float m_tiltSpeed = 180.0f;  // degrees per second
    public float m_tiltMax = 60.0f;      // degrees
    public float m_tiltMin = 0.0f;    // degrees
    public float m_collRad = 0.1f;
    public float m_distSpeed = 4.0f;
    public float m_lookAhead = 2.0f;
    bool m_doQuat = false;

    float m_distanceCurrent;
    float m_distanceOrig;
    float m_azimuth;
    float m_elevation;
    Vector3 m_lookAheadVec;
    Quaternion m_quat;
    PlayerInput m_playerInput;

    public class CamInput
    {
        public float m_pan;
        public float m_tilt;
    }
    CamInput m_input;

    void Start()
    {
        Vector3 target = m_target.transform.TransformPoint(m_targetOffset);
        Vector3 p = transform.position - target;
        m_distanceCurrent = p.magnitude;
        m_distanceOrig = m_distanceCurrent;
        Vector3 pxz = p;
        pxz.y = 0.0f;
        float dxz = pxz.magnitude;
        m_azimuth = Mathf.Atan2(p.x, p.z);
        m_elevation = Mathf.Atan2(p.y, dxz);
        m_quat = transform.rotation;

        m_playerInput = GetComponent<PlayerInput>();

        m_input = new CamInput();
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        m_doQuat = AngleMode.s_mode != AngleMode.Mode.Euler;
        Vector2 look = m_playerInput.actions["Look"].ReadValue<Vector2>();
        m_input.m_pan = look.x;
        m_input.m_tilt = -look.y;
        bool isMouse = Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0;
        if (isMouse)
        {
            m_input.m_pan *= m_mouseSensitivity;
            m_input.m_tilt *= m_mouseSensitivity;
        }
        else
        {
            m_input.m_pan *= Time.deltaTime;
            m_input.m_tilt *= Time.deltaTime;
        }

        m_azimuth += m_input.m_pan * m_panSpeed * Mathf.Deg2Rad;
        if (m_azimuth > Mathf.PI)
            m_azimuth -= 2.0f * Mathf.PI;
        if (m_azimuth < -Mathf.PI)
            m_azimuth += 2.0f * Mathf.PI;
        m_elevation += m_input.m_tilt * m_tiltSpeed * Mathf.Deg2Rad;
        m_elevation = Mathf.Clamp(m_elevation, Mathf.Deg2Rad * m_tiltMin, Mathf.Deg2Rad * m_tiltMax);
        m_quat = m_quat * Quaternion.Euler(m_input.m_tilt * m_tiltSpeed, 0.0f, 0.0f);
        m_quat = Quaternion.Euler(0.0f, m_input.m_pan * m_panSpeed, 0.0f) * m_quat;
    }

    void LateUpdate()
    {
        Vector3 target = m_target.transform.TransformPoint(m_targetOffset);
        Vector3 p;
        if (m_doQuat)
        {
            transform.rotation = m_quat;
            p = new Vector3(0.0f, 0.0f, -m_distanceCurrent);
            p = m_quat * p;

            // camera collisions
            Ray ray = new Ray(target, p);
            RaycastHit hitInfo;
            int mask = ~LayerMask.GetMask("Player");
            if (Physics.SphereCast(ray, m_collRad, out hitInfo, m_distanceCurrent + m_collRad, mask))
            {
                m_distanceCurrent = hitInfo.distance - m_collRad;
                p = new Vector3(0.0f, 0.0f, -m_distanceCurrent);
                p = m_quat * p;
            }
            else
            {
                float lerp = Mathf.Clamp01(m_distSpeed * Time.deltaTime);
                m_distanceCurrent = Mathf.Lerp(m_distanceCurrent, m_distanceOrig, lerp);
            }
        }
        else
        {
            p.y = m_distanceCurrent * Mathf.Sin(m_elevation);
            float dxz = m_distanceCurrent * Mathf.Cos(m_elevation);
            p.x = dxz * Mathf.Sin(m_azimuth);
            p.z = dxz * Mathf.Cos(m_azimuth);

            // camera collisions
            Ray ray = new Ray(target, p);
            RaycastHit hitInfo;
            int mask = ~LayerMask.GetMask("Player");
            if (Physics.SphereCast(ray, m_collRad, out hitInfo, m_distanceCurrent + m_collRad, mask))
            {
                m_distanceCurrent = hitInfo.distance - m_collRad;
                p.y = m_distanceCurrent * Mathf.Sin(m_elevation);
                dxz = m_distanceCurrent * Mathf.Cos(m_elevation);
                p.x = dxz * Mathf.Sin(m_azimuth);
                p.z = dxz * Mathf.Cos(m_azimuth);
            }
            else
            {
                float lerp = Mathf.Clamp01(m_distSpeed * Time.deltaTime);
                m_distanceCurrent = Mathf.Lerp(m_distanceCurrent, m_distanceOrig, lerp);
            }
        }

        p += target;
        transform.position = p;
        if (!m_doQuat)
        {
#if false   // this has a gimbal lock issue.
            if (m_elevation > 0.5f * Mathf.PI)
                transform.LookAt(target, Vector3.down);
            else
                transform.LookAt(target, Vector3.up);
#else
            // Look ahead
            Vector3 lookTarget = m_lookAhead * m_target.transform.forward + target;
            Ray ray = new Ray(target, lookTarget);
            RaycastHit hitInfo;
            int mask = ~LayerMask.GetMask("Player");
            if (Physics.SphereCast(ray, m_collRad, out hitInfo, m_lookAhead + m_collRad, mask))
            {
                lookTarget = hitInfo.point;
            }
            lookTarget -= target;
            m_lookAheadVec = Vector3.Lerp(m_lookAheadVec, lookTarget, 0.1f);
            lookTarget = m_lookAheadVec + target;
            Vector3 toLook = p - lookTarget;
            Vector3 toLookXZ = toLook;
            toLookXZ.y = 0.0f;
            float dxz = toLookXZ.magnitude;
            float az = Mathf.Atan2(toLook.x, toLook.z);
            float el = Mathf.Atan2(toLook.y, dxz);

            Vector3 ang = new Vector3(Mathf.Rad2Deg * el, 180.0f + Mathf.Rad2Deg * az, 0.0f);
            transform.localEulerAngles = ang;
#endif
        }
    }
}
