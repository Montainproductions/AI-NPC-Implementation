using UnityEngine;
using System.Collections.Generic;
using System;

public class GameMode_Match : MonoBehaviour
{
    public static GameMode_Match Instance { get; private set; }

    public List<InteractionItem> buyableObjects = new List<InteractionItem>();

    private int currentRound, typeOfMatch = 0;

    private int amountOfPlayers = 1;

    [SerializeField]
    private GameObject[] players;

    [SerializeField]
    private MatchType[] matcheTypes;
    
    public StartMatchInfo startMatchInfo;

    public static Action<GameObject[]> matchStart;

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

        BindEvent();
    }

    public void Start()
    {
        StartMatch(0);
    }

    public void BindEvent()
    {
        InteractionItem.doorOpened += RemoveAreaDoors;
    }

    public void UnbindEvent()
    {
        InteractionItem.doorOpened -= RemoveAreaDoors;
    }

    public void RemoveAreaDoors(int commonId, int areaID)
    {
        for (int i = buyableObjects.Count - 1; i >= 0; i--)
        {
            if (buyableObjects[i].scriptableObjectToInteract.commonId == commonId) { buyableObjects[i].toBeDelete = true; }

            if (buyableObjects[i].toBeDelete)
            {
                GameObject doorToRemove = buyableObjects[i].transform.gameObject;
                Destroy(doorToRemove);
                buyableObjects.RemoveAt(i);
            }
        }
    }

    public int GetCurrentRound() {  return currentRound; }

    public void IncreaseCurrentRound() {  currentRound++; }

    public void SetCurrentRound(int newRound) { currentRound = newRound; }

    public int GetAmountOfPlayers() {  return amountOfPlayers; }

    public void StartMatch(int typeOfMatchReciving)
    {
        typeOfMatch = typeOfMatchReciving;

        RoundEnemyDirector.Instance.SetTypeOfEnemies(typeOfMatch);

        amountOfPlayers = players.Length;

        currentRound = 0;

        matchStart?.Invoke(matcheTypes[0].startingWeaponPlayer);
        
    }

    private void OnDestroy()
    {
        UnbindEvent();
    }
}
