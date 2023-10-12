using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Health : MonoBehaviour{
    [SerializeField]
    private bool isPlayer;

    //Health
    [SerializeField]
    [Tooltip("Current health that the character has at any point.")]
    [Range(0,1000)]
    private float currentHealth;
    
    [SerializeField]
    [Tooltip("Max health character can have at any point in time.")]
    [Range(0, 1000)]
    private float maxHealth;
    private float lastTimeHitTimer; //Timer to know when to start the self healing
    
    [SerializeField]
    [Tooltip("Can the character heal healingRate HP per second?")]
    private bool healingOverTimeAllowed;
    private bool recentlyHit; //If the character was recently hit most for the healing
    
    [SerializeField]
    [Tooltip("Rate in which the character will heal over time.")]
    [Range(0, 1000)]
    private int healingRate;

    [Header("Extra info")]
    [SerializeField]
    private bool printingValues;
    [SerializeField]
    private bool updateHealthUI;

    [SerializeField]
    private bool isHFSM;

    // Start is called before the first frame update
    void Start(){
        currentHealth = maxHealth;
        lastTimeHitTimer = 0;
        recentlyHit = false;
        //StartCoroutine(HealingOverTime());
    }

    // Update is called once per frame
    void Update(){
        if (updateHealthUI)
        {
            Sc_Basic_UI.Instance.NewHealth(currentHealth);
        }
        
            Healing();
        if (currentHealth <= 1)
        { 
            Debug.Log("No more health");
        }
    }

    //Heal over time at a certain amount of hp per second
    public void Healing(){
        //If the current hp is over the max just set it to the max
        if(currentHealth >= maxHealth){
            currentHealth = maxHealth;
            return;
        }

        //If the player was recently hit then increase timer
        if(recentlyHit && lastTimeHitTimer < 3.0f) 
        {
            lastTimeHitTimer += Time.deltaTime;
        }
        else if(recentlyHit && lastTimeHitTimer > 3.0f)
        { //Else if it was recently hit but 5 seconds have passed then they player wasnt hit recently
            lastTimeHitTimer = 0;
            recentlyHit = false;
        }else if (!recentlyHit && healingOverTimeAllowed)
        { //If it can heal then start healing
            //Debug.Log("Healing");
            lastTimeHitTimer = 0;
            currentHealth += healingRate * Time.deltaTime;
        }
    }

    //Take damage will reduce the amount of health the player currently has
    public void TakeDamage(float damage){
        currentHealth -= damage;
        recentlyHit = true;
        lastTimeHitTimer = 0.0f;

        if (!isPlayer && !isHFSM)
        {
            gameObject.GetComponent<Sc_AIStateManager>().RecentlyHit();
        }else if (isHFSM)
        {
            gameObject.GetComponent<Sc_AIStatesManagerHierarchical>().RecentlyHit();
        }

        if (updateHealthUI)
        {
            Sc_Basic_UI.Instance.NewHealth(currentHealth);
        }

        if (currentHealth <= 1) {
            if (isPlayer)
            {
                Sc_GameManager.Instance.PlayerDied(gameObject.transform.position);
            }
            Destroy(gameObject); 
        } //If no more Health then tell the game manager to go to end game
    }

    IEnumerator HealingOverTime()
    {
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
            yield return null;
        }
        if (recentlyHit)
        {
            yield return new WaitForSeconds(4);
            recentlyHit = false;
        }
        else
        {
            //Debug.Log("Healing");
            Healing();
        }
        yield return null;
    }

    public float CurrentHealthValue()
    {
        return currentHealth;
    }
    public float MaxHealthValue()
    {
        return maxHealth;
    }

    public bool ReturnRecentlyHit()
    {
        return recentlyHit;
    }
}
