using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumeroDadoText : MonoBehaviour
{
	public Text text;
	public static int numeroDado;
	public static int update_num;
	public PlMove movimiento;


	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	public void ChangeText () {
		text.text = "Número: " + numeroDado.ToString ();
	
		//movimiento.PositionCalculations();
		
	}




}
