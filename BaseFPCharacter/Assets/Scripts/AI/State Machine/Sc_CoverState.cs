using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_CoverState : Sc_AIBaseState
{
    private GameObject self, closestCover;

    private GameObject[] allCover;

    private float closestDist;

    public override void EnterState(Sc_AIStateManager state, float speed)
    {
        closestDist = Mathf.Infinity;
        Debug.Log("Going to cover");
        state.StartCoroutine(ChoosingCover());
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {

    }

    public override void OnCollisionEnter(Sc_AIStateManager state)
    {

    }

    public void CoverStartStateInfo(GameObject selfObj, GameObject[] allCoverObjs)
    {
        self = selfObj;
        allCover = allCoverObjs;
    }

    IEnumerator ChoosingCover() {
        for (int i = 0; i >= allCover.Length; i++)
        {
            if (Vector3.Distance(allCover[i].transform.position, self.transform.position) <= closestDist)
            {
                closestDist = Vector3.Distance(allCover[i].transform.position, self.transform.position);
                closestCover = allCover[i];
            }
        }
        //Debug.Log(closeCover.Length);
        yield return null;
    }
}
