using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinnerText : MonoBehaviour
{
    public Canvas canvas;
    public TextMeshProUGUI textMeshProUGUI;
    // Start is called before the first frame update
    void Start()
    {
        canvas.sortingOrder = 999;
        textMeshProUGUI.gameObject.SetActive(false);
    }

    public void AnnounceWinner(string playerName, string playerPoint)
    {
        textMeshProUGUI.gameObject.SetActive(true);
        textMeshProUGUI.text = "Le gagnant est le joueur " + playerName + " avec " + playerPoint + " points.";
    }
}
