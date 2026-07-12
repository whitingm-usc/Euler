using UnityEngine;
using UnityEngine.UI;

public class SwitchScene : MonoBehaviour
{
    public string m_scene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button)
        {
            button.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(m_scene);
            });
        }
    }
}
