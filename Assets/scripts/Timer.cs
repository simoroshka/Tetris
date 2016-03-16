using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//displays time since starting the game. 
public class Timer : MonoBehaviour
{
    private Text timeText;
    private float startTime;

    private bool running = false;

    // Use this for initialization
    void Start()
    {
        timeText = GetComponent<Text>();       
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            float timePassed = Time.time - startTime;
            int minutes = (int)(timePassed / 60);
            int seconds = (int)(timePassed % 60);

            //display the timer
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }               
    }

    public void stop()
    {
        running = false;
    }
    public void run()
    {
        running = true;
        startTime = Time.time;
    }

}