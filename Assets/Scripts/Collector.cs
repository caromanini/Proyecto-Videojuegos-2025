using UnityEngine;
using System.Collections.Generic;
using System.Collections; 

public class Collector : MonoBehaviour
{
    public int glassesCount = 0;

    [Header("Shader Settings")]
    public Material pixelatedMaterial;

    //SOY EL VICHO. Esta linea es para debuggear si cambia el pixelsize
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

    // --- temporizador / control de boost temporal ---
    private Coroutine tempBoostCoroutine = null;

    void Start()
    {
        UpdatePixelSize(glassesCount);
    }

    private void OnEnable()
    {
        Gem.OnGemCollect += AddGlasses;
    }

    private void OnDisable()
    {
        Gem.OnGemCollect -= AddGlasses;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IItem item = collision.GetComponent<IItem>();
        if (item != null)
        {
            item.Collect();
        }
    }

    // ahora en público para que otros items como los eye drops lo puedan llamar
    public void AddGlasses(int amount)
    {
        glassesCount += amount;

        UpdatePixelSize(glassesCount);
    }

    // Aplica temporalmente la "visión completa" durante 5 segundos.
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

        // Restaurar al conteo previo de lentes
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
            // 1. Actualiza la variable pública (para el Inspector)
            CurrentPixelSize = newPixelSize;
            pixelatedMaterial.SetFloat(PixelSizePropertyName, newPixelSize);
        }
        else if (count > 4)
        {
            pixelatedMaterial.SetFloat(PixelSizePropertyName, pixelSizeMap[4]);
        }
    }
}