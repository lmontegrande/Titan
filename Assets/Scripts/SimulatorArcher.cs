using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatorArcher : MonoBehaviour {

    public GameObject launchLocation;
    public GameObject arrowPrefab;
    public float arrowSpawnOffset = 1f;
    public float arrowInitialSpeed = 20f;

    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject arrowInstance = Instantiate(arrowPrefab, launchLocation.transform.position + launchLocation.transform.forward * arrowSpawnOffset, launchLocation.transform.rotation);
            arrowInstance.GetComponent<Rigidbody>().velocity = arrowInstance.transform.forward * arrowInitialSpeed;
            arrowInstance.GetComponent<Rigidbody>().useGravity = false;
        }
    }
}
