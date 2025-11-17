using System.Collections;
using UnityEngine;

public class NPCVisualController : MonoBehaviour
{
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private float bubbleTime = 3f;

    void Start()
    {
        if (speechBubble != null) speechBubble.SetActive(false);
    }

    public void ShowBubble()
    {
        if (speechBubble != null)
        {
            speechBubble.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(HideBubble());
        }
    }

    private IEnumerator HideBubble()
    {
        yield return new WaitForSeconds(bubbleTime);
        if (speechBubble != null) speechBubble.SetActive(false);
    }
}