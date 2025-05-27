using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraPivot : MonoBehaviour {
    public Vector2 sensitivity;
    public Transform body;

    float xAngle, yAngle = 45;

    bool isDragging;

    private void Awake() {
        transform.localRotation = Quaternion.Euler(yAngle, 0, 0);
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            isDragging = true;
        }

        if(Input.GetMouseButtonUp(0)) {
            isDragging = false;
        }

        if(!isDragging) return;

        Vector2 delta = Mouse.current.delta.value;
        yAngle -= delta.y * sensitivity.y;
        yAngle = Mathf.Clamp(yAngle, -85f, 85f);

        float xAngle = delta.x * sensitivity.x;

        transform.localRotation = Quaternion.Euler(yAngle, 0, 0);
        body.Rotate(Vector3.up * xAngle);
    }
}
