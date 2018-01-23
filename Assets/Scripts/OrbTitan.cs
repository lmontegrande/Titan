using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbTitan : MonoBehaviour {

    public GameObject[] orbs;

    public void Start()
    {
        orbs = GameObject.FindGameObjectsWithTag("Orb");
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            foreach(GameObject orb in orbs)
            {
                orb.GetComponent<Orb>().orbState = Orb.OrbState.firing;
            }
        }
    }
}
