using UnityEngine;

[CreateAssetMenu(fileName = "BuyableObject", menuName = "Scriptable Objects/BuyableObject")]
public class BuyableObject : ScriptableObject
{
    public string name;
    public int pointsCost;
    public string action;
    public int objectID, commonId;
}
