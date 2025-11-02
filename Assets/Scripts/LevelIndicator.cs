using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// Para mostrar en que nivel va al jugador
public class LevelIndicator : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text levelText;

    [Header("Settings")]
    [SerializeField] private string levelPrefix = "NIVEL ";

    void Start()
    {
        UpdateLevelDisplay();
    }

    private void UpdateLevelDisplay()
    {
        if (levelText == null) return;

        string sceneName = SceneManager.GetActiveScene().name;

        int levelNumber = GetLevelNumberFromSceneName(sceneName);

        if (levelNumber > 0)
        {
            levelText.text = $"{levelPrefix}{levelNumber}";
        }
        else
        {
            levelText.text = sceneName;
        }
    }

    private int GetLevelNumberFromSceneName(string sceneName)
    {
        if (sceneName.Contains("Level_") || sceneName.Contains("level_"))
        {
            string[] parts = sceneName.Split('_');
            if (parts.Length > 1)
            {
                if (int.TryParse(parts[1], out int levelNum))
                {
                    return levelNum;
                }
            }
        }

        return 0;
    }
}