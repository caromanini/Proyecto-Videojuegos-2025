using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Collector : MonoBehaviour
{
    public int glassesCount = 0;

    [Header("Shader Settings")]
    public Material pixelatedMaterial;

    public float CurrentPixelSize = 50f;

    private readonly Dictionary<int, float> pixelSizeMap = new Dictionary<int, float>()
    {
        { 0, 50f },
        { 1, 120f },
        { 2, 240f },
        { 3, 360f },
        { 4, 512f }
    };

    private const string PixelSizePropertyName = "_PixelSize";
    private Coroutine tempBoostCoroutine = null;

    void Start()
    {
        UpdatePixelSize(glassesCount);
        SubscribeToEvents();
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnGlassPieceCollected += AddGlasses;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnGlassPieceCollected -= AddGlasses;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IItem item = collision.GetComponent<IItem>();
        if (item != null)
        {
            item.Collect();
        }
    }

    public void AddGlasses(int amount)
    {
        Debug.Log($"[Collector] Adding glasses: {amount}");
        glassesCount += amount;
        UpdatePixelSize(glassesCount);
    }

    public void ApplyTemporaryFullGlasses(float duration)
    {
        if (tempBoostCoroutine != null)
        {
            StopCoroutine(tempBoostCoroutine);
            tempBoostCoroutine = null;
        }

        int previousCount = glassesCount;
        glassesCount = 5;
        UpdatePixelSize(glassesCount);

        tempBoostCoroutine = StartCoroutine(TemporaryFullGlassesCoroutine(duration, previousCount));
    }

    private IEnumerator TemporaryFullGlassesCoroutine(float duration, int previousCount)
    {
        yield return new WaitForSeconds(duration);
        glassesCount = previousCount;
        UpdatePixelSize(glassesCount);
        tempBoostCoroutine = null;
    }

    private void UpdatePixelSize(int count)
    {
        if (pixelatedMaterial == null)
        {
            return;
        }

        if (pixelSizeMap.TryGetValue(count, out float newPixelSize))
        {
            CurrentPixelSize = newPixelSize;
            pixelatedMaterial.SetFloat(PixelSizePropertyName, newPixelSize);
        }
        else if (count > 4)
        {
            pixelatedMaterial.SetFloat(PixelSizePropertyName, pixelSizeMap[4]);
        }
    }
}