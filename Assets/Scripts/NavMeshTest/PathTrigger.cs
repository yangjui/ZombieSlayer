using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTrigger : MonoBehaviour
{
    [SerializeField]
    private NavAgentManager navAgentManager = null;

    [SerializeField]
    private List<Transform> paths = null;

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("Zombie"))
        {
            navAgentManager.SetNewTarget(this, _other.name);
        }
    }

    public Transform PathPosition()
    {
        return this.transform;
    }

    public Transform NextPos()
    {
        for (int i = 0; i < paths.Count; ++i)
        {
            if (paths[i].name == this.name)
            {
                if (i == paths.Count - 1)
                {
                    return paths[0];
                }
                else
                {
                    return paths[i + 1];
                }
            }
        }
        return this.transform;
    }

    public string PathName()
    {
        return this.name;
    }
}