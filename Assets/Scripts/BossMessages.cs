using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BossMessages : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip bossAppearSound;

    [Header("Message UI")]
    public GameObject messageBubble;
    public TMP_Text bossMessageText;

    [Header("Message Settings")]
    public float messageDuration = 3f;
    public float minInterval = 8f;
    public float maxInterval = 15f;

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
            ShowRandomMessage();
            yield return new WaitForSeconds(messageDuration);

            messageBubble.SetActive(false);
        }
    }

    void ShowRandomMessage()
    {
        if (bossMessages.Length == 0) return;

        int index = Random.Range(0, bossMessages.Length);
        bossMessageText.text = bossMessages[index];
        messageBubble.SetActive(true);

        if (audioSource != null && bossAppearSound != null)
            audioSource.PlayOneShot(bossAppearSound);
    }

    public void StopMessages()
    {
        if (messageRoutine != null)
        {
            StopCoroutine(messageRoutine);
            messageBubble.SetActive(false);
        }
    }
}
