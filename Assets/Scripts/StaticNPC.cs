using UnityEngine;
using System.Collections;

public class StaticNPC : MonoBehaviour
{
    [Header("Speech Bubble")]
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private float bubbleDisplayTime = 3f;

    private bool hasCollidedThisFrame = false;

    void Start()
    {      
        if (speechBubble != null)
        {
            speechBubble.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasCollidedThisFrame)
        {
            hasCollidedThisFrame = true;
            ShowBubble();
            Invoke(nameof(ResetCollision), 0.5f); // cooldown para que no haya mas de uno
        }
    }

    private void ShowBubble()
    {
        if (speechBubble != null)
        {
            speechBubble.SetActive(true);
            StartCoroutine(HideBubbleAfterDelay());
        }
    }

    private IEnumerator HideBubbleAfterDelay()
    {
        yield return new WaitForSeconds(bubbleDisplayTime);

        if (speechBubble != null)
        {
            speechBubble.SetActive(false);
        }
    }

    private void ResetCollision()
    {
        hasCollidedThisFrame = false;
    }
}