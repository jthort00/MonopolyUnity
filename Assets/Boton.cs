using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boton : MonoBehaviour
{

    public Button boton;
    public GameObject dado1;
    public GameObject dado2;
    private int fuerza;
    private Vector3 spawn1;
    private Vector3 spawn2;
    private GameObject d1;
    private GameObject d2;


    void Start()
    {
        spawn1 = new Vector3(-2, 5, -100);
        spawn2 = new Vector3(2, 5, -100);
        Button btn = boton.GetComponent<Button>();
        btn.onClick.AddListener(TirarDados);
    }

    void TirarDados()
    {
        d1 = Instantiate(dado1, spawn1, Quaternion.identity);
        d2 = Instantiate(dado2, spawn2, Quaternion.identity);
        fuerza = Random.Range(1, 6);
        d1.GetComponent<Rigidbody>().AddForce(new Vector3(0, fuerza * 100, 0));
        d1.GetComponent<Rigidbody>().rotation = Random.rotation;
        d2.GetComponent<Rigidbody>().AddForce(new Vector3(0, fuerza * 100, 0));
        d2.GetComponent<Rigidbody>().rotation = Random.rotation;
        StartCoroutine(DestruirDados());

    }

    IEnumerator DestruirDados()
    {
        yield return new WaitForSeconds(7f);
        DestroyImmediate(d1.gameObject, true);
        DestroyImmediate(d2.gameObject, true);
    }



}

