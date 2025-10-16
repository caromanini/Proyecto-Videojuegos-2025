using UnityEngine;
using System.Collections.Generic;

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

    private void AddGlasses(int amount)
    {
        glassesCount += amount;

        UpdatePixelSize(glassesCount);
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