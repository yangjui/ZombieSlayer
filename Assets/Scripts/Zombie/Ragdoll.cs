using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] private GameObject pelvis = null;
    private Rigidbody rb;
    private float onGroundTime = 0;
    private bool isGround = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (rb.velocity == Vector3.zero && isGround)
        {
            onGroundTime += Time.deltaTime;
            if (onGroundTime >= 2f)
                Kinematic();
        }
        else
        {
            onGroundTime -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("Platform"))
        {
            isGround = true;
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.CompareTag("Platform"))
        {
            isGround = false;
        }
    }

    //private void ColliderPosition()
    //{
    //    rb.constraints = RigidbodyConstraints.FreezeAll;
    //}

    public void Kinematic()
    {
        transform.position = pelvis.transform.position;
        pelvis.GetComponent<DeadZombie>().SetPosition();
    }
}