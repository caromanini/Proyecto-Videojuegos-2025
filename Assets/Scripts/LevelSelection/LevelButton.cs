using UnityEngine;
using UnityEngine.UI;
using TMPro; //just for better text handling
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelNameText;
    [SerializeField] private Image _artworkImage;

    public LevelData LevelData { get; set; }

    private Button _button;

    private Image _image;

    public Color ReturnColor { get; set; }

    private Vector3 _defaultScale;
    private Vector3 _selectedScale;
    private Coroutine _scaleCoroutine;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
        ReturnColor = Color.grey;

        _defaultScale = transform.localScale;
        _selectedScale = _defaultScale * 1.12f;
    }

    public void Setup(LevelData level, bool isUnlocked, Sprite overrideSprite = null)
    {
        LevelData = level;
        _levelNameText.SetText(level.LevelID);

        Sprite toUse = overrideSprite != null ? overrideSprite : level.LevelSprite;
        if (_artworkImage != null)
        {
            _artworkImage.sprite = toUse;
            _artworkImage.enabled = toUse != null;
        }

        _button.interactable = isUnlocked;

        if (isUnlocked)
        {
            _button.onClick.AddListener(LoadLevel);
            ReturnColor = Color.white;
            _image.color = ReturnColor;
        }
        else
        {
            ReturnColor = Color.grey;
            _image.color = ReturnColor;
        }
    }

    public void Unlock()
    {
        _button.interactable = true;
        _button.onClick.AddListener(LoadLevel);
        ReturnColor = Color.white;
        _image.color = ReturnColor;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(LevelData.Scene);
    }

    public void SetSelected(bool selected)
    {
        if (_scaleCoroutine != null)
        {
            StopCoroutine(_scaleCoroutine);
            _scaleCoroutine = null;
        }

        Vector3 target = selected ? _selectedScale : _defaultScale;
        _scaleCoroutine = StartCoroutine(SmoothScale(target, 0.12f));
    }

    private IEnumerator SmoothScale(Vector3 target, float duration)
    {
        float t = 0f;
        Vector3 start = transform.localScale;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t / duration));
            yield return null;
        }

        transform.localScale = target;
        _scaleCoroutine = null;
    }

    public bool IsInteractable()
    {
        return _button != null && _button.interactable;
    }
}
