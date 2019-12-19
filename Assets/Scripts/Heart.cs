using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 2f);
    }
}
