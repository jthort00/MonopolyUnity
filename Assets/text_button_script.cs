using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class text_button_script : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onClick;
    public LogicScript1 logic;
    public int clickable = 0;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (clickable ==1)
        {
        logic = GameObject.FindGameObjectWithTag("logic").GetComponent<LogicScript1>();
        logic.GetGame(this.GetComponent<Text>().text);
        onClick.Invoke();
        }

        if (clickable ==2)
        {
            logic = GameObject.FindGameObjectWithTag("logic").GetComponent<LogicScript1>();
            logic.InviteUser(this.GetComponent<Text>().text);
            onClick.Invoke();
        }
    }

    public void SetClickable(int value)
    {
        this.clickable = value;
    }


}
