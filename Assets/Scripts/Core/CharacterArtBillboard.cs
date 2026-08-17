using UnityEngine;

namespace SeasOfLegends.Core
{
    /// <summary>
    /// Keeps a transparent character concept panel facing the prototype camera. It is a temporary
    /// presentation bridge until final rigged character models and animation controllers arrive.
    /// </summary>
    public sealed class CharacterArtBillboard : MonoBehaviour
    {
        [SerializeField] private bool lockVertical = true;
        private Transform targetCamera;

        private void LateUpdate()
        {
            if (targetCamera == null && Camera.main != null) targetCamera = Camera.main.transform;
            if (targetCamera == null) return;

            Vector3 lookDirection = targetCamera.position - transform.position;
            if (lockVertical) lookDirection.y = 0f;
            if (lookDirection.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
        }
    }
}
