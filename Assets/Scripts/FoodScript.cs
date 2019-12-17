using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour
{

    public bool isBeingApproached = false;
    private MasterScript masterScript;
    [HideInInspector] public float radius;
    private bool timeOngoing;
    private float offset;
    public bool regrowing;

    private void Start()
    {
        GameObject master = GameObject.FindGameObjectWithTag("GameController");
        masterScript = master.GetComponent<MasterScript>();
        radius = masterScript.radius;
        StartCoroutine("Time");

        GameObject myParent = GameObject.Find("FoodParent");
        transform.parent = myParent.transform;
    }

    private void Update()
    {
        if (!timeOngoing)
            StartCoroutine("Time");
    }

    IEnumerator Time()
    {
        timeOngoing = true;
        offset = 10f + Random.Range(0f, 10f);
        yield return new WaitForSeconds(offset);
        if (!isBeingApproached)
        {
            float randomAngle = Random.Range(0f, Mathf.PI * 2);
            float maxPosX = Mathf.Cos(randomAngle) * radius;
            float maxPosZ = Mathf.Sin(randomAngle) * radius;
            float randomX = Random.Range(0f, maxPosX);
            float randomZ = Random.Range(0f, maxPosZ);

            transform.position = new Vector3(randomX, 0, randomZ);
            if (!regrowing)
                regrowing = true;
            else
                regrowing = false;
        }
        timeOngoing = false;
    }

}
