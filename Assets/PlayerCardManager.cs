using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCardManager : MonoBehaviour
{
    public int numberCard = 0;
    public bool canInteract = false;
    public UnityEvent OnInteracted;

    public void ChangeCard()
    {
        if (canInteract)
        {
            OnInteracted.Invoke();
        }
    }
}