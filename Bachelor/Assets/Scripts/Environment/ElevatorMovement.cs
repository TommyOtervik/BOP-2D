using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Dette skriptet tilhører heis objektet.
 * Startposisjon er der man setter objektet i spillverdenen, setter 
 * sluttposisjonen basert på en Transfom (pos).
 * 
 * @AOP - 225280
 */
public class ElevatorMovement : MonoBehaviour
{
    // Posisjoner
    private Vector3 startPos; 
    private Vector3 endPos;
    private Vector3 nextPos; // Med på å beregne hvor den skal gå

    [SerializeField] private float speed; // Hastighet

    private Transform trans; // Posisjonen
    [SerializeField] 
    private Transform endTransform; // Slutt pos fra editor


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
        // Beveger seg til et punkt (nextPos)
        trans.localPosition = Vector3.MoveTowards(trans.localPosition, nextPos, speed * Time.deltaTime);
        // Hvis heisen når punktet, endre retning
        if (Vector3.Distance(trans.localPosition, nextPos) <= 0.1)
        {
            // Venter 3 sekunder
            StartCoroutine(ElevatorWait());
            ChangeDestination();
        }
    }
    // Endrer destinasjon
    private void ChangeDestination()
    {
        nextPos = nextPos != startPos ? startPos : endPos;
    }

    // Venter 3 sekunder, dette er nå heisen når ett av punktene.
    // (For å gi spilleren tid til å stige av/på)
    IEnumerator ElevatorWait()
    {
        yield return new WaitForSeconds(3f);
        
    }


    // Holder spilleren på plattformen ved å sette plattform til parent
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(trans);
        }
    }
    // Fjerner parent
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(null);
        }
    }
}
