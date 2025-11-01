using System;
using UnityEngine;

public class GlassPiece : ItemBase
{
    [Header("Glass Piece Settings")]
    [SerializeField] private int worth = 1;

    protected override void ApplyEffect()
    {
        // Se esta ocupando patron observer para notificar usando GameEventManager
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.TriggerGlassPieceCollected(worth);
            GameEventManager.Instance.TriggerItemCollected("GlassPiece");
        }
    }
}