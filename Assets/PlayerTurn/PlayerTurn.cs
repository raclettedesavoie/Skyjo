using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerTurn : MonoBehaviour
{
    public Animator animator;

    public IEnumerator ActivatePlayerTurn()
    {
        animator.SetBool("ActivateAnimation", true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.SetBool("ActivateAnimation", false);
    }
}
