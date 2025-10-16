using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BossMessages : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] bossAppearSounds;

    [Header("Message UI")]
    public GameObject messageBubble;
    public TMP_Text bossMessageText;

    // --- NUEVAS REFERENCIAS Y CONFIGURACIÓN DE ANIMACIÓN ---
    [Header("Boss Visuals")]
    public RectTransform angryBossImage; // La imagen del jefe (debe ser un RectTransform)
    public float entryDuration = 0.5f;   // Duración de la animación de entrada
    public float pulseSpeed = 1f;        // Velocidad de la pulsación
    public float pulseMagnitude = 0.05f; // Magnitud de la pulsación (0.05 = 5% de escala)

    // Posiciones calculadas en Start()
    private Vector2 offScreenPosition;
    private Vector2 onScreenBossPosition;
    private Vector2 onScreenBubblePosition;

    // Coroutine para el pulso
    private Coroutine pulseRoutine;


    [Header("Message Settings")]
    public float messageDuration = 3f;
    public float minInterval = 3f;
    public float maxInterval = 5f;

    [Header("Messages Pool")]
    [TextArea(2, 5)]
    public string[] bossMessages = {
        "¡Apúrate, no queda tiempo!",
        "¡Estamos esperando!",
        "¿Dónde dejaste los lentes?",
        "¡Muévete más rápido!"
    };

    private GameController gameController;
    private Coroutine messageRoutine;
    private bool hasStartedMessages = false;

    void Start()
    {
        gameController = FindAnyObjectByType<GameController>();

        // 1. Configurar posiciones de la UI
        if (angryBossImage != null && messageBubble != null)
        {
            // Posición en pantalla (se asume que ya están en la posición deseada en el Editor)
            onScreenBossPosition = angryBossImage.anchoredPosition;
            onScreenBubblePosition = messageBubble.GetComponent<RectTransform>().anchoredPosition;

            // Posición fuera de pantalla (Abajo)
            // Se asume que el pivote Y=0 está en el borde inferior. 
            offScreenPosition = onScreenBossPosition;
            offScreenPosition.y -= 500f; // Mueve 500 unidades hacia abajo fuera de la vista.

            // Mover ambos elementos fuera de la pantalla al inicio
            angryBossImage.anchoredPosition = offScreenPosition;
            messageBubble.GetComponent<RectTransform>().anchoredPosition = offScreenPosition;
        }

        // 2. Ocultar la burbuja (la imagen del jefe debe estar fuera de pantalla)
        messageBubble.SetActive(false);
    }

    void Update()
    {
        if (gameController == null) return;

        float elapsed = gameController.TotalTime - gameController.TimeRemaining;
        float fraction = elapsed / gameController.TotalTime;

        if (!hasStartedMessages && fraction >= 1f / 3f)
        {
            hasStartedMessages = true;
            messageRoutine = StartCoroutine(MessageLoop());
        }
    }

    IEnumerator MessageLoop()
    {
        while (gameController != null && gameController.TimeRemaining > 0)
        {
            float urgency = 1f - (gameController.TimeRemaining / gameController.TotalTime);
            float interval = Mathf.Lerp(maxInterval, minInterval, urgency);

            yield return new WaitForSeconds(interval);

            // 1. Mostrar, animar entrada, y pulsar
            StartCoroutine(AnimateEntryAndShowMessage());

            // Esperar la duración del mensaje + duración de la entrada
            yield return new WaitForSeconds(messageDuration + entryDuration);

            // 2. Ocultar y animar salida
            StartCoroutine(AnimateExitAndHide());
        }
    }

    // =======================================================
    // NUEVOS MÉTODOS DE ANIMACIÓN
    // =======================================================

    IEnumerator AnimateEntryAndShowMessage()
    {
        ShowRandomMessageTextAndSound();

        // Muestra la imagen y la burbuja (inician desde fuera de pantalla)
        if (angryBossImage != null) angryBossImage.gameObject.SetActive(true);
        messageBubble.SetActive(true);

        RectTransform bubbleRect = messageBubble.GetComponent<RectTransform>();

        float startTime = Time.time;
        float endTime = startTime + entryDuration;

        // Mueve ambos elementos de offScreenPosition a onScreenPosition
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / entryDuration;

            // Movimiento suave (Lerp)
            angryBossImage.anchoredPosition = Vector2.Lerp(offScreenPosition, onScreenBossPosition, t);
            bubbleRect.anchoredPosition = Vector2.Lerp(offScreenPosition, onScreenBubblePosition, t);

            yield return null;
        }

        // Asegura que lleguen a la posición final
        angryBossImage.anchoredPosition = onScreenBossPosition;
        bubbleRect.anchoredPosition = onScreenBubblePosition;

        // Iniciar la pulsación después de que la entrada ha terminado
        pulseRoutine = StartCoroutine(PulseAnimation(bubbleRect));
    }

    IEnumerator AnimateExitAndHide()
    {
        // Detener el pulso
        if (pulseRoutine != null) StopCoroutine(pulseRoutine);

        RectTransform bubbleRect = messageBubble.GetComponent<RectTransform>();

        float startTime = Time.time;
        float endTime = startTime + entryDuration;

        // Mueve ambos elementos de onScreenPosition a offScreenPosition
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / entryDuration;

            // Movimiento inverso
            angryBossImage.anchoredPosition = Vector2.Lerp(onScreenBossPosition, offScreenPosition, t);
            bubbleRect.anchoredPosition = Vector2.Lerp(onScreenBubblePosition, offScreenPosition, t);

            yield return null;
        }

        // Asegura que queden fuera y los desactiva
        if (angryBossImage != null) angryBossImage.gameObject.SetActive(false);
        messageBubble.SetActive(false);
    }

    IEnumerator PulseAnimation(RectTransform bubbleRect)
    {
        Vector3 originalScale = bubbleRect.localScale;

        while (true)
        {
            float scaleFactor = 1f + (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) * pulseMagnitude);

            // Aplicar la pulsación a la burbuja y al texto simultáneamente
            bubbleRect.localScale = originalScale * scaleFactor;
            bossMessageText.transform.localScale = originalScale * scaleFactor; // Pulso en el texto

            yield return null;
        }
    }

    // =======================================================
    // LÓGICA DE MENSAJE ORIGINAL (RENOMBRADA)
    // =======================================================

    void ShowRandomMessageTextAndSound()
    {
        if (bossMessages.Length == 0) return;

        // Lógica de mensaje de texto
        int textIndex = Random.Range(0, bossMessages.Length);
        bossMessageText.text = bossMessages[textIndex];

        // Lógica de audio
        if (audioSource != null && bossAppearSounds.Length > 0)
        {
            int audioIndex = Random.Range(0, bossAppearSounds.Length);
            AudioClip clipToPlay = bossAppearSounds[audioIndex];

            if (clipToPlay != null)
            {
                audioSource.PlayOneShot(clipToPlay);
            }
        }
    }

    public void StopMessages()
    {
        if (messageRoutine != null)
        {
            StopCoroutine(messageRoutine);
            // Asegúrate de detener también el pulso y ocultar
            if (pulseRoutine != null) StopCoroutine(pulseRoutine);
            if (angryBossImage != null) angryBossImage.gameObject.SetActive(false);
            messageBubble.SetActive(false);
        }
    }
}