using UnityEngine;
using Cinemachine; // Requires Cinemachine package

/// <summary>
/// Manages camera transitions between free-roam exploration and locked-on combat.
/// Attach to a GameObject in the scene with Cinemachine Virtual Cameras.
/// </summary>
[RequireComponent(typeof(CinemachineBrain))]
public class CameraController : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera freeLookCamera; // Default 3rd-person follow camera
    public CinemachineVirtualCamera combatCamera;   // Locked-on arena camera (2.5D or 3D)

    [Header("Settings")]
    public float transitionTime = 0.5f; // Smooth transition duration
    public Vector3 combatCameraOffset = new Vector3(0, 2, -5); // Offset from player when locked on
    public bool lockOnToTarget = false; // Set to true when in combat

    // Internal
    private Transform playerTransform;
    private Transform targetTransform; // Enemy or focus target during lock-on
    private CinemachineVirtualCamera activeCamera;

    private void Awake()
    {
        // Find player (assumes tag "Player")
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;

        // Ensure we have at least one camera
        if (freeLookCamera == null)
        {
            Debug.LogWarning("CameraController: FreeLookCamera not assigned. Trying to find one.");
            freeLookCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }
    }

    private void Start()
    {
        // Start with free-look camera
        SwitchToFreeLook();
    }

    private void Update()
    {
        // Update combat camera target if lock-on is active
        if (lockOnToTarget && combatCamera != null && targetTransform != null)
        {
            // Position combat camera behind player, looking at target
            Vector3 desiredPos = playerTransform.position + combatCameraOffset;
            // Ensure camera stays above ground
            desiredPos.y = Mathf.Max(desiredPos.y, playerTransform.position.y + 1.5f);
            combatCamera.transform.position = desiredPos;
            combatCamera.transform.LookAt(targetTransform);
        }
    }

    /// <summary>
    /// Switch to free-roam exploration camera.
    /// </summary>
    public void SwitchToFreeLook()
    {
        if (freeLookCamera != null)
        {
            freeLookCamera.Priority = 10; // Higher priority = active
            if (combatCamera != null)
                combatCamera.Priority = 0;
            activeCamera = freeLookCamera;
            lockOnToTarget = false;
        }
    }

    /// <summary>
    /// Switch to locked-on combat camera focusing on a target.
    /// </summary>
    /// <param name="target">The enemy or object to lock onto</param>
    public void SwitchToCombatCamera(Transform target)
    {
        if (combatCamera == null) return;

        targetTransform = target;
        lockOnToTarget = true;

        // Set priorities to blend
        freeLookCamera.Priority = 0;
        combatCamera.Priority = 10;
        activeCamera = combatCamera;

        // Optional: force immediate position update
        if (playerTransform != null && targetTransform != null)
        {
            Vector3 desiredPos = playerTransform.position + combatCameraOffset;
            desiredPos.y = Mathf.Max(desiredPos.y, playerTransform.position.y + 1.5f);
            combatCamera.transform.position = desiredPos;
            combatCamera.transform.LookAt(targetTransform);
        }
    }

    /// <summary>
    /// Returns whether the combat camera is currently active.
    /// </summary>
    public bool IsCombatCameraActive() => activeCamera == combatCamera;

    /// <summary>
    /// Called by CombatSystem when entering execution (finisher) to lock camera tightly.
    /// </summary>
    public void LockForFinisher(Transform enemyTransform)
    {
        SwitchToCombatCamera(enemyTransform);
        // Optionally adjust combatCameraOffset for closer shot
        combatCameraOffset = new Vector3(0, 1.5f, -3f);
    }

    /// <summary>
    /// Called by CombatSystem when execution ends to return to free-look.
    /// </summary>
    public void UnlockFromFinisher()
    {
        SwitchToFreeLook();
        combatCameraOffset = new Vector3(0, 2, -5); // Reset offset
    }
}