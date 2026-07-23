using System;
using UnityEngine;

public class AreaChecker : MonoBehaviour
{
    public AreaInfo areaInfo;
    public bool playerInArea;

    public bool AreaAvailable;

    public static Action<AreaChecker> checkArea;

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
