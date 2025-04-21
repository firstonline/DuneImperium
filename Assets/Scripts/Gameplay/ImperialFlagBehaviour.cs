using System.Collections;
using UnityEngine;


public class ImperialFlagBehaviour : MonoBehaviour
{
    public void Setup(ExchangeDefinition exchange)
    {
        if (exchange.Rewards.Count == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            var gameElementBehaviour = GetComponentInChildren<ExchangeBehaviour>();

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

        EditorUtils.SetDirty(gameObject);
    }
}
