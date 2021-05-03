using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBossSpawner : MonoBehaviour
{
    [SerializeField] public Transform spawnPointLeft;
    [SerializeField] public Transform spawnPointRight;
    [SerializeField] public GameObject bulletPrefab;

    private float bulletSpawnRate = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnBulletsFromLeft(int amount)
    {
        StartCoroutine(SpawnBullets(amount, bulletSpawnRate, "Left"));
    }

    public void SpawnBulletsFromRight(int amount)
    {
        StartCoroutine(SpawnBullets(amount, bulletSpawnRate, "Right"));
    }

    IEnumerator SpawnBullets(int amount, float delay, string spawnPoint)
    {
        GameObject tempBullet;
        
        for (int i = 0; i < amount; i++)
        {
            if (spawnPoint == "Left")
            {
                tempBullet = Instantiate(bulletPrefab, spawnPointLeft.transform.position, Quaternion.identity);
                tempBullet.GetComponent<Bullet>().Init(Vector2.right);    
            }
            else
            {
                tempBullet = Instantiate(bulletPrefab, spawnPointRight.transform.position, Quaternion.identity);
                tempBullet.GetComponent<Bullet>().Init(Vector2.left);
            }

            yield return new WaitForSeconds(delay);

        }
        
    }
    
    


}
