using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Singleton-mønster. Det er 
// bare én forekomst av denne klassen i spillscenen.
// Dette skriptet er festet til et tomt objekt i scenen. Man abonnerer på handligen
// når skriptet aktiveres (i begynnelsen av spillet).
// Og vi avslutter abonnementet for å optimalisere ytelsen.
// For å utløse en hendelse trenger vi bare en linje med kode (TriggerEvent)
public class EventManager : MonoBehaviour
{
    // Dictionary med Enum som key og UnityEvent som value.
    private Dictionary<EnumEvents, UnityEvent> eventDictionary;

    private static EventManager eventManager;

    private void Awake()
    {
        // Ikke ødelegg skriptet mellom load
        DontDestroyOnLoad(this);
        // Er det flere enn ett av dette objeketet, slett det.
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    // Instance av EventManager
    public static EventManager instance
    {
        get
        {
            // Hvis EventManager ikke finnes
            if (!eventManager)
            {
                // I scenen, på hvilket som helst spillobjekt, finn den første forekomsten av EventManager og returner en referenase.
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                // Finnes ikke denne send Err, eller Init() -> Lage ny Dictionary.
                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    void Init()
    {
        // Hvis den ikke finnes, opprett ny.
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<EnumEvents, UnityEvent>();
        }
    }

    // Start å lytt basert på Enum og UnityAction
    public static void StartListening(EnumEvents eventName, UnityAction listener)
    {
        // Prøv å få tak event. Finnes den, legg til lytter.
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        // Hvis ikke, opprett nytt event og legg til lytter + legg den i eventDictionary
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    // Slutt å lytte basert på Enum og UnityAction
    public static void StopListening(EnumEvents eventName, UnityAction listener)
    {
        // Hvis denne ikke finnes, return.
        if (eventManager == null) return;
        // Prøv å hent Event og fjern fra eventDictionary
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    // Trigger event
    public static void TriggerEvent(EnumEvents eventName)
    {
        // Prøv og hent event basert på Enum
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            // Finner man den -> kjør metoden.
            thisEvent.Invoke();
        }
    }

    
}
