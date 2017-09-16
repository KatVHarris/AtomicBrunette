using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour {
    //Stores the GameObjects of the clocks.
    public GameObject Clock1, Clock2, Clock3;

    //It sets a new time and date for a clock.
    public IEnumerator SetTime()
    {
        yield return new WaitForSeconds(2); //It waits two seconds before changing the date and time (for demonstration purposes).
        //Adjusts the time of the clocks.
        Clock1.GetComponent<Clock>().SetNewDateTime(2015, 5, 1, 0, 0, 0); //Year, Month, Day, Hour, Minute, Second.
        Clock2.GetComponent<Clock>().SetNewDateTime(2015, 5, 1, 0, 0, 0);
        Clock3.GetComponent<Clock>().SetNewDateTime(2015, 5, 1, 15, 35, 55);
        //Set a new time for the alarm.
        Clock3.GetComponent<Clock>().AlarmActive = false; //Disables the alarm of clock3.
        Clock3.GetComponent<Clock>().AlarmHour = Clock3.GetComponent<Clock>().CustomClock.Hour; //It sets the alarm time to the current time (in case of a custom datetime).
        Clock3.GetComponent<Clock>().AlarmMin = Clock3.GetComponent<Clock>().CustomClock.Minute + 1; //Arrow the alarm minute for one minute after the current minute.
        yield return new WaitForSeconds(1); //Wait a second.
        Clock3.GetComponent<Clock>().AlarmActive = true; //Reactivate the alarm.
        //After 8 seconds, deactivates the alarm.
        yield return new WaitForSeconds(8);
        Clock3.GetComponent<Clock>().AlarmActive = false;
        //It waits another 2 seconds and changes the color of the watch.
        yield return new WaitForSeconds(2);
        Clock3.GetComponent<Clock>().NewColor = Color.red; //Define a new Color.
        Clock3.GetComponent<Clock>().SetNewColor(); //Call the function to set the new color.
        //Set a new Speed of clock 3
        Clock3.GetComponent<Clock>().speed = 6.0f;
    }

    // Use this for initialization
    void Start () {
        StartCoroutine(SetTime());
    }
}
