using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PopScript : MonoBehaviour
{
    public int food;
    public GameObject myCurrentTarget;
    public bool isDying;
    //public bool dove = true;
    //public int perception = 2;
    //public bool foundFood;
    //public bool haveTempDestination;
    //public Vector3 tempDestination;
    public float radius;
    private RaycastHit hit;
    private Ray ray;
    public float rayDistance;
    public float socialCooldown;
    private bool socialCoroutine;
    public bool coroutineOngoing = false;
    public static int Count;
    public bool hadEncounter;
    public GameObject heart;
    public float timeOffset;
    public int gatheredFood;
    public int energySpentForReproduction;
    //public bool isOther;

    public NavMeshAgent navMeshAgent;

    public MasterScript masterScript;

    private void Awake()
    {
        GameObject master = GameObject.FindGameObjectWithTag("GameController");
        masterScript = master.GetComponent<MasterScript>();
        masterScript.Pops.Add(this.gameObject);
        Count++;
        this.gameObject.name = "Pop n." + Count.ToString();
        food = 3;
        SetScores();
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        radius = masterScript.radius;
        GameObject myParent = GameObject.Find("PopParent");
        transform.parent = myParent.transform;
    }

    private void Update()
    {
        if (!isDying)
        {
            if (!navMeshAgent.enabled)
                navMeshAgent.enabled = true;
            ray = new Ray(transform.position, transform.forward);
            MyRoutine(ray);
        }

        if (!socialCoroutine)
            StartCoroutine("SocialEncounters");
        if (!coroutineOngoing)
            StartCoroutine("FoodSpending");

    }

    public virtual void MyRoutine(Ray myRay)
    {

        if (Physics.Raycast(myRay, out hit))
        {
            if (hit.distance < rayDistance)
            {
                GameObject target = hit.collider.gameObject;
                if (target.tag == "Pop" && !hadEncounter && !target.GetComponent<PopScript>().isDying)// && !target.GetComponent<PopScript>().hadEncounter)
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
            food--;
            if (CheckDying(food))
            {
                isDying = true;
                StartCoroutine("PopDeath");
            }
            if (!isDying)
                if (CheckReproduction(food))
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
        float randomAngle = Random.Range(0f, Mathf.PI * 2);
        float maxPosX = Mathf.Cos(randomAngle) * radius;
        float maxPosZ = Mathf.Sin(randomAngle) * radius;
        float randomX = Random.Range(0f, maxPosX);
        float randomZ = Random.Range(0f, maxPosZ);

        myCurrentTarget.transform.position = new Vector3(randomX, 0, randomZ);
        myCurrentTarget.GetComponent<FoodScript>().regrowing = true;
        food += gatheredFood;

        myCurrentTarget.GetComponent<FoodScript>().isBeingApproached = false;
        myCurrentTarget = null;
        //foundFood = false;
    }

    public void Die()
    {
        isDying = true;
        StartCoroutine("PopDeath");
    }

    IEnumerator PopDeath()
    {
        if (myCurrentTarget != null && myCurrentTarget.GetComponent<FoodScript>())
            myCurrentTarget.GetComponent<FoodScript>().isBeingApproached = false;
        Animation anim = GetComponent<Animation>();
        anim.Play("Death");
        while (GetComponent<Animation>().isPlaying)
            yield return null;
        masterScript.Pops.Remove(gameObject);
        masterScript.deads.Add(gameObject);
        food = 3;
        myCurrentTarget = null;
        //foundFood = false;
        //haveTempDestination = false;
        coroutineOngoing = false;
        //tempDestination = Vector3.zero;
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
            if (foodScript.regrowing || foodScript.isBeingApproached)
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
            if (!t.GetComponent<Dove>() || (t.GetComponent<Dove>() && t.GetComponent<Dove>().food < 4))
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


public class Dove : PopScript
{
    public override void MyBehaviour(PopScript other)
    {
        base.MyBehaviour(other);

        //DOVES GIFT FOOD TO EVERYONE BUT SACRIFICES THEMSELVES TO GUARDIANS

        if (other is Guardian)
        {
            print("I gave food to a Guardian.");
            other.food += food;
            food = 0;
        }
        else
        {
            other.food += food / 2;
            food /= 2;
        }

        SpawnHeart();
    }

    public void SpawnHeart()
    {
        Vector3 heartPos = transform.position;
        heartPos.y += 0.2f;
        GameObject myHeart = Instantiate(masterScript.heartPrefab);
        myHeart.GetComponent<Heart>().myPop = gameObject;
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
        food -= energySpentForReproduction;
        GameObject newPop = null;
        int deadsIndex = -1;
        for (int i = 0; i < masterScript.deads.Count; i++)
        {
            if (masterScript.deads[i].GetComponent<PopScript>() is Dove)
            {
                deadsIndex = i;
                break;
            }
        }
        if (deadsIndex == -1)
        {
            newPop = Instantiate(masterScript.popPrefab, new Vector3(0f, 100f, 0f), Quaternion.identity);
            newPop.GetComponent<MeshRenderer>().material.color = Color.cyan;
            newPop.AddComponent<Dove>();
        }
        else
        {
            newPop = masterScript.deads[deadsIndex];
            newPop.SetActive(true);
            masterScript.deads.Remove(newPop);
            masterScript.Pops.Add(newPop);
        }

        float randomAngle = Random.Range(0f, Mathf.PI * 2);
        float maxPosX = Mathf.Cos(randomAngle) * radius;
        float maxPosZ = Mathf.Sin(randomAngle) * radius;
        float randomX = Random.Range(0f, maxPosX);
        float randomZ = Random.Range(0f, maxPosZ);
        Vector3 newPos = new Vector3(randomX, 0.575f, randomZ);
        newPop.GetComponent<NavMeshAgent>().Warp(newPos);
        newPop.GetComponent<NavMeshAgent>().speed = navMeshAgent.speed;
        newPop.GetComponent<PopScript>().rayDistance = rayDistance;

    }

    public override void MyRoutine(Ray myRay)
    {
        base.MyRoutine(myRay);
        int guardianCount = 0;
        foreach (GameObject t in masterScript.Pops)
        {
            if (t.GetComponent<Guardian>())
                guardianCount++;
        }
        if (guardianCount == 0)
        {
            GameObject newPop = null;
            int deadsIndex = -1;
            for (int i = 0; i < masterScript.deads.Count; i++)
            {
                if (masterScript.deads[i].GetComponent<PopScript>() is Guardian)
                {
                    deadsIndex = i;
                    break;
                }
            }
            if (deadsIndex == -1)
            {
                newPop = Instantiate(masterScript.popPrefab, new Vector3(0f, 100f, 0f), Quaternion.identity);
                newPop.GetComponent<MeshRenderer>().material.color = Color.yellow;
                newPop.AddComponent<Guardian>();
            }
            else
            {
                newPop = masterScript.deads[deadsIndex];
                newPop.SetActive(true);
                masterScript.deads.Remove(newPop);
                masterScript.Pops.Add(newPop);
            }

            float randomAngle = Random.Range(0f, Mathf.PI * 2);
            float maxPosX = Mathf.Cos(randomAngle) * radius;
            float maxPosZ = Mathf.Sin(randomAngle) * radius;
            float randomX = Random.Range(0f, maxPosX);
            float randomZ = Random.Range(0f, maxPosZ);
            Vector3 newPos = new Vector3(randomX, 0.575f, randomZ);
            newPop.GetComponent<NavMeshAgent>().Warp(newPos);
            newPop.GetComponent<NavMeshAgent>().speed = navMeshAgent.speed;
            newPop.GetComponent<PopScript>().rayDistance = rayDistance;
        }
        if (myCurrentTarget == null)
        {
            Transform myTarget = GetClosestFood(masterScript.Meals);
            if (myTarget != null)
            {
                myCurrentTarget = myTarget.gameObject;
                navMeshAgent.destination = myTarget.position;
                myCurrentTarget.GetComponent<FoodScript>().isBeingApproached = true;
            }
        }
        if (myCurrentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, myCurrentTarget.transform.position);
            if (dist < 1.5f)
                base.Eat();
        }
    }
}

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
            food = 0;
            other.food = 0;
        }
        //...BUT THEY STEAL ALL THE FOOD FROM DOVES...
        else if (other is Dove)
        {
            food += other.food;
            other.food = 0;
            SpawnHeart();
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
        food -= energySpentForReproduction;
        GameObject newPop = null;
        int deadsIndex = -1;
        for (int i = 0; i < masterScript.deads.Count; i++)
        {
            if (masterScript.deads[i].GetComponent<PopScript>() is Hawk)
            {
                deadsIndex = i;
                break;
            }
        }
        if (deadsIndex == -1)
        {
            newPop = Instantiate(masterScript.popPrefab, new Vector3(0f, 100f, 0f), Quaternion.identity);
            newPop.GetComponent<MeshRenderer>().material.color = Color.red;
            newPop.AddComponent<Hawk>();
        }
        else
        {
            newPop = masterScript.deads[deadsIndex];
            newPop.SetActive(true);
            masterScript.deads.Remove(newPop);
            masterScript.Pops.Add(newPop);
        }

        float randomAngle = Random.Range(0f, Mathf.PI * 2);
        float maxPosX = Mathf.Cos(randomAngle) * radius;
        float maxPosZ = Mathf.Sin(randomAngle) * radius;
        float randomX = Random.Range(0f, maxPosX);
        float randomZ = Random.Range(0f, maxPosZ);
        Vector3 newPos = new Vector3(randomX, 0.575f, randomZ);
        newPop.GetComponent<NavMeshAgent>().Warp(newPos);
        newPop.GetComponent<NavMeshAgent>().speed = navMeshAgent.speed;
        newPop.GetComponent<PopScript>().rayDistance = rayDistance;

    }

    public override void MyRoutine(Ray myRay)
    {
        base.MyRoutine(myRay);
        Transform myTarget = GetClosestDove(masterScript.Pops);
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
        Vector3 heartPos = transform.position;
        heartPos.y += 0.2f;
        GameObject myHeart = Instantiate(masterScript.heartPrefab);
        myHeart.GetComponent<Heart>().myPop = gameObject;
        foreach (Transform t in myHeart.transform)
            t.gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
    }

}

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
            food += other.food;
            other.food = 0;
            SpawnHeart();
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
        food -= energySpentForReproduction;
        GameObject newPop = null;
        int deadsIndex = -1;
        for (int i = 0; i < masterScript.deads.Count; i++)
        {
            if (masterScript.deads[i].GetComponent<PopScript>() is Guardian)
            {
                deadsIndex = i;
                break;
            }
        }
        if (deadsIndex == -1)
        {
            newPop = Instantiate(masterScript.popPrefab, new Vector3(0f, 100f, 0f), Quaternion.identity);
            newPop.GetComponent<MeshRenderer>().material.color = Color.yellow;
            newPop.AddComponent<Guardian>();
        }
        else
        {
            newPop = masterScript.deads[deadsIndex];
            newPop.SetActive(true);
            masterScript.deads.Remove(newPop);
            masterScript.Pops.Add(newPop);
        }

        float randomAngle = Random.Range(0f, Mathf.PI * 2);
        float maxPosX = Mathf.Cos(randomAngle) * radius;
        float maxPosZ = Mathf.Sin(randomAngle) * radius;
        float randomX = Random.Range(0f, maxPosX);
        float randomZ = Random.Range(0f, maxPosZ);
        Vector3 newPos = new Vector3(randomX, 0.575f, randomZ);
        newPop.GetComponent<NavMeshAgent>().Warp(newPos);
        newPop.GetComponent<NavMeshAgent>().speed = navMeshAgent.speed;
        newPop.GetComponent<PopScript>().rayDistance = rayDistance;

    }

    public override void MyRoutine(Ray myRay)
    {
        base.MyRoutine(myRay);
        Transform myTarget = GetClosestHawk(masterScript.Pops);
        if (myTarget == null)
        {
            GameObject newPop = null;
            int deadsIndex = -1;
            for (int i = 0; i < masterScript.deads.Count; i++)
            {
                if (masterScript.deads[i].GetComponent<PopScript>() is Hawk)
                {
                    deadsIndex = i;
                    break;
                }
            }
            if (deadsIndex == -1)
            {
                newPop = Instantiate(masterScript.popPrefab, new Vector3(0f, 100f, 0f), Quaternion.identity);
                newPop.GetComponent<MeshRenderer>().material.color = Color.red;
                newPop.AddComponent<Hawk>();
            }
            else
            {
                newPop = masterScript.deads[deadsIndex];
                newPop.SetActive(true);
                masterScript.deads.Remove(newPop);
                masterScript.Pops.Add(newPop);
            }

            float randomAngle = Random.Range(0f, Mathf.PI * 2);
            float maxPosX = Mathf.Cos(randomAngle) * radius;
            float maxPosZ = Mathf.Sin(randomAngle) * radius;
            float randomX = Random.Range(0f, maxPosX);
            float randomZ = Random.Range(0f, maxPosZ);
            Vector3 newPos = new Vector3(randomX, 0.575f, randomZ);
            newPop.GetComponent<NavMeshAgent>().Warp(newPos);
            newPop.GetComponent<NavMeshAgent>().speed = navMeshAgent.speed;
            newPop.GetComponent<PopScript>().rayDistance = rayDistance;
        }
        myTarget = GetClosestHawk(masterScript.Pops);
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
        Vector3 heartPos = transform.position;
        heartPos.y += 0.2f;
        GameObject myHeart = Instantiate(masterScript.heartPrefab);
        myHeart.GetComponent<Heart>().myPop = gameObject;
        foreach (Transform t in myHeart.transform)
            t.gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }
}
