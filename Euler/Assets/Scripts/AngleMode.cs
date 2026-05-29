using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class AngleMode : MonoBehaviour
{
    public enum Mode
    {
        Euler,
        Quat,
        QuatLookAt,
        QuatFromEuler
    }
    public static Mode s_mode = Mode.Quat;
    public TextMeshProUGUI m_text;

    Mode oldMode;

    private void Start()
    {
        if (m_text == null)
        {
            m_text = GetComponent<TextMeshProUGUI>();
        }
        oldMode = s_mode;
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            s_mode = (Mode)(((int)s_mode + 1) % System.Enum.GetValues(typeof(Mode)).Length);
        }
        if (s_mode != oldMode)
        {
            UpdateText();
            oldMode = s_mode;
        }
    }

    void UpdateText()
    {
        m_text.text = $"Angle Mode: {s_mode}";
    }
}
