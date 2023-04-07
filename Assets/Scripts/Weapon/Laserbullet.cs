using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Laserbullet : MonoBehaviour
{
    [SerializeField] private float damage;

    [SerializeField] private GameObject leftZombiePrefab;
    [SerializeField] private GameObject rightZombiePrefab;

    private void Start()
    {
        Destroy(gameObject, 1f);
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("Zombie"))
        {
            Zombie zombie = _other.GetComponent<Zombie>();
            zombie.TakeDamage(damage);
            
            Vector3 Dir = transform.position - _other.transform.position;
            float angle = Vector3.SignedAngle(Dir, transform.forward, Vector3.up);

            GameObject dummyPrefab = angle < 0 ? leftZombiePrefab : rightZombiePrefab;
            Instantiate(dummyPrefab, _other.transform.position, Quaternion.identity);

            //bool dummyPrefab = angle < 0 ? true : false;
            //zombie.레이저좀비소환(dummyPrefab);
        }
    }
}