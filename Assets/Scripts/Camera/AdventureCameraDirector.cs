using Cinemachine;
using UnityEngine;

namespace SeasOfLegends.CameraSystem
{
    public enum CameraMode { Exploration, Combat, Arena, Cinematic }

    /// <summary>
    /// Assign three Cinemachine Virtual Cameras. Higher priority wins, allowing smooth blends
    /// authored in the main Cinemachine Brain. Set combatTarget when locking onto an elite enemy.
    /// </summary>
    public sealed class AdventureCameraDirector : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera explorationCamera;
        [SerializeField] private CinemachineVirtualCamera combatCamera;
        [SerializeField] private CinemachineVirtualCamera arenaCamera;
        [SerializeField] private CinemachineVirtualCamera cinematicCamera;
        [SerializeField] private int activePriority = 20;
        [SerializeField] private int inactivePriority = 0;

        public CameraMode Mode { get; private set; } = CameraMode.Exploration;

        private void Start() => SetMode(CameraMode.Exploration);

        public void SetMode(CameraMode mode, Transform combatTarget = null)
        {
            Mode = mode;
            SetPriority(explorationCamera, mode == CameraMode.Exploration);
            SetPriority(combatCamera, mode == CameraMode.Combat);
            SetPriority(arenaCamera, mode == CameraMode.Arena);
            SetPriority(cinematicCamera, mode == CameraMode.Cinematic);

            if (combatTarget != null)
            {
                if (combatCamera != null) combatCamera.LookAt = combatTarget;
                if (arenaCamera != null) arenaCamera.LookAt = combatTarget;
            }
        }

        private void SetPriority(CinemachineVirtualCamera virtualCamera, bool active)
        {
            if (virtualCamera != null) virtualCamera.Priority = active ? activePriority : inactivePriority;
        }
    }
}
