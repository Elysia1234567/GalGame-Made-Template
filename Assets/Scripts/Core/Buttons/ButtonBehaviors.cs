using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBehaviors : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private static ButtonBehaviors selectedButton = null;
    public Animator anim;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(selectedButton != null&& selectedButton!=this)
        {
            selectedButton.OnPointerExit(null);
        }
        anim.Play("Enter");
        selectedButton = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.Play("Exit");
    }

   
}
