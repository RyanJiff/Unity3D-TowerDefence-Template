using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class CameraController : MonoBehaviour
{
    /*
     * Player Camera controls.
     */

    [SerializeField] Transform cameraRig = null;
    Transform mainCamera = null;

    [SerializeField] float rotationSpeed = 0.05f;
    [SerializeField] float zoom_min = 3;
    [SerializeField] float zoom_max = 15;
    [SerializeField] float zoom_speed = 0.5f;
    [SerializeField] float movement_speed = 2f;
    [SerializeField] float map_size = 50;

    private Vector3 currentMousePos;
    private Vector3 mousePosLastFrame;
    private Vector3 mouseDelta;
    private Vector3 rigRotation;

    private void Start()
    {
        if (!cameraRig)
        {
            Debug.LogWarning("No Camera Rig Assigned to camera controller!");
        }
        mainCamera = Camera.main.transform;
        mousePosLastFrame = Input.mousePosition;

    }

    private void Update()
    {
        if (!cameraRig)
        {
            return;
        }

        currentMousePos = Input.mousePosition;
        mouseDelta = mousePosLastFrame - currentMousePos;
        if (Input.GetKey(KeyCode.Mouse1))
        {  
            cameraRig.Rotate(new Vector3(0, -mouseDelta.x * rotationSpeed, 0), Space.World);
            cameraRig.Rotate(new Vector3(mouseDelta.y * rotationSpeed, 0, 0), Space.Self);
        }
        mousePosLastFrame = currentMousePos;

        mainCamera.localPosition -= new Vector3(0, 0, Input.mouseScrollDelta.y * zoom_speed * mainCamera.localPosition.z);

        mainCamera.localPosition = new Vector3(0, 0, Mathf.Clamp(mainCamera.localPosition.z, -zoom_max, -zoom_min));

        if (Input.GetKey(KeyCode.W))
        {
            cameraRig.position += new Vector3(cameraRig.forward.normalized.x * movement_speed * Time.deltaTime, 0, cameraRig.forward.normalized.z * movement_speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            cameraRig.position -= new Vector3(cameraRig.forward.normalized.x * movement_speed * Time.deltaTime, 0, cameraRig.forward.normalized.z * movement_speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            cameraRig.position -= new Vector3(cameraRig.right.normalized.x * movement_speed * Time.deltaTime, 0, cameraRig.right.normalized.z * movement_speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            cameraRig.position += new Vector3(cameraRig.right.normalized.x * movement_speed * Time.deltaTime, 0, cameraRig.right.normalized.z * movement_speed * Time.deltaTime);
        }

        cameraRig.position = new Vector3(Mathf.Clamp(cameraRig.position.x, -map_size / 2, map_size/2), cameraRig.position.y, Mathf.Clamp(cameraRig.position.z, -map_size / 2, map_size/2));

    }
}
