using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Guardian : PopScript
{
    public override void MyBehaviour(PopScript other)
    {
        base.MyBehaviour(other);
        //GUARDIANS PROTECT DOVES FROM HARM FOR A LITTLE TIME...
        if (other is Dove)
            other.hadEncounter = true;
        //...THEY 'STEAL' FOOD ONLY FROM HAWKS...
        else if (other is Hawk)
        {
            EnergyRemaining += other.EnergyRemaining;
            other.EnergyRemaining = 0;
            //SpawnHeart();
        }
        //..AND IGNORE EACH OTHERS
        else if (other is Guardian)
        {
            hadEncounter = false;
        }
    }

    public override void SetScores()
    {
        //gatheredFood = 2; //GUARDIANS ARE AVERAGE AT GATHERING FOOD
        energySpentForReproduction = 6;
        rayDistance = 3f; //VERY GOOD SIGHT
        socialCooldown = 3f;
        GetComponent<NavMeshAgent>().speed = 4f; //LOW SPEED
        GetComponent<NavMeshAgent>().acceleration = 8f;
        timeOffset = 4f + Random.Range(0f, 1.5f);
    }

    public override void Reproduction()
    {
        EnergyRemaining -= energySpentForReproduction;
        GameObject newPop = null;
        int deadsIndex = -1;
        for (int i = 0; i < MasterScript.Instance.deads.Count; i++)
        {
            if (MasterScript.Instance.deads[i].GetComponent<PopScript>() is Guardian)
            {
                deadsIndex = i;
                break;
            }
        }
        if (deadsIndex == -1)
        {
            newPop = Instantiate(MasterScript.Instance.popPrefab, new Vector3(0f, 100f, 0f), Quaternion.identity);
            newPop.GetComponent<MeshRenderer>().material.color = Color.yellow;
            newPop.AddComponent<Guardian>();
        }
        else
        {
            newPop = MasterScript.Instance.deads[deadsIndex];
            newPop.SetActive(true);
            MasterScript.Instance.deads.Remove(newPop);
            MasterScript.Instance.Pops.Add(newPop);
        }

        newPop.GetComponent<PopScript>().Respawn();
        newPop.GetComponent<NavMeshAgent>().enabled = true;
        newPop.GetComponent<NavMeshAgent>().speed = navMeshAgent.speed;
        newPop.GetComponent<PopScript>().rayDistance = rayDistance;

    }

    public override void MyRoutine(Ray myRay)
    {
        base.MyRoutine(myRay);
        Transform myTarget = GetClosestHawk(MasterScript.Instance.Pops);
        //if (myTarget == null)
        //{
        //    GameObject newPop = null;
        //    int deadsIndex = -1;
        //    for (int i = 0; i < MasterScript.Instance.deads.Count; i++)
        //    {
        //        if (MasterScript.Instance.deads[i].GetComponent<PopScript>() is Hawk)
        //        {
        //            deadsIndex = i;
        //            break;
        //        }
        //    }
        //    if (deadsIndex == -1)
        //    {
        //        newPop = Instantiate(MasterScript.Instance.popPrefab, new Vector3(0f, 100f, 0f), Quaternion.identity);
        //        newPop.GetComponent<MeshRenderer>().material.color = Color.red;
        //        newPop.AddComponent<Hawk>();
        //    }
        //    else
        //    {
        //        newPop = MasterScript.Instance.deads[deadsIndex];
        //        newPop.SetActive(true);
        //        MasterScript.Instance.deads.Remove(newPop);
        //        MasterScript.Instance.Pops.Add(newPop);
        //    }

        //    newPop.GetComponent<PopScript>().Respawn();
        //    newPop.GetComponent<NavMeshAgent>().enabled = true;
        //    newPop.GetComponent<NavMeshAgent>().speed = navMeshAgent.speed;
        //    newPop.GetComponent<PopScript>().rayDistance = rayDistance;
        //}
        //myTarget = GetClosestHawk(MasterScript.Instance.Pops);
        if (myCurrentTarget == null && myTarget != null)
        {
            if (myTarget != null)
            {
                myCurrentTarget = myTarget.gameObject;
                navMeshAgent.destination = myTarget.position;
            }
        }
        if (myCurrentTarget != null)
        {
            navMeshAgent.destination = myTarget.position;
        }
    }

    public void SpawnHeart()
    {
        GameObject myHeart = Instantiate(MasterScript.Instance.heartPrefab, gameObject.transform);
        myHeart.transform.position = new Vector3(gameObject.transform.position.x, 1.5f, gameObject.transform.position.z);
        foreach (Transform t in myHeart.transform)
            t.gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }
}
