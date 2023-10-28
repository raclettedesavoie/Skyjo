using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinnerPanel : MonoBehaviour
{
    public Canvas canvas;
    public Image panelWinner;
    // Start is called before the first frame update
    void Start()
    {
        canvas.sortingOrder = 998;
        panelWinner.gameObject.SetActive(false);
        panelWinner.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }

    public void AnnounceWinner()
    {
        panelWinner.gameObject.SetActive(true);
    }
}
