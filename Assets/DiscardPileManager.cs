using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiscardPileManager : MonoBehaviour
{
    public bool interactAfterDrawDeckPile = false;
    public UnityEvent OnInteractAfterDrawDeckPile;

    public void ChangeCard()
    {
        if (interactAfterDrawDeckPile)
        {
            OnInteractAfterDrawDeckPile.Invoke();
        }
    }
}
