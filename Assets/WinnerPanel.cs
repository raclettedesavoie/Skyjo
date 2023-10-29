using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinnerPanel : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);

    }

    public void AnnounceWinner(string winnerText )
    {
        gameObject.SetActive(true);
        GetComponentInChildren<TextMeshProUGUI>().text = winnerText;
    }
}
