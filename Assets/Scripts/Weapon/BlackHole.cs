using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlackHole : MonoBehaviour
{
    private List<GameObject> agents = new List<GameObject>();
    private float destroyTime = 7f;
    [SerializeField] private GameObject bombEffect = null;

    private float scale = 0.0f;
    private bool isGrowingFast = true;
    private bool isGrowingSlow = false;

    private void Start()
    {
        StartCoroutine(DestroyCoroutine());
    }

    private void Update()
    {
        if (isGrowingFast)
        {
            // 0에서 0.2까지 빠르게 커지는 부분
            Debug.Log("1");
            scale = Mathf.SmoothStep(scale, 0.05f, Time.deltaTime * 10.0f);
            if (scale >= 0.04f)
            {
                isGrowingFast = false;
                isGrowingSlow = true;
            }
        }
        else if (isGrowingSlow)
        {
            Debug.Log("2");
            // 0.2에서 0.8까지 서서히 커지는 부분
            scale = Mathf.Lerp(scale, 0.4f, Time.deltaTime * 5.0f);
            if (scale >= 0.79f)
            {
                isGrowingSlow = false;
                isGrowingFast = true;
            }
        }

        transform.localScale = new Vector3(scale, scale, scale);

        if (agents.Count == 0) return;

        for (int i = 0; i < agents.Count; ++i)
        {
            if (agents[i] == null) continue;
            if (agents[i].CompareTag("Zombie"))
            {
                agents[i].GetComponent<BakeZombie>().TakeDamage(0);
                agents[i].GetComponent<BakeZombie>().BlackHole();

                Vector3 dir = transform.position - agents[i].transform.position;
                agents[i].transform.position += dir * 3f * Time.deltaTime;
            }
        }
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(destroyTime);
        isGrowingSlow = false;
        isGrowingFast = false;
        while (true)
        {
            scale = Mathf.SmoothStep(scale, 0.05f, Time.deltaTime * 20.0f);
            yield return null;
            if (scale <= 0.06f) 
            {
                for (int i = agents.Count - 1; i >= 0; --i)
                {
                    if (agents[i] == null) continue;
                    agents[i].GetComponent<BakeZombie>().TakeDamage(100);
                    agents.RemoveAt(i);
                }
                GameObject go = Instantiate(bombEffect, transform.position, Quaternion.identity);
                Destroy(go, 2f);
                Destroy(gameObject);
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("Zombie"))
        {
            agents.Add(_other.gameObject);
        }
    }
}