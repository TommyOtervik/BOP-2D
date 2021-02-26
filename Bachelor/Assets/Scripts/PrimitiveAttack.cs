using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimitiveAttack : MonoBehaviour
{
    public Transform attackPoint;

    public GameObject bulletPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Attack1();
    }

    // Update is called once per frame
    void Update()
    {
        Attack1();
    }

    void Attack1()
    {
        GameObject tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<PrimitiveObjectMove>().Init(Vector2.left);
        
        tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<PrimitiveObjectMove>().Init(Vector2.up);
        
        tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<PrimitiveObjectMove>().Init(Vector2.right);
        
        tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<PrimitiveObjectMove>().Init(Vector2.down);
        
        
        
    }

    void Attack2()
    {
        
    }
}
