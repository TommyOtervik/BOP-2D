using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimitiveAttack : MonoBehaviour
{
    public Transform attackPoint;

    private float attackRate = 1.5f;
    
    public GameObject bulletPrefab;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AttackPattern());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AttackPattern()
    {
        // Egentlig while fienden er i livet, evt om vi skal ha en trigger range før første angrep?
        while (true)
        {
            yield return new WaitForSeconds(attackRate);
            HorizontalVerticalAttack();
            yield return new WaitForSeconds(attackRate);
            DiagonalAttack();
        }

    }

    void HorizontalVerticalAttack()
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

    void DiagonalAttack()
    {
        GameObject tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<PrimitiveObjectMove>().Init(new Vector2(-1, 1));
        
        tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<PrimitiveObjectMove>().Init(new Vector2(1, 1));
        
        tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<PrimitiveObjectMove>().Init(new Vector2(-1, -1));
        
        tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<PrimitiveObjectMove>().Init(new Vector2(1, -1));
        
        
    }
}
