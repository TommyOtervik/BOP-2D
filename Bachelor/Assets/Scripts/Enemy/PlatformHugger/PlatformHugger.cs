using System.Numerics;
using UnityEngine;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlatformHugger : MonoBehaviour
    {
        [SerializeField] private Transform[] positions;
        [SerializeField] private float objectSpeed;
        private int _nextPosIndex;
        private Transform _nextPos;
        
        [SerializeField] private int collisionDamageAmount = 10;
        private const string PLAYER_NAME = "Player";

        
        // Rotasjonstesting
        private float distanceToPoint; // This will store the remaining distance between player and _nextPos.transform
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
                // Rotasjonstest
                Rotate();
                
                // Originalet
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
        
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
        
            if (collision.gameObject.name.Equals(PLAYER_NAME))
            {
                DamageBroker.CallTakeDamageEvent(collisionDamageAmount);
            }

        }
        // Viktig info, alle posisjoner må ha en -90 Z rotasjon.
        // Sett i tillegg startposisjon til objektet sånn at han løper til høyre mot punkt 1 i positions array. Visst ikke blir det rotasjonskrøll.
        // 4    ----------------     1
        // 3    ----------------     2
        
        private void Rotate()
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.z += positions[_nextPosIndex].transform.eulerAngles.z;
            transform.eulerAngles = currentRotation;
        }

    }
