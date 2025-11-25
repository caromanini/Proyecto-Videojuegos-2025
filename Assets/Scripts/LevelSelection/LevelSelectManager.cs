using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LevelSelectManager : MonoBehaviour
{
    public Transform LevelParent;
    public GameObject LevelButtonPrefab;
    public TextMeshProUGUI AreaHeaderText;
    public TextMeshProUGUI LevelHeaderText;
    public AreaData CurrentArea;

    [Header("Opcional: sprites por índice (si los asignas aquí, se usarán en orden)")]
    public Sprite[] LevelSprites;

    public HashSet<string> UnlockedLevelIDs = new HashSet<string>();

    private Camera _camera;

    private List<GameObject> _buttonObjects= new List<GameObject>();
    private Dictionary<GameObject, Vector3> _buttonLocations = new Dictionary<GameObject, Vector3>();

    // índice del botón actualmente seleccionado (-1 = ninguno)
    private int _selectedIndex = -1;

    // Nuevo: referencia al player UI que se moverá entre botones
    public LevelSelectPlayer LevelPlayer;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        AssignAreaText();
        LoadUnlockedLevels();
        CreateLevelButtons();
        InitializeSelection();
    }

    public void AssignAreaText()
    {
        AreaHeaderText.SetText(CurrentArea.AreaName);
    }

    private void LoadUnlockedLevels()
    {
        foreach (var level in CurrentArea.Levels)
        {
            if (level.IsUnlockedByDefault)
            {
                UnlockedLevelIDs.Add(level.LevelID);
            }
        }
    }

    private void CreateLevelButtons()
    {
        for (int i = 0; i < CurrentArea.Levels.Count; i++)
        {
            GameObject buttonGO = Instantiate(LevelButtonPrefab, LevelParent);
            _buttonObjects.Add(buttonGO);

            RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();

            buttonGO.name = CurrentArea.Levels[i].LevelID;
            CurrentArea.Levels[i].LevelButtonObj = buttonGO;

            LevelButton levelButton = buttonGO.GetComponent<LevelButton>();

            // elegir sprite: primero el array LevelSprites (por orden), si no existe usar LevelData.LevelSprite
            Sprite spriteOverride = (LevelSprites != null && i < LevelSprites.Length) ? LevelSprites[i] : null;

            levelButton.Setup(CurrentArea.Levels[i], UnlockedLevelIDs.Contains(CurrentArea.Levels[i].LevelID), spriteOverride);
        }
    }

    private void Update()
    {
        // Navegación con teclado: izquierda (A o LeftArrow), derecha (D o RightArrow)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSelection(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSelection(1);
        }

        // Activar nivel seleccionado con Enter o Espacio
        if (_selectedIndex >= 0 && (_selectedIndex < _buttonObjects.Count))
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
            {
                var selGO = _buttonObjects[_selectedIndex];
                if (selGO != null)
                {
                    LevelButton lb = selGO.GetComponent<LevelButton>();
                    if (lb != null && lb.IsInteractable())
                    {
                        lb.LoadLevel();
                    }
                }
            }
        }
    }

    // Inicializa la selección en el primer botón desbloqueado (si existe)
    private void InitializeSelection()
    {
        int first = FindNextInteractable(0, 1);
        if (first != -1)
        {
            SelectButton(first);
            // Si hay player UI, colocarlo exactamente en el botón inicial
            if (LevelPlayer != null)
            {
                var go = _buttonObjects[_selectedIndex];
                var rt = go.GetComponent<RectTransform>();
                LevelPlayer.SnapToButton(rt);
            }
        }
    }

    // Mueve la selección en dirección -1 (izquierda) o +1 (derecha)
    private void MoveSelection(int direction)
    {
        if (_buttonObjects.Count == 0) return;

        int start = _selectedIndex;
        if (start < 0) start = 0;

        int next = FindNextInteractable(start + (direction > 0 ? 1 : -1), direction);

        // Si no se encontró desde la posición, intentar desde el extremo (wrap)
        if (next == -1)
        {
            if (direction > 0)
                next = FindNextInteractable(0, 1);
            else
                next = FindNextInteractable(_buttonObjects.Count - 1, -1);
        }

        if (next != -1)
        {
            SelectButton(next);
        }
    }

    // Busca el siguiente botón interactuable a partir de 'from' en pasos 'direction' (puede envolver)
    // Devuelve índice o -1 si no hay ninguno interactuable
    private int FindNextInteractable(int from, int direction)
    {
        if (_buttonObjects.Count == 0) return -1;

        int count = _buttonObjects.Count;
        int i = Mod(from, count);

        for (int checkedCount = 0; checkedCount < count; checkedCount++)
        {
            GameObject go = _buttonObjects[i];
            if (go != null)
            {
                LevelButton lb = go.GetComponent<LevelButton>();
                if (lb != null && lb.IsInteractable())
                {
                    return i;
                }
            }
            i = Mod(i + direction, count);
        }

        return -1;
    }

    // Selecciona el botón por índice y actualiza visual y EventSystem
    private void SelectButton(int index)
    {
        if (index < 0 || index >= _buttonObjects.Count) return;

        // Desmarcar el anterior
        if (_selectedIndex >= 0 && _selectedIndex < _buttonObjects.Count)
        {
            var prevGO = _buttonObjects[_selectedIndex];
            if (prevGO != null)
            {
                var prevLB = prevGO.GetComponent<LevelButton>();
                if (prevLB != null) prevLB.SetSelected(false);
            }
        }

        // Marcar nuevo
        _selectedIndex = index;
        var newGO = _buttonObjects[_selectedIndex];
        if (newGO != null)
        {
            var newLB = newGO.GetComponent<LevelButton>();
            if (newLB != null) newLB.SetSelected(true);

            // Actualizar EventSystem para que la navegación UI sea coherente
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(newGO);
            }

            // Mover el player UI al nuevo botón (si está asignado)
            if (LevelPlayer != null)
            {
                RectTransform rt = newGO.GetComponent<RectTransform>();
                LevelPlayer.MoveToButton(rt);
            }

            // Actualizar el texto del nivel (LevelHeaderText) con el LevelName del LevelData
            UpdateLevelHeaderFromButton(newLB);
        }
    }

    // Actualiza el LevelHeaderText usando los datos del LevelButton (usa LevelName si existe, sino LevelID)
    private void UpdateLevelHeaderFromButton(LevelButton lb)
    {
        if (LevelHeaderText == null || lb == null || lb.LevelData == null) return;

        string nameToShow = string.IsNullOrEmpty(lb.LevelData.LevelName) ? lb.LevelData.LevelID : lb.LevelData.LevelName;
        LevelHeaderText.SetText(nameToShow);
    }

    // Helper para modulo positivo
    private int Mod(int value, int m)
    {
        int r = value % m;
        return r < 0 ? r + m : r;
    }
}
