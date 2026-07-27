using System;
using UnityEngine;

public class AreaChecker : MonoBehaviour
{
    public AreaInfo areaInfo;
    public bool playerInArea;

    public bool AreaAvailable;

    public SpawningLocations[] spawnPoints;

    public static Action<AreaChecker> checkArea;

    public void UpdatedAvailableSpawningPoints()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[i].SpawningLocationBecomeAvailable();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player") {
            playerInArea = true;
            checkArea?.Invoke(this);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInArea = false;
        }
    }
}
