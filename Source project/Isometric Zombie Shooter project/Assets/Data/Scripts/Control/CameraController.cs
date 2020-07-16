using UnityEngine;

/// <summary> Camera control </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target = null;   // target to chase
    [SerializeField] private float stopRadius = 2f;     // atempt to smooth stopping
    [SerializeField] private float smooth = 5f;

    private Vector3 cameraOffset = Vector3.zero;        // offset for isometric view

    private void Awake() 
    {
        // offset setting up by camera's start position
        cameraOffset = new Vector3(0, transform.position.y, transform.position.z);
    }

    // moves camera to target
    private void Update() 
    {
        MoveToTarget();
    }

    /// <summary> Moves camera to target </summary>
    private void MoveToTarget()
    {
        if((transform.position - target.position + cameraOffset).magnitude > stopRadius)
            transform.position = Vector3.Lerp(transform.position, target.position + cameraOffset, Time.deltaTime * smooth);
    }
}
