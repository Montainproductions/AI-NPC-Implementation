using UnityEngine;

[CreateAssetMenu(fileName = "BuyableObject", menuName = "Scriptable Objects/BuyableObject")]
public class BuyableObject : ScriptableObject
{
    string name;
    int pointsCost;
    string action;
}
