using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dado : MonoBehaviour
{
    static Rigidbody rb;
    public static Vector3 velDado;
    public Text texto;
    public Cara[] caras;
    public int NumeroActual;
    public bool rotando = false;

    // Start is called before the first frame update
    void Start()
    {
        if(rb == null){
            rb = GetComponent<Rigidbody>();
        }
        //NumeroDado();
        Destroy(this,8);
    }

    // Update is called once per frame
    void Update()
    {
       //texto.text = "Numero: " + NumeroActual;
        
    }

    /*void NumeroDado()
    {
        for (int i = 0; i < caras.Length; i++)
        {
            if (caras[i].TocaSuelo)
            {
                NumeroActual = 7 - caras [i].Numero;
            }
        }
        Invoke("NumeroDado", 0.5f);
    }*/
    /*public void LanzarDado()
    {
        float fuerzaInicial = Random.Range (1, 6);
        GetComponent <Rigidbody> ().isKinematic = false;
        GetComponent <Rigidbody> ().AddForce (new Vector3 (0, fuerzaInicial*100, 0));
        GetComponent <Rigidbody> ().rotation = Random.rotation;
    }*/
}
