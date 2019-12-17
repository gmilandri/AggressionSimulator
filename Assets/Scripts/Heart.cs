using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public GameObject myPop;
    // Start is called before the first frame update
    void Start()
    {
        GameObject myParent = GameObject.Find("EmojiParent");
        transform.parent = myParent.transform;
        Destroy(gameObject, 2f);
    }

    private void Update()
    {
        transform.position = new Vector3(myPop.transform.position.x, myPop.transform.position.y + 1f, myPop.transform.position.z);
    }

}
