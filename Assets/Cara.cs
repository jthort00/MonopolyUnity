using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cara : MonoBehaviour
{
    public int Numero;
    public bool TocaSuelo;
    // Start is called before the first frame update
    void Start()
    {
        Numero = int.Parse(GetComponent<TextMesh>().text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Suelo")
        {
            TocaSuelo = true;
        }
    }

    void OnTriggerExit(Collider col)
    {

            TocaSuelo = false;

    }
}
