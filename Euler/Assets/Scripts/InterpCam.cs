using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static FollowCam;

public class InterpCam : MonoBehaviour
{
    public GameObject m_target;
    public Vector3 m_targetOffset;
    public float m_lerp = 0.0f;

    Quaternion m_quatCurrent;
    Quaternion m_quatTarget;
    Vector3 m_cameraArm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 target = m_target.transform.TransformPoint(m_targetOffset);
        m_cameraArm = transform.InverseTransformPoint(target);
        m_quatCurrent = transform.rotation;
        m_quatTarget = m_quatCurrent;
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            m_quatTarget = Quaternion.Euler(0.0f, 180.0f, 0.0f) * m_quatTarget;
        }
    }

    void LateUpdate()
    {
        Vector3 target = m_target.transform.TransformPoint(m_targetOffset);
        transform.rotation = Quaternion.Slerp(m_quatCurrent, m_quatTarget, m_lerp);
        transform.position = target - transform.rotation * m_cameraArm;
    }
}
