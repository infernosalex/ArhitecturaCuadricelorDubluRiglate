using UnityEngine;

public class SmoothRotCopy : MonoBehaviour {
    public Transform target;
    public Transform cam;
    public float rotationSmooth, zoomSpeed, zoomSmooth;


    float zoom = 10f;
    float rawZoom = 10f;
    private void Update() {
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotationSmooth);

        rawZoom -= Input.mouseScrollDelta.y * zoomSpeed;
        rawZoom = Mathf.Clamp(rawZoom, 1f, 20f);

        zoom = Mathf.Lerp(zoom, rawZoom, zoomSmooth * Time.deltaTime);

        cam.localPosition = new Vector3(0, 0, -zoom);
    }
}
