using UnityEngine;

public class PlayerItemsHandInfo : MonoBehaviour
{
    private GameObject[] currentItems;
    private int currentItemsPosition;

    private int primaryUtilityAmount, secondaryUtilityAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentItemsPosition = 0;

        BindEvents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BindEvents()
    {
        InteractionSystem.pickedUpGunEvent += AquireItem;
    }

    public void UnbindEvent()
    {
        InteractionSystem.pickedUpGunEvent -= AquireItem;
    }

    public void AquireItem(GameObject weaponBeingPickedUp)
    {
        currentItems[currentItemsPosition] = weaponBeingPickedUp;
        
    }

    public void DropItem()
    {
        currentItems[currentItemsPosition] = null;
    }

    public void ChangeHandItem()
    {
        currentItemsPosition++;
        if (currentItemsPosition >= 1)
        {
            currentItemsPosition = 0;
        }
    }

    public void SetItemActive(int newWeaponPosition)
    {

    }

    public void PrimaryAction()
    {
        if (currentItems[currentItemsPosition].GetComponent<Gun>())
        {
            currentItems[currentItemsPosition].GetComponent<Gun>().PrimaryAction();
        }
    }

    public void SecondaryAction()
    {

    }

    public void ReloadAction()
    {

    }

    public void InspectWeapon()
    {

    }
}
