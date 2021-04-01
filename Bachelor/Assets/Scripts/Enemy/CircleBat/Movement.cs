
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Transform[] positions;
    [SerializeField] private float objectSpeed;
    private int _nextPosIndex;
    private Transform _nextPos;

    void Start()
    {
        _nextPos = positions[0];
    }

    void Update()
    {
        MoveGameObject();
    }

    void MoveGameObject()
    {
        if (transform.position == _nextPos.position)
        {
            _nextPosIndex++;
            if (_nextPosIndex >= positions.Length)
            {
                _nextPosIndex = 0;
            }

            _nextPos = positions[_nextPosIndex];
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, _nextPos.position, objectSpeed * Time.deltaTime);
            
        }
    }
}
