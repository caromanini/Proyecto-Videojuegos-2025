using UnityEngine;

// Clase para todos los items. Es para manejar las funcionalidades comunes, como
// la destruccion del objeto
public abstract class ItemBase : MonoBehaviour, IItem
{
    [Header("Item Settings")]
    [SerializeField] protected float destroyDelay = 0f;

    protected bool isCollected = false;

    public void Collect()
    {
        if (isCollected) return;
        isCollected = true;

        // Para aplicar el efecto especifico del item. Las clases especificas
        // de los items lo implementan.
        ApplyEffect();

        DestroyItem();
    }

    protected abstract void ApplyEffect();

    protected virtual void DestroyItem()
    {
        Destroy(gameObject, destroyDelay);
    }
}