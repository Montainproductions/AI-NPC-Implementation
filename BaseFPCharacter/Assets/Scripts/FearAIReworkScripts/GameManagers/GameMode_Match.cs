using UnityEngine;
using System.Collections.Generic;

public class GameMode_Match : MonoBehaviour
{
    public static GameMode_Match Instance { get; private set; }

    public List<InteractionItem> buyableObjects = new List<InteractionItem>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void BindEvent()
    {
        InteractionItem.doorOpened += RemoveAreaDoors;
    }

    public void UnbindEvent()
    {
        InteractionItem.doorOpened -= RemoveAreaDoors;
    }

    public void RemoveAreaDoors(int idOfAreaDoor)
    {
        for (int i = 0; i < buyableObjects.Count; i++)
        {
            if (buyableObjects[i].scriptableObjectToInteract.objectID != idOfAreaDoor) { continue; }

            GameObject doorToRemove = buyableObjects[i].transform.parent.gameObject;

            buyableObjects.RemoveAt(i);
        }
    }

}
