using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSelector : MonoBehaviour
{
    public Camera[] m_cameras;
    Camera m_currentCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_currentCamera = m_cameras[0];
        for (int i = 0; i < m_cameras.Length; ++i)
        {
            m_cameras[i].enabled = i == 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                m_currentCamera.enabled = false;
                m_currentCamera = m_cameras[0];
                m_currentCamera.enabled = true;
            }
             if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                m_currentCamera.enabled = false;
                m_currentCamera = m_cameras[1];
                m_currentCamera.enabled = true;
            }
        }
    }
}
