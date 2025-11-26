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
        { 1, 80f },
        { 2, 110f },
        { 3, 200f },
        { 4, 250f }
    };

    private const string PixelSizePropertyName = "_PixelSize";
    private Coroutine tempBoostCoroutine = null;
    private bool isEyeDropsActive = false;

    private PlayerStateController stateController;

    void Start()
    {
        stateController = GetComponent<PlayerStateController>();

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
        glassesCount += amount;

        if (!isEyeDropsActive)
        {
            UpdatePixelSize(glassesCount);
        }
    }

    public void ApplyTemporaryFullGlasses(float duration)
    {
        if (tempBoostCoroutine != null)
        {
            StopCoroutine(tempBoostCoroutine);
        }

        tempBoostCoroutine = StartCoroutine(TemporaryFullGlassesCoroutine(duration));
    }

    private IEnumerator TemporaryFullGlassesCoroutine(float duration)
    {
        isEyeDropsActive = true;
        UpdatePixelSize(5);

        if (stateController != null) stateController.SetEyeDropsActive(true);

        yield return new WaitForSeconds(duration);

        if (stateController != null) stateController.SetEyeDropsActive(false);

        isEyeDropsActive = false;
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