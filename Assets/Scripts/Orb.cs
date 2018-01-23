using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour, IHittable{

    public enum OrbState {rotating, firing, dead}

    public GameObject rotateTarget;
    public Material rotatingMaterial;
    public Material firingMaterial;
    public Material deadMaterial;
    public AudioClip collisionAudioClip;
    public float rotateSpeed = 1f;
    public float fireSpeed = 5f;
    public float originPullSpeed = 1f;
    public OrbState orbState;

    private Rigidbody _rigidbody;
    private MeshRenderer _meshRenderer;
    private AudioSource _audioSource;
    private GameObject player;
    private Vector3 moveTarget;
    private bool isGrounded;

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        moveTarget = transform.position;

        orbState = OrbState.rotating;
        _rigidbody.angularVelocity = new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180));
        StartCoroutine(PlaySound());
    }

    public void Update()
    {
        if (orbState == OrbState.dead)
            return;

        UpdateVariables();

        if (orbState == OrbState.rotating)
            RotatingState();
        if (orbState == OrbState.firing)
            FiringState();
    }

    public void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Tree":
                Destroy(collision.gameObject);
                GetHit();
                break;
            case "Player":
                collision.gameObject.GetComponent<ArcherPlayer>().GetHit();
                GetHit();
                break;
            case "Floor":
                if (!isGrounded)
                {
                    iTween.ShakePosition(GameObject.Find("Arena"), new Vector3(1f, 1f, 1f), .1f);
                    _audioSource.PlayOneShot(collisionAudioClip);
                    _rigidbody.velocity = Vector3.zero;
                    isGrounded = true;
                }
                break;
        }
    }

    public void Die()
    {
        orbState = OrbState.dead;
        _rigidbody.useGravity = true;
        _rigidbody.velocity = Vector3.zero;
        _meshRenderer.material = deadMaterial;
        gameObject.layer = SortingLayer.GetLayerValueFromName("Ground");
        _audioSource.Stop();
    }

    public void GetHit()
    {
        if (orbState == OrbState.dead)
            return;

        _audioSource.PlayOneShot(collisionAudioClip);
        _rigidbody.velocity = Vector3.zero;
        orbState = OrbState.rotating;

        iTween.ShakePosition(GameObject.Find("Arena"), new Vector3(1f, 1f, 1f), .1f);
    }

    private void UpdateVariables()
    {
        switch (orbState)
        {
            case OrbState.rotating:
                _meshRenderer.material = rotatingMaterial;
                break;
            case OrbState.firing:
                _meshRenderer.material = firingMaterial;
                break;
        }

        Vector3 deltaVector = rotateTarget.transform.position - moveTarget;
        Vector3 orthoVector = Vector3.Cross(deltaVector.normalized, rotateTarget.transform.up.normalized);
        moveTarget += orthoVector * rotateSpeed * Time.deltaTime;
    }

    private void RotatingState()
    {
        _rigidbody.MovePosition(Vector3.MoveTowards(transform.position, moveTarget, originPullSpeed * Time.deltaTime));
    }

    private void FiringState()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        _rigidbody.MovePosition(Vector3.MoveTowards(transform.position, player.transform.position, fireSpeed * Time.deltaTime));
    }

    private IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(Random.Range(0, 4));
        _audioSource.Play();
    }
}
