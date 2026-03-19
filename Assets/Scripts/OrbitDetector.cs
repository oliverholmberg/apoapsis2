using UnityEngine;

public class OrbitDetector : MonoBehaviour
{
    Moon trackedMoon;
    bool[] quadrantsVisited = new bool[4];
    int lastQuadrant = -1;

    public System.Action<Moon> OnOrbitComplete;

    void FixedUpdate()
    {
        Moon currentMoon = FindCurrentSOI();

        if (currentMoon == null)
        {
            if (trackedMoon != null)
                ResetTracking();
            return;
        }

        // Skip completed moons
        if (currentMoon.Completed) return;

        if (currentMoon != trackedMoon)
        {
            ResetTracking();
            trackedMoon = currentMoon;
        }

        Vector2 offset = (Vector2)transform.position - (Vector2)currentMoon.transform.position;
        int quadrant;
        if (offset.x >= 0 && offset.y >= 0) quadrant = 0;
        else if (offset.x < 0 && offset.y >= 0) quadrant = 1;
        else if (offset.x < 0 && offset.y < 0) quadrant = 2;
        else quadrant = 3;

        if (quadrant != lastQuadrant)
        {
            if (lastQuadrant == -1)
            {
                quadrantsVisited[quadrant] = true;
            }
            else
            {
                int fwd = (lastQuadrant + 1) % 4;
                int bwd = (lastQuadrant + 3) % 4;
                if (quadrant == fwd || quadrant == bwd)
                {
                    quadrantsVisited[quadrant] = true;
                }
                else
                {
                    ResetTracking();
                    trackedMoon = currentMoon;
                    quadrantsVisited[quadrant] = true;
                }
            }

            lastQuadrant = quadrant;

            if (quadrantsVisited[0] && quadrantsVisited[1] && quadrantsVisited[2] && quadrantsVisited[3])
            {
                OnOrbitComplete?.Invoke(currentMoon);
                ResetTracking();
            }
        }
    }

    Moon FindCurrentSOI()
    {
        Moon[] moons = Object.FindObjectsByType<Moon>(FindObjectsSortMode.None);
        foreach (Moon moon in moons)
        {
            float dist = Vector2.Distance(transform.position, moon.transform.position);
            if (dist < moon.soiRadius)
                return moon;
        }
        return null;
    }

    void ResetTracking()
    {
        trackedMoon = null;
        lastQuadrant = -1;
        for (int i = 0; i < 4; i++) quadrantsVisited[i] = false;
    }
}
