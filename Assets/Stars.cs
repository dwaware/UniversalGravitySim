using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour
{
    public List<GameObject> sphereList = new List<GameObject>();
    Vector3 sunScale = new Vector3(20, 20, 20);
    Vector3 planetScale = new Vector3(5, 5, 5);

    // Start is called before the first frame update
    void Start()
    {
        GameObject g0 = Instantiate(sphereList[0], new Vector3(0, 0, 0), Quaternion.identity);
        g0.transform.localScale = sunScale;
        Rigidbody sunRB = g0.GetComponent<Rigidbody>();
        sunRB.mass = 330000;
        sunRB.velocity = new Vector3(0, 0, 0);

        GameObject g1 = Instantiate(sphereList[1], new Vector3(100, 0, 100), Quaternion.identity);
        g1.transform.localScale = planetScale;
        Rigidbody planetRB = g1.GetComponent<Rigidbody>();
        planetRB.mass = 1;
        planetRB.velocity = new Vector3(200, 0, -200);
    }

    // Update is called once per frame
    void Update()
    {

    }
}