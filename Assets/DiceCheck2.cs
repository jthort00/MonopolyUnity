using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCheck2 : MonoBehaviour
{
    Vector3 velDado;


    void FixedUpdate()
    {
        velDado = Dado.velDado;
    }

    void OnTriggerStay(Collider col)
    {
        if (velDado.x == 0f && velDado.y == 0f && velDado.z == 0f)
        {
            switch (col.gameObject.name)
            {
                case "1d2":
                    NumeroDadoText2.numeroDado2 = 6;
                    break;
                case "2d2":
                    NumeroDadoText2.numeroDado2 = 5;
                    break;
                case "3d2":
                    NumeroDadoText2.numeroDado2 = 4;
                    break;
                case "4d2":
                    NumeroDadoText2.numeroDado2 = 3;
                    break;
                case "5d2":
                    NumeroDadoText2.numeroDado2 = 2;
                    break;
                case "6d2":
                    NumeroDadoText2.numeroDado2 = 1;
                    break;
            }
        }
    }
   
}


