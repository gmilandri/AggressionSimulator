using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PopScript : MonoBehaviour
{
    public int EnergyRemaining;
    public GameObject myCurrentTarget;
    public bool isDying;
    private RaycastHit hit;
    private Ray ray;
    public float rayDistance;
    public float socialCooldown;
    private bool socialCoroutine;
    public bool coroutineOngoing = false;
    public static int Count;
    public bool hadEncounter;
    public float timeOffset;
    public int energySpentForReproduction;
    public int gatheredFood;

    public NavMeshAgent navMeshAgent;

    private void Awake()
    {
        MasterScript.Instance.Pops.Add(this.gameObject);
        Count++;
        gameObject.name = "Pop n." + Count.ToString();
        EnergyRemaining = 3;
        gatheredFood = MasterScript.Instance.FoodEnergy;
        SetScores();
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        GameObject myParent = GameObject.Find("PopParent");
        transform.parent = myParent.transform;
    }

    private void Update()
    {
        if (Time.frameCount % 3 == 0)
        {
            if (!isDying)
            {
                ray = new Ray(transform.position, transform.forward);
                MyRoutine(ray);
            }

            if (!socialCoroutine)
                StartCoroutine(SocialEncounters());
            if (!coroutineOngoing)
                StartCoroutine(FoodSpending());
        }

    }

    public void Respawn()
    {
        transform.position = new Vector3(Random.Range(1, MasterScript.Instance.squareSize), 0, Random.Range(1, MasterScript.Instance.squareSize));
    }

    public virtual void MyRoutine(Ray myRay)
    {

        if (Physics.Raycast(myRay, out hit))
        {
            if (hit.distance < rayDistance)
            {
                GameObject target = hit.collider.gameObject;
                if (target.tag == "Pop" && !hadEncounter && !target.GetComponent<PopScript>().isDying)
                {
                    PopScript other = hit.collider.gameObject.GetComponent<PopScript>();
                    MyBehaviour(other);
                }
            }
        }
    }

    IEnumerator SocialEncounters()
    {
        if (!isDying)
        {
            socialCoroutine = true;
            yield return new WaitForSeconds(socialCooldown);
            hadEncounter = false;
            socialCoroutine = false;
        }
    }


    IEnumerator FoodSpending()
    {
        if (!isDying)
        {
            coroutineOngoing = true;
            EnergyRemaining--;
            if (CheckDying(EnergyRemaining))
            {
                isDying = true;
                StartCoroutine(PopDeath());
            }
            if (!isDying)
                if (CheckReproduction(EnergyRemaining))
                    Reproduction();
            if (!isDying)
            {
                yield return new WaitForSeconds(timeOffset);
                coroutineOngoing = false;
            }
            else
                yield return null;
        }
    }

    bool CheckReproduction(int energy)
    {
        if (energy >= 12)
            return true;
        else
            return false;
    }

    bool CheckDying(int energy)
    {
        if (energy <= 0)
            return true;
        else
            return false;
    }

    public void Eat()
    {
        myCurrentTarget.GetComponent<FoodScript>().StopApproach();
        myCurrentTarget.GetComponent<FoodScript>().Respawn();
        //myCurrentTarget.GetComponent<FoodScript>().StartRegrowing();
        
        myCurrentTarget = null;

        EnergyRemaining += gatheredFood;
    }

    public void Die()
    {
        isDying = true;
        StartCoroutine(PopDeath());
    }

    IEnumerator PopDeath()
    {
        if (myCurrentTarget != null && myCurrentTarget.GetComponent<FoodScript>())
            myCurrentTarget.GetComponent<FoodScript>().StopApproach();
        Animation anim = GetComponent<Animation>();
        anim.Play("Death");
        while (GetComponent<Animation>().isPlaying)
            yield return null;
        MasterScript.Instance.Pops.Remove(gameObject);
        MasterScript.Instance.deads.Add(gameObject);
        EnergyRemaining = 3;
        myCurrentTarget = null;
        coroutineOngoing = false;
        hadEncounter = false;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        transform.position = new Vector3(0f, 0.575f, 0f);
        isDying = false;
        navMeshAgent.enabled = false;
        gameObject.SetActive(false);
    }

    public virtual void MyBehaviour (PopScript other)
    {
        hadEncounter = true;
    }

    public virtual void SetScores()
    {

    }

    public virtual void Reproduction ()
    {

    }

    public Transform GetClosestFood(Transform[] meals)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform t in meals)
        {
            FoodScript foodScript = t.gameObject.GetComponent<FoodScript>();
            if (foodScript.IsBeingApproached) //foodScript.IsRegrowing || foodScript.IsBeingApproached)
                continue;
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

    public Transform GetClosestDove(List<GameObject> doves)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in doves)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (!t.GetComponent<Dove>() || (t.GetComponent<Dove>() && t.GetComponent<Dove>().EnergyRemaining < 4))
                continue;
            if (dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin;
    }

    public Transform GetClosestHawk(List<GameObject> hawks)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in hawks)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (!t.GetComponent<Hawk>())
                continue;
            if (dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin;
    }

}