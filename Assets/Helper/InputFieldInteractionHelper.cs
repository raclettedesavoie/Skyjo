using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldInteractionHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public TMP_InputField inputField;
    public Image inputFieldImage;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(inputField != null)
        {
            StartCoroutine(TransitionColor(Color.white, new Color(0.9f, 0.9f, 0.9f)));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inputField != null)
        {
            StartCoroutine(TransitionColor(new Color(0.9f, 0.9f, 0.9f), Color.white)) ;
        }

    }

    private IEnumerator TransitionColor(Color startColor, Color endColor)
    {
        float elapsedTime = 0f;

        while (elapsedTime < 0.4f)
        {
            inputFieldImage.color = Color.Lerp(startColor, endColor, elapsedTime / 0.4f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        inputFieldImage.color = endColor;
    }

}
