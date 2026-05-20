using UnityEngine;
using TMPro;
using UnityEngine.Audio;

public class AngleText : MonoBehaviour
{
    public enum EAngleType
    {
        Yaw,
        Pitch,
        Roll
    }

    public EAngleType m_angleType = EAngleType.Yaw;
    public Transform m_target;

    TextMeshProUGUI m_text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        float angle = 0.0f;
        switch (m_angleType)
        {
            case EAngleType.Yaw:
                angle = m_target.localEulerAngles.y;
                break;
            case EAngleType.Pitch:
                angle = m_target.localEulerAngles.x;
                break;
            case EAngleType.Roll:
                angle = m_target.localEulerAngles.z;
                break;
        }
        angle = (angle > 180.0f) ? angle - 360.0f : angle;
        angle = Mathf.Round(angle);
        m_text.text = angle.ToString();
    }
}
