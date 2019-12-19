using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dove : PopScript
{
    public override void MyBehaviour(PopScript other)
    {
        base.MyBehaviour(other);

        //DOVES GIFT FOOD TO EVERYONE BUT SACRIFICES THEMSELVES TO GUARDIANS

        if (other is Guardian)
        {
            print("I gave food to a Guardian.");
            other.EnergyRemaining += EnergyRemaining;
            EnergyRemaining = 0;
        }
        else
        {
            other.EnergyRemaining += EnergyRemaining / 2;
            EnergyRemaining /= 2;
        }

        //SpawnHeart();
    }

    public void SpawnHeart()
    {
        GameObject myHeart = Instantiate(MasterScript.Instance.heartPrefab, gameObject.transform);
        myHeart.transform.position = new Vector3(gameObject.transform.position.x, 1.5f, gameObject.transform.position.z);
    }

    public override void SetScores()
    {
        gatheredFood = 3; //DOVES ARE GOOD AT GATHERING FOOD
        energySpentForReproduction = 5; //AVERAGE ENERGY SPENT TO REPRODUCE
        rayDistance = 4f;//2f; //AVERAGE SIGHT
        socialCooldown = 5f;
        GetComponent<NavMeshAgent>().speed = 4f; //AVERAGE SPEED
        GetComponent<NavMeshAgent>().acceleration = 4f;
        timeOffset = 2f + Random.Range(0f, 1.5f);
    }

    public override void Reproduction()
    {
        EnergyRemaining -= energySpentForReproduction;
        GameObject newPop;
        int deadsIndex = -1;
        for (int i = 0; i < MasterScript.Instance.deads.Count; i++)
        {
            if (MasterScript.Instance.deads[i].GetComponent<PopScript>() is Dove)
            {
                deadsIndex = i;
                break;
            }
        }
        if (deadsIndex == -1)
        {
            newPop = Instantiate(MasterScript.Instance.popPrefab, new Vector3(0f, 100f, 0f), Quaternion.identity);
            newPop.GetComponent<MeshRenderer>().material.color = Color.cyan;
            newPop.AddComponent<Dove>();
        }
        else
        {
            newPop = MasterScript.Instance.deads[deadsIndex].gameObject;
            newPop.SetActive(true);
            MasterScript.Instance.deads.Remove(newPop.GetComponent<PopScript>());
            MasterScript.Instance.Pops.Add(newPop.GetComponent<PopScript>());
        }

        newPop.GetComponent<PopScript>().Respawn();
        newPop.GetComponent<NavMeshAgent>().enabled = true;
        newPop.GetComponent<NavMeshAgent>().speed = navMeshAgent.speed;
        newPop.GetComponent<PopScript>().rayDistance = rayDistance;

    }

    public override void MyRoutine(Ray myRay)
    {
        base.MyRoutine(myRay);
        int guardianCount = 0;
        foreach (PopScript t in MasterScript.Instance.Pops)
        {
            if (t is Guardian)
                guardianCount++;
        }
        if (guardianCount == 0)
        {
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
                newPop = MasterScript.Instance.deads[deadsIndex].gameObject;
                newPop.SetActive(true);
                MasterScript.Instance.deads.Remove(newPop.GetComponent<PopScript>());
                MasterScript.Instance.Pops.Add(newPop.GetComponent<PopScript>());
            }

            newPop.GetComponent<PopScript>().Respawn();
            newPop.GetComponent<NavMeshAgent>().enabled = true;
            newPop.GetComponent<NavMeshAgent>().speed = navMeshAgent.speed;
            newPop.GetComponent<PopScript>().rayDistance = rayDistance;
        }
        if (myCurrentTarget == null)
        {
            Transform myTarget = GetClosestFood(MasterScript.Instance.Meals);
            if (myTarget != null)
            {
                myCurrentTarget = myTarget.gameObject;
                navMeshAgent.destination = myTarget.position;
                myCurrentTarget.GetComponent<FoodScript>().Approach();
            }
        }
        if (myCurrentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, myCurrentTarget.transform.position);
            if (dist < 1.5f)
                Eat();
        }
    }
}