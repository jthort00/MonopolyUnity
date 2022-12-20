using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCheck : MonoBehaviour
{
    Vector3 velDado;

    void FixedUpdate()
    {
        velDado = Dado.velDado;
    }

    void OnTriggerStay(Collider col){
        if(velDado.x == 0f && velDado.y == 0f && velDado.z == 0f){
            switch(col.gameObject.name){
                case "1":
                    NumeroDadoText.numeroDado = 6;
                    break;
                case "2":
                NumeroDadoText.numeroDado = 5;
                    break;
                case "3":
                NumeroDadoText.numeroDado = 4;
                    break;
                case "4":
                NumeroDadoText.numeroDado = 3;
                    break;
                case "5":
                NumeroDadoText.numeroDado = 2;
                    break;
                case "6":
                NumeroDadoText.numeroDado = 1;
                    break;
            }
        }
    }
}
