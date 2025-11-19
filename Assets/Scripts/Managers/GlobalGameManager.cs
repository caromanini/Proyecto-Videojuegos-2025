using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GlobalGameManager : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Cheats Enabled")]
    [SerializeField] private bool cheatsEnabled = true;

    void Update()
    {
        if (Keyboard.current == null) return;

        HandleNavigationInput();

        if (cheatsEnabled)
        {
            HandleCheatInput();
        }
    }

    private void HandleNavigationInput()
    {
        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    private void HandleCheatInput()
    {
        // Con F1 se gana el nivel (permite avanzar en el juego)
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            if (GameEventManager.Instance != null)
            {
                GameEventManager.Instance.TriggerPlayerWin();
            }
        }

        // Con F2 se "recolecta" una pieza de lente.
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            Collector collector = FindAnyObjectByType<Collector>();
            if (collector != null)
            {
                collector.AddGlasses(1);

                if (GameEventManager.Instance != null)
                {
                    GameEventManager.Instance.TriggerGlassPieceCollected(1);
                }
            }
        }

        // Con F3 se obtiene una almohada/escudo
        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            PlayerStateController state = FindAnyObjectByType<PlayerStateController>();
            if (state != null)
            {
                state.SetHasPillow(true);
            }
        }

        // Con F4 se obtiene una bebida energetica
        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            PlayerPhysicsController physics = FindAnyObjectByType<PlayerPhysicsController>();
            if (physics != null)
            {
                physics.ActivateSpeedBoost(15f, 10f);
            }
        }

        // Con F5 se obtienen las gotas de ojos
        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            Collector collector = FindAnyObjectByType<Collector>();
            if (collector != null)
            {
                collector.ApplyTemporaryFullGlasses(5f);
            }
        }
    }
}