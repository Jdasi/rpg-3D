using UnityEngine;

public class TabletopCamera : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float _moveSpeed = 8f;
    [SerializeField] float _sprintSpeedModifier = 2f;
    [SerializeField] float _walkSpeedModifier = 0.5f;

    [Header("Sensitivity")]
    [SerializeField] float _sensitivityX = 20f;
    [SerializeField] float _sensitivityY = 20f;
    [SerializeField] float _sensitivityZoom = 200f;

    [Header("Restrictions")]
    [SerializeField] float _pitchMin = 20f;
    [SerializeField] float _pitchMax = 90f;
    [SerializeField] float _zoomClosest = 3f;
    [SerializeField] float _zoomFurthest = -15f;

    [Header("References")]
    [SerializeField] Transform _pitchPivot;
    [SerializeField] Camera _camera;

    private bool _orbiting;

    private void Update()
    {
        bool movedThisFrame = false;
        _orbiting = Input.GetMouseButton(1);

        Cursor.visible = !_orbiting;
        Cursor.lockState = _orbiting ? CursorLockMode.Locked : CursorLockMode.None;

        if (_orbiting)
        {
            HandleOrbit(ref movedThisFrame);
        }

        HandleMovement(ref movedThisFrame);
        HandleZoom();

        if (movedThisFrame)
        {
            LocalEvents.TabletopCameraMoved.Invoke(this);
        }
    }

    private void HandleOrbit(ref bool movedThisFrame)
    {
        float mx = Input.GetAxis("mx");
        float my = Input.GetAxis("my");

        if (mx == 0 && my == 0)
        {
            return;
        }

        float yaw = transform.localRotation.eulerAngles.y;
        float pitch = _pitchPivot.localRotation.eulerAngles.x;

        yaw += mx * Time.unscaledDeltaTime * _sensitivityX;
        pitch -= my * Time.unscaledDeltaTime * _sensitivityY;
        pitch = Mathf.Clamp(pitch, _pitchMin, _pitchMax);

        transform.localRotation = Quaternion.Euler(0, yaw, 0);
        _pitchPivot.localRotation = Quaternion.Euler(pitch, 0, 0);
        movedThisFrame = true;
    }

    private void HandleMovement(ref bool movedThisFrame)
    {
        float v = Input.GetAxisRaw("v");
        float h = Input.GetAxisRaw("h");

        if (v == 0 && h == 0)
        {
            return;
        }

        float speedModifier;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speedModifier = _sprintSpeedModifier;
        }
        else if (Input.GetKey(KeyCode.LeftAlt))
        {
            speedModifier = _walkSpeedModifier;
        }
        else
        {
            speedModifier = 1;
        }

        Vector3 forwardMove = v * transform.forward;
        Vector3 sideMove = h * transform.right;

        transform.position += _moveSpeed * speedModifier * Time.unscaledDeltaTime * (forwardMove + sideMove);
        movedThisFrame = true;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("ms");

        if (scroll == 0)
        {
            return;
        }

        Vector3 zoomVec = _camera.transform.localPosition;
        zoomVec.z += scroll * _sensitivityZoom * Time.unscaledDeltaTime;
        zoomVec.z = Mathf.Clamp(zoomVec.z, _zoomFurthest, _zoomClosest);
        _camera.transform.localPosition = zoomVec;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
