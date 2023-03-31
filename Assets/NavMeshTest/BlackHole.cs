using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlackHole : MonoBehaviour
{
    private NavAgentManager navAgentManager = null;
    private List<GameObject> agents = new List<GameObject>();


    private void Awake()
    {
        navAgentManager = FindObjectOfType<NavAgentManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Agent") || other.CompareTag("Map"))
        {
            navAgentManager.DetectBlackHole(transform.position);
            if (other.CompareTag("Agent"))
            {
                agents.Add(other.gameObject);
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < agents.Count; ++i)
        {
            if (agents[i].CompareTag("Agent"))
            {
                Debug.Log("BlackHole Update");
                Debug.Log(agents[i].name + "hitByblackHole");
                agents[i].GetComponent<NavAgentTest>().HitByBlackHole(this.transform.position);
            }
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < agents.Count; ++i)
        {
            agents[i].GetComponent<NavAgentTest>().ResetAgent();
        }
    }

}