using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordSearch
{
    public class Timer
    {
        private float startTime; // Total duration of the timer
        private float remainingTime; // Remaining time to count down from
        private bool isRunning = false; // Track if the timer is running

        // Starts the timer with a fixed duration
        public void StartCounting(float time)
        {
            startTime = time; // Set the fixed starting time
            remainingTime = time; // Initialize remaining time
            isRunning = true; // Start the timer
        }

        // Updates the remaining time (to be called in Update method)
        public void UpdateTimer()
        {
            if (isRunning)
            {
                remainingTime -= Time.deltaTime; // Reduce time by the elapsed frame time

                if (remainingTime <= 0)
                {
                    remainingTime = 0;
                    isRunning = false; // Stop the timer when it hits 0
                }
            }
        }
        public void AddPoints()
        {
            remainingTime += 2;
        }
        // Returns the remaining time
        public float RemainingTime()
        {
            return remainingTime;
        }

        // Checks if the timer has finished
        public bool IsFinished()
        {
            return remainingTime <= 0;
        }

        // Optionally: Stop the timer manually
        public void StopTimer()
        {
            isRunning = false;
        }

        // Optionally: Reset the timer
        public void ResetTimer(float time)
        {
            startTime = time;
            remainingTime = time;
            isRunning = true;
        }
    }
}
