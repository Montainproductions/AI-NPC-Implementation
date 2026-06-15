using UnityEngine;

public class CharacterGeneral : MonoBehaviour
{
    [SerializeField]
    private CharacterData characterData;

    public CharacterData GetCharacterData()
    {
        return characterData;
    }
}
