using UnityEngine;

[CreateAssetMenu(fileName = "AreaInfo", menuName = "Scriptable Objects/AreaInfo")]
public class AreaInfo : ScriptableObject
{
    public string areaName;
    public int areaIndex;

    public int[] connectedAreasIndex;

    public SpawningLocations[] spawnPoints;
}
