using UnityEngine;

// Es una clase base para todos los tipos distintos de power ups que hay
// Almohada, Energy Drink
public abstract class PowerUpBase : ItemBase
{
    protected override void ApplyEffect()
    {
        // Aplicar el efecto específico del power up
        ApplyPowerUpEffect();

        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.TriggerItemCollected(GetType().Name);
        }
    }

    // Metodo que cada power up tiene que implementar
    protected abstract void ApplyPowerUpEffect();

    protected T FindPlayerComponent<T>() where T : Component
    {
        T component = FindAnyObjectByType<T>();

        if (component == null)
        {
            Debug.LogWarning($"{GetType().Name}: No se encontró {typeof(T).Name} en la escena.");
        }

        return component;
    }
}