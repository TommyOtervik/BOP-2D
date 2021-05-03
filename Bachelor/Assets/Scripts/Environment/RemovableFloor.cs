using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Dette skirptet håndterer RemovableFloor.
 *  Dette er selve gulvet som kan fjernes, og det blir til en snarvei.
 *   
 * @AOP - 225280
 */
public class RemovableFloor : MonoBehaviour
{
    private Rigidbody2D floor;
    private BoxCollider2D boxCollider;

    private Vector3 bounds;   // Bounds for å slette objektet (faller ned, Y pos)
    private Vector3 startPos;

    private bool isOpen;


    void Start()
    {

        if (GameManager.IsFloorOpen())
            Destroy(this.gameObject);

        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        bounds = startPos + new Vector3(0, -20, 0);

        floor = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

    }

    void Update()
    {
        // Sjekker om gulvet har falt ned til en viss y pos -> slett det
        if (this.gameObject.transform.position.y < bounds.y)
            Destroy(this.gameObject);
    }

    // "Åpner" gulvet
    public void Open()
    {
        if (!isOpen)
        {
            // Deaktiverer fysiske egenskaper for å få det til å falle
            floor.constraints = RigidbodyConstraints2D.None;
            floor.bodyType = RigidbodyType2D.Dynamic;

            StartCoroutine(WaitForDisable());

            isOpen = true;

            // Registrer dette i GameManager
            GameManager.PlayerOpenedFloor(isOpen);
        }
    }
    
    IEnumerator WaitForDisable()
    {
        yield return new WaitForSeconds(.5f);
        boxCollider.enabled = false;
    }

   
}
