using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuStartPanel : MonoBehaviour
{
    public Button previousBtn;
    public Button nextBtn;
    public GeneratePlayers generatePlayers;
    public TextMeshProUGUI numberOfPlayerTxt;
    public int numberOfPlayers=4;
    // Start is called before the first frame update
    void Start()
    {
        numberOfPlayers = 4;
        numberOfPlayerTxt.text = numberOfPlayers.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfPlayers == 8)
        {
            nextBtn.interactable = false;
        }
        else
        {
            nextBtn.interactable = true;
        }
        if (numberOfPlayers == 3)
        {
            previousBtn.interactable=false;
        }
        else
        {
            previousBtn.interactable=true;
        }
    }
    public void NextBtnPressed()
    {
        numberOfPlayers++;
        numberOfPlayerTxt.text = numberOfPlayers.ToString();
    }

    public void PreviousBtnPressed()
    {
        numberOfPlayers--;
        numberOfPlayerTxt.text = numberOfPlayers.ToString();
    }

    public void StartBtnPressed()
    {
        gameObject.SetActive(false);
        generatePlayers.GeneratePlayerCards(numberOfPlayers!=0 ? numberOfPlayers:4);
    }
}
