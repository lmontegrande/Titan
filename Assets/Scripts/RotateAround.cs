using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour {

    public GameObject target;
    public float rotateSpeed = 1f;
    public float originPullSpeed = 1f;

    private Rigidbody _rigidbody;
    private Vector3 moveTarget;

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        moveTarget = transform.position;

        _rigidbody.angularVelocity = new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180));
    }

    public void Update()
    {
        Vector3 deltaVector = target.transform.position - moveTarget;
        Vector3 orthoVector = Vector3.Cross(deltaVector.normalized, target.transform.up.normalized);
        moveTarget += orthoVector * rotateSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, moveTarget, originPullSpeed);
    }
}
