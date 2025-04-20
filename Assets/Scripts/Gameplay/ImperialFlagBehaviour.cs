using System.Collections;
using UnityEngine;


[RequireComponent(typeof(ExchangeBehaviour))]
public class ImperialFlagBehaviour : MonoBehaviour
{
    public void Setup(ExchangeDefinition exchange)
    {
        var gameElementBehaviour = GetComponent<ExchangeBehaviour>();


        if (exchange.Rewards.Count > 0)
        {
            gameObject.SetActive(true);
            gameElementBehaviour.Setup(exchange);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
