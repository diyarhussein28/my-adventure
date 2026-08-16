using UnityEngine;
using UnityEngine.SceneManagement;

namespace SeasOfLegends.Core
{
    /// <summary>
    /// Required Components:
    /// - None (pure manager)
    /// 
    /// Singleton game manager handling high-level game state,
    /// scene transitions, and pause functionality.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Exploration;
        public GameState CurrentState => currentState;

        [Header("Scene Names")]
        [SerializeField] private string bootSceneName = "Boot";
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private string oceanWorldSceneName = "OceanWorld";
        [SerializeField] private string arenaSceneName = "ArenaDuel";

        [Header("Time Settings")]
        [SerializeField] private float defaultTimeScale = 1f;
        [SerializeField] private float pausedTimeScale = 0f;

        private bool isPaused = false;
        public bool IsPaused => isPaused;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.OnPauseRequested += TogglePause;
        }

        private void OnDisable()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.OnPauseRequested -= TogglePause;
        }

        /// <summary>
        /// Transitions the game to a new high-level state.
        /// Triggers camera, input, and time-dilation changes.
        /// </summary>
        public void SetGameState(GameState newState)
        {
            if (currentState == newState) return;

            GameState previousState = currentState;
            currentState = newState;

            OnStateEnter(newState, previousState);
        }

        private void OnStateEnter(GameState newState, GameState previousState)
        {
            switch (newState)
            {
                case GameState.Exploration:
                    Time.timeScale = defaultTimeScale;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;

                case GameState.Combat:
                    Time.timeScale = defaultTimeScale;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;

                case GameState.ArenaDuel:
                    Time.timeScale = defaultTimeScale;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    // Camera transition handled by CameraController
                    break;

                case GameState.Cinematic:
                    Time.timeScale = defaultTimeScale;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = false;
                    break;

                case GameState.Paused:
                    Time.timeScale = pausedTimeScale;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;

                case GameState.MainMenu:
                    Time.timeScale = defaultTimeScale;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
            }
        }

        public void TogglePause()
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        public void PauseGame()
        {
            isPaused = true;
            SetGameState(GameState.Paused);
        }

        public void ResumeGame()
        {
            isPaused = false;
            SetGameState(GameState.Exploration);
        }

        public void LoadMainMenu()
        {
            SetGameState(GameState.MainMenu);
            SceneManager.LoadScene(mainMenuSceneName);
        }

        public void StartNewGame()
        {
            SceneManager.LoadScene(oceanWorldSceneName);
            SetGameState(GameState.Exploration);
        }

        public void LoadArenaDuel()
        {
            // Async load arena additively, then transition
            SetGameState(GameState.ArenaDuel);
        }

        public void ReturnToOcean()
        {
            SetGameState(GameState.Exploration);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    public enum GameState
    {
        Boot,
        MainMenu,
        Exploration,    // Open world, free camera
        Combat,         // Regular enemy encounters
        ArenaDuel,      // Elite/boss locked arena
        Cinematic,      // Cutscenes, finishers
        Paused
    }
}
