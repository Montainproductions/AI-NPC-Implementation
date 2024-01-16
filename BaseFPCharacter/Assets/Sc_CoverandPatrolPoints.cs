using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_CoverandPatrolPoints : MonoBehaviour
{
    [SerializeField]
    private GameObject[] coverPoints, patrolPoints;

    public GameObject[] ReturnCoverPoints()
    {
        return coverPoints;
    }

    public GameObject[] ReturnPatrolPoints()
    {
        return patrolPoints;
    }
}
