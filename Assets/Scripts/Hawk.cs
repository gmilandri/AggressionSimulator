using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hawk : PopScript
{
    public override void MyBehaviour(PopScript other)
    {
        base.MyBehaviour(other);
        //HAWKS WASTE EACH OTHER FOOS
        if (other is Hawk)
        {
            //other.Die();
            //Die();
            EnergyRemaining = 0;
            other.EnergyRemaining = 0;
        }
        //...BUT THEY STEAL ALL THE FOOD FROM DOVES...
        else if (other is Dove)
        {
            EnergyRemaining += other.EnergyRemaining;
            other.EnergyRemaining = 0;
            //SpawnHeart();
        }
        //...AND THEY HAVE A GOOD CHANCE TO KILL A GUARDIAN.
        else if (other is Guardian)
        {
            int randomChance = Random.Range(0, 100);
            if (randomChance < 75)
                other.Die();

        }
    }

    public override void SetScores()
    {
        //gatheredFood = 1; //HAWKS ARE ONLY GOOD AT STEALING AND KILLING.
        energySpentForReproduction = 4; //HAWKS REPRODUCE QUICKLY
        rayDistance = 5f; //GOOD SIGHT
        socialCooldown = 1f;
        GetComponent<NavMeshAgent>().speed = 5f; //HIGH SPEED
        GetComponent<NavMeshAgent>().acceleration = 5f;
        timeOffset = 1.5f + Random.Range(0f, 1.5f);
    }

    public override void Reproduction()
    {
        EnergyRemaining -= energySpentForReproduction;
        GameObject newPop;
        int deadsIndex = -1;
        for (int i = 0; i < MasterScript.Instance.deads.Count; i++)
        {
            if (MasterScript.Instance.deads[i].GetComponent<PopScript>() is Hawk)
            {
                deadsIndex = i;
                break;
            }
        }
        if (deadsIndex == -1)
        {
            newPop = Instantiate(MasterScript.Instance.popPrefab, new Vector3(0f, 100f, 0f), Quaternion.identity);
            newPop.GetComponent<MeshRenderer>().material.color = Color.red;
            newPop.AddComponent<Hawk>();
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
        Transform myTarget = GetClosestDove(MasterScript.Instance.Pops);
        if (myCurrentTarget == null)
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
            t.gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
    }

}