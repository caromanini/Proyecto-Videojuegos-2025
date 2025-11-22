using UnityEngine;

[CreateAssetMenu(menuName = "Level Data/Lavels", fileName = "New Level")]

public class LevelData : ScriptableObject
{
    [Header("Level Stats")]
    public string LevelID;
    [Tooltip("For Starting Levels")] public bool IsUnlockedByDefault;
    public SceneField Scene;

    [Header("Level Display Information")]
    public string LevelName;

    public GameObject LevelButtonObj { get; set; }
}
