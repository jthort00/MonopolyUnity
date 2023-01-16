using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCheck : MonoBehaviour
{
    Vector3 velDado;
    public NumeroDadoText script;

    void FixedUpdate()
    {
        velDado = Dado.velDado;
 
    }

    void OnTriggerStay(Collider col)
    {
        Debug.Log(velDado.x);
        if (velDado.x == 0f && velDado.y == 0f && velDado.z == 0f)
        {
            switch (col.gameObject.name)
            {
                case "1d1":
                    NumeroDadoText.numeroDado = 6;
                    script.ChangeText();
                    break;
                case "2d1":
                    NumeroDadoText.numeroDado = 5;
                    script.ChangeText();
                    break;
                case "3d1":
                    NumeroDadoText.numeroDado = 4;
                    script.ChangeText();
                    break;
                case "4d1":
                    NumeroDadoText.numeroDado = 3;
                    script.ChangeText();
                    break;
                case "5d1":
                    NumeroDadoText.numeroDado = 2;
                    script.ChangeText();
                    break;
                case "6d1":
                    NumeroDadoText.numeroDado = 1;
                    script.ChangeText();
                    break;
            }
        }
    }
}
