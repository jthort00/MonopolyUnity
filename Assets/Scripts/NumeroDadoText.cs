using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumeroDadoText : MonoBehaviour
{
	Text text;
	public static int numeroDado;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		text.text = "Número: " + numeroDado.ToString ();
	}
}
