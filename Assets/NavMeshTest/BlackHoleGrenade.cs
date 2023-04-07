using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BlackHoleGrenade : MonoBehaviour
{
    [SerializeField]
    private NavAgentManager navAgentManager = null;

    [SerializeField] 
    private GameObject obstacle;

    private float destroyTime = 7f;

    private void Awake()
    {
        navAgentManager = FindObjectOfType<NavAgentManager>();
    }

    private void Start()
    {
        StartCoroutine(BlackholeStart());
    }

    private IEnumerator BlackholeStart()
    {
        yield return null;
        GameObject newOBJ = Instantiate(obstacle, transform.position, Quaternion.Euler(Vector3.zero));
        navAgentManager.DetectNewObstacle(transform.position);
        Destroy(newOBJ, destroyTime);
        Destroy(this.gameObject);
    }

    public void GrenadeRigidbody(Vector3 _direction)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(_direction, ForceMode.Impulse);
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
    }

    private void OnDestroy()
    {
        navAgentManager.ResetAgent();
    }

}
