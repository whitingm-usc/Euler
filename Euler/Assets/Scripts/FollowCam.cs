using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    float m_distanceCurrent;
    float m_distanceOrig;
    float m_azimuth;
    float m_elevation;
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
        p.y = 0.0f;
        float dxz = p.magnitude;
        m_azimuth = Mathf.Atan2(p.x, p.z);
        m_elevation = Mathf.Atan2(p.y, dxz);

        m_playerInput = GetComponent<PlayerInput>();

        m_input = new CamInput();
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
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
    }

    void LateUpdate()
    {
        Vector3 target = m_target.transform.TransformPoint(m_targetOffset);
        Vector3 p;
        p.y = m_distanceCurrent * Mathf.Sin(m_elevation);
        float dxz = m_distanceCurrent * Mathf.Cos(m_elevation);
        p.x = dxz * Mathf.Sin(m_azimuth);
        p.z = dxz * Mathf.Cos(m_azimuth);

        // camera collisions
        Ray ray = new Ray(target, p);
        RaycastHit hitInfo;
        if (Physics.SphereCast(ray, m_collRad, out hitInfo, m_distanceCurrent + m_collRad))
        {
            m_distanceCurrent = hitInfo.distance - m_collRad;
            p.y = m_distanceCurrent * Mathf.Sin(m_elevation);
            dxz = m_distanceCurrent * Mathf.Cos(m_elevation);
            p.x = dxz * Mathf.Sin(m_azimuth);
            p.z = dxz * Mathf.Cos(m_azimuth);
        }
        else
        {
            float lerp = Mathf.Clamp01(m_distSpeed * Time.fixedDeltaTime);
            m_distanceCurrent = Mathf.Lerp(m_distanceCurrent, m_distanceOrig, lerp);
        }

        p += target;
        transform.position = p;
#if false   // this has a gimbal lock issue.
        if (m_elevation > 0.5f * Mathf.PI)
            transform.LookAt(target, Vector3.down);
        else
            transform.LookAt(target, Vector3.up);
#else
        Vector3 ang = new Vector3(Mathf.Rad2Deg * m_elevation, 180.0f + Mathf.Rad2Deg * m_azimuth, 0.0f);
        transform.localEulerAngles = ang;
#endif
    }
}
