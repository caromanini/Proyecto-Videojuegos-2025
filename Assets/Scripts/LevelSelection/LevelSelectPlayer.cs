using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnityEngine.RectTransform))]
public class LevelSelectPlayer : MonoBehaviour
{
    [Tooltip("Offset (en unidades UI) desde el centro del botón para posicionar el player (ej.: encima)")]
    public Vector2 offset = new Vector2(0, 50);
    [Tooltip("Duración del movimiento (segundos)")]
    public float moveDuration = 0.16f;

    private UnityEngine.RectTransform _rectTransform;
    private Canvas _rootCanvas;
    private Coroutine _moveCoroutine;

    private void Awake()
    {
        _rectTransform = GetComponent<UnityEngine.RectTransform>();
        _rootCanvas = GetComponentInParent<Canvas>();
    }

    // Mueve suavemente al botón (usa la cámara del canvas si existe)
    public void MoveToButton(UnityEngine.RectTransform buttonRect)
    {
        if (buttonRect == null) return;
        Vector2 target = WorldToLocalAnchoredPosition(buttonRect) + offset;

        // Determina dirección del movimiento y ajusta el flip antes de empezar a moverse
        float deltaX = target.x - _rectTransform.anchoredPosition.x;
        SetFacingByDirection(deltaX);

        StartMove(target);
    }

    // Posiciona inmediatamente en el botón (sin animación)
    public void SnapToButton(UnityEngine.RectTransform buttonRect)
    {
        if (buttonRect == null) return;
        if (_moveCoroutine != null) { StopCoroutine(_moveCoroutine); _moveCoroutine = null; }
        Vector2 target = WorldToLocalAnchoredPosition(buttonRect) + offset;
        // Ajusta el flip en función de la nueva posición objetivo
        SetFacingByDirection(target.x - _rectTransform.anchoredPosition.x);
        _rectTransform.anchoredPosition = target;
    }

    // Convierte la posición world del botón a anchoredPosition relativa al padre del player
    private Vector2 WorldToLocalAnchoredPosition(UnityEngine.RectTransform buttonRect)
    {
        RectTransform parentRT = _rectTransform.parent as UnityEngine.RectTransform;
        if (parentRT == null) return Vector2.zero;

        Vector2 screenPoint;
        Camera cam = (_rootCanvas != null && _rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? (_rootCanvas.worldCamera ?? Camera.main) : Camera.main;
        screenPoint = cam.WorldToScreenPoint(buttonRect.position);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRT, screenPoint, cam, out localPoint);
        return localPoint;
    }

    private void StartMove(Vector2 target)
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(MoveRoutine(target, moveDuration));
    }

    private IEnumerator MoveRoutine(Vector2 target, float duration)
    {
        Vector2 start = _rectTransform.anchoredPosition;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            _rectTransform.anchoredPosition = Vector2.Lerp(start, target, Mathf.Clamp01(t / duration));
            yield return null;
        }
        _rectTransform.anchoredPosition = target;
        _moveCoroutine = null;
    }

    // Establece la escala X para "mirar" a la derecha o a la izquierda en función de la dirección del movimiento.
    // Sólo invierte la escala si la dirección y el signo actual difieren.
    private void SetFacingByDirection(float deltaX)
    {
        const float eps = 0.001f;
        if (Mathf.Abs(deltaX) < eps) return; // sin cambio significativo, no tocar

        Vector3 s = _rectTransform.localScale;
        float currentSign = Mathf.Sign(s.x);

        if (deltaX < 0f && currentSign > 0f)
        {
            // se mueve a la izquierda y actualmente mira a la derecha -> girar a la izquierda
            s.x = -Mathf.Abs(s.x);
            _rectTransform.localScale = s;
        }
        else if (deltaX > 0f && currentSign < 0f)
        {
            // se mueve a la derecha y actualmente mira a la izquierda -> girar a la derecha
            s.x = Mathf.Abs(s.x);
            _rectTransform.localScale = s;
        }
    }
}