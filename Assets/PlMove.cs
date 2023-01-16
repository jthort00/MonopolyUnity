using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlMove : MonoBehaviour
{
    // Array of waypoints to move the object along
    public Transform[] waypoints;

    // The current index of the waypoint the object is moving towards
    private int currentWaypointIndex = 0;

    // Speed at which the object moves
    public float movementSpeed = 7f;

    // The result of rolling the first dice
    public int diceRoll1 = 0;

    // The result of rolling the second dice
    public int diceRoll2 = 0;

    public int CurrentPosition = 0;


    void Start()
    {
        // Set the object's initial position to the first waypoint
        transform.position = waypoints[currentWaypointIndex].position;
    }

    public void PositionCalculations(int number)
    {
        //diceRoll1 = NumeroDadoText.numeroDado;
        //diceRoll2 = NumeroDadoText2.numeroDado2;
        //int dices = diceRoll1 + diceRoll2;
        //int random = Random.Range(1, 12);
        CurrentPosition = number;
        Debug.Log("A CALCULATION HAS BEEN DONE " + CurrentPosition);

        if (CurrentPosition >= 40)
        {
            CurrentPosition = CurrentPosition - 40;
        }

    }

    void Update()
    {

        // Calculate the number of waypoints to move based on the dice rolls

        transform.position = Vector3.MoveTowards(transform.position, waypoints[CurrentPosition].position, movementSpeed * Time.deltaTime);

    }

    
}
    