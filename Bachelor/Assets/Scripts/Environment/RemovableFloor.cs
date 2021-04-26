using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovableFloor : MonoBehaviour, ICanBeSetInactive
{
    [SerializeField]
    private string objectName;

    private Rigidbody2D floor;
    private BoxCollider2D boxCollider;

    private Vector3 bounds;
    private Vector3 startPos;

    private bool isOpen;


    // Start is called before the first frame update
    void Start()
    {

        if (GameManager.IsFloorOpen())
            Destroy(this.gameObject);

        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        bounds = startPos + new Vector3(0, -20, 0);

        floor = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {

        if (this.gameObject.transform.position.y < bounds.y)
            Destroy(this.gameObject);
    }

    public void Open()
    {
        if (!isOpen)
        {
            floor.constraints = RigidbodyConstraints2D.None;
            floor.bodyType = RigidbodyType2D.Dynamic;

            StartCoroutine(WaitForDisable());

            isOpen = true;

            GameManager.PlayerOpenedFloor(isOpen);
        }
    }

    IEnumerator WaitForDisable()
    {
        yield return new WaitForSeconds(.5f);
        boxCollider.enabled = false;
    }

    public string GetObjectName()
    {
        return objectName;
    }
}
