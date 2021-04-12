using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorMovement : MonoBehaviour
{

    private Vector3 startPos;
    private Vector3 endPos;

    private Vector3 nextPos;

    [SerializeField] 
    private float speed;

    private bool moving;

    private Transform trans;
    [SerializeField]
    private Transform endTransform;

    // Start is called before the first frame update
    void Start()
    {
        trans = this.transform;

        startPos = trans.localPosition;
        endPos = endTransform.localPosition;

        nextPos = endPos;
    }

    
    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        trans.localPosition = Vector3.MoveTowards(trans.localPosition, nextPos, speed * Time.deltaTime);

        if (Vector3.Distance(trans.localPosition, nextPos) <= 0.1)
        {

            StartCoroutine(ElevatorWait());

            ChangeDestination();
        }
    }

    private void ChangeDestination()
    {
        nextPos = nextPos != startPos ? startPos : endPos;
    }

    IEnumerator ElevatorWait()
    {
        yield return new WaitForSeconds(3f);
        
    }


    // Holder spilleren på plattformen
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(trans);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(null);
        }
    }
}
