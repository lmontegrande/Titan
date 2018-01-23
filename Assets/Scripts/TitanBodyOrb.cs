using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitanBodyOrb : MonoBehaviour, IHittable {

    public delegate void OnHitHandle();
    public OnHitHandle OnHit;
    public delegate void OnDieHandle();
    public OnDieHandle OnDie;

    public Material deadMaterial;
    public int health;

    private bool isDead;

    public void GetHit()
    {
        if (isDead) return;

        if (OnHit != null)
            OnHit.Invoke();

        health--;
        if (health <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        GetComponent<MeshRenderer>().material = deadMaterial;
        if (OnDie != null)
            OnDie.Invoke();
    }
}
