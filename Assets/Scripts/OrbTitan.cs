using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbTitan : MonoBehaviour {

    public GameObject[] orbs;
    public GameObject[] bodyOrbs;

    private int health;

    public void Start()
    {
        foreach (GameObject bodyOrb in bodyOrbs)
        {
            bodyOrb.GetComponent<TitanBodyOrb>().OnHit += FireOrbs;
            bodyOrb.GetComponent<TitanBodyOrb>().OnDie += OrbKilled;
            health++;
        }
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire2"))
            FireOrbs();
    }

    private void FireOrbs()
    {
        foreach (GameObject orb in orbs)
        {
            orb.GetComponent<Orb>().orbState = Orb.OrbState.firing;
        }
    }

    private void OrbKilled()
    {
        health--;
        if (health <= 0)
        {
            foreach (GameObject orb in orbs)
            {
                orb.GetComponent<Orb>().Die();
            }
        }
    }
}
