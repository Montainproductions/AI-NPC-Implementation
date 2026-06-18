using UnityEngine;

public class StartRangeLane : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Item entered");
        gameObject.GetComponent<GunRangeDirector>().StartGunRange();
    }

    public void OnCollisionExit(Collision collision)
    {
        gameObject.GetComponent<GunRangeDirector>().EndGunRange();
    }
}
