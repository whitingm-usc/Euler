using UnityEngine;
using UnityEngine.InputSystem;

public class TargetOnSphere : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000.0f))
            {
                transform.position = hitInfo.point;
            }
        }
    }
}
