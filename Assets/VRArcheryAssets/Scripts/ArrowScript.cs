using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float DespawnDistance = 3000;
    public bool isExplodingArrow = true;

    Vector3 ArrowVelocity;
    float xVelocity;
    float yVelocity;
    float zVelocity;

    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {       
        //Work out relative velocity of arrow to use for rotation and to decide if the arrow is going fast enough to stick into its target.
        ArrowVelocity = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
        xVelocity = ArrowVelocity.x * 3 - GetComponent<Rigidbody>().angularVelocity.x;
        yVelocity = ArrowVelocity.y * -3 - GetComponent<Rigidbody>().angularVelocity.y;
        zVelocity = ArrowVelocity.z;        

        //if arrow is beyond specified distance destroy it, just in case arrows escape the level and drag performance down mainly.
        if ((player.transform.position - transform.position).magnitude > DespawnDistance) {
            Destroy(gameObject);
        }

        if (GetComponent<Rigidbody>().isKinematic == false)
        {
            //Rotates arrow to face movement direction, by default I use torque to rotate the arrows which can lead to more unexpected quirky results, but if you prefer that they stay precicely pointing in the movement direction with no physics wobble then comment out the torque version and un comment the bellow quarteration version.
            GetComponent<Rigidbody>().AddRelativeTorque(yVelocity, xVelocity, 0);
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(GetComponent<Rigidbody>().velocity.normalized), Time.time * 0.01f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isExplodingArrow)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }

        //Make sure other obect is not an arrow (looks dumb when two arrows hit mid air and stop there!)
        if (zVelocity > 3 && collision.gameObject.tag != "Arrow")
        {
        //stop physics simulation when imbeded in object
            GetComponent<Rigidbody>().isKinematic = true;                      
            
            // PS THIS IS WHERE YOU ,MIGHT DO DAMAGE TO THE HIT OBJECT, SPAWN EXPLOSIONS ETC

            //if the other object has physics make this a child so the arrow will not be left behind (note if you have something without physics you want them to attach to either add a kinematic ridgedbody to that object or remove the below if statment.
            if (collision.gameObject.GetComponent<Rigidbody>() != null)
            {                
                transform.parent = collision.transform;

                //Just for the demo add gravity to targets when they are hit so they fall
                //collision.gameObject.GetComponent<Rigidbody>().useGravity = true;
            
            }
        }
    }
}
