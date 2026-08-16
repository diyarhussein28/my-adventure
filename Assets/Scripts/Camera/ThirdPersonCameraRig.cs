using UnityEngine;
using UnityEngine.InputSystem;

namespace SeasOfLegends.CameraSystem
{
    /// <summary>
    /// Self-contained prototype camera. Production scenes may replace this with the Cinemachine
    /// director; this rig deliberately remains dependency-light for a fresh project import.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class ThirdPersonCameraRig : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 pivotOffset = new Vector3(0f, 1.65f, 0f);
        [SerializeField] private float distance = 6.4f;
        [SerializeField] private float sensitivity = 0.12f;
        [SerializeField] private float followSharpness = 14f;
        private float yaw = 180f;
        private float pitch = 17f;

        public void ConfigureForPrototype(Transform followTarget)
        {
            target = followTarget;
            if (target != null) yaw = target.eulerAngles.y;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (target == null) return;
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked)
            {
                Vector2 delta = Mouse.current.delta.ReadValue();
                yaw += delta.x * sensitivity;
                pitch = Mathf.Clamp(pitch - delta.y * sensitivity, -15f, 52f);
            }

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 pivot = target.position + pivotOffset;
            Vector3 desiredPosition = pivot - rotation * Vector3.forward * distance;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, 1f - Mathf.Exp(-followSharpness * Time.unscaledDeltaTime));
            transform.rotation = rotation;
        }
    }
}
