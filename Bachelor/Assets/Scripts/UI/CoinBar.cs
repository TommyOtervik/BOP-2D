using TMPro;
using UnityEngine;

public class CoinBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;
    private int coinAmount;
    void Start()
    {
        coinAmount = 0;
    }

    void addCoin(int value)
    {
        coinAmount += value;
        textField.SetText(coinAmount.ToString());
    }

    private void OnEnable()
    {
        PickupBroker.CoinPickupEvent += addCoin;
    }

    private void OnDisable()
    {
        PickupBroker.CoinPickupEvent -= addCoin;
    }
}
