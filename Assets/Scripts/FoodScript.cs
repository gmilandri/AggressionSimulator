using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    //private bool m_timeOngoing;
    public bool IsBeingApproached { get; private set; }
    //public bool IsRegrowing { get; private set; }

    void Start()
    {
        //StartCoroutine(Time());

        GameObject myParent = GameObject.Find("FoodParent");
        transform.parent = myParent.transform;
    }

    //void Update()
    //{
    //    if (!m_timeOngoing)
    //        StartCoroutine(Time());
    //}

    //public void StartRegrowing()
    //{
    //    IsRegrowing = true;
    //}
    public void Approach()
    {
        IsBeingApproached = true;
    }
    public void StopApproach()
    {
        IsBeingApproached = false;
    }

    //IEnumerator Time()
    //{
    //    m_timeOngoing = true;
    //    float offset = 10f + Random.Range(0f, 10f);
    //    yield return new WaitForSeconds(offset);
    //    if (!IsBeingApproached)
    //    {
    //        Respawn();
    //        if (!IsRegrowing)
    //            IsRegrowing = true;
    //        else
    //            IsRegrowing = false;
    //    }
    //    m_timeOngoing = false;
    //}

    public void Respawn()
    {
        transform.position = new Vector3(Random.Range(1, MasterScript.Instance.squareSize), 0, Random.Range(1, MasterScript.Instance.squareSize));
    }

}
