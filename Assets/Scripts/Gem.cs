using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour, IItem
{
    public static event Action<int> OnGemCollect;
    public int worth = 1;

    bool collected = false;

    public void Collect()
    {
        if (collected) return;
        collected = true;

        OnGemCollect?.Invoke(worth);

        Destroy(gameObject);
    }

}
