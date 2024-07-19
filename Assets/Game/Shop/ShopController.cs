using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public GameObject coinsNumber; // Assicurati di assegnare questo GameObject dall'editor di Unity
    public GameObject buy; // L'oggetto che vogliamo mostrare/nascondere
    public GameObject canvas; // L'oggetto canvas che contiene la componente "Scroll"

    private Scroll scrollScript; // Riferimento allo script Scroll

    // Start is called before the first frame update
    void Start()
    {
        // Ottieni il valore "money" da PlayerPrefs
        int money = PlayerPrefs.GetInt("PlayerMoney", 0);   
        Debug.Log("Money retrieved from PlayerPrefs: " + money);

        // Trova il componente Text nell'oggetto "Coins Number" e aggiorna il testo
        Text coinsText = coinsNumber.GetComponent<Text>();
        if (coinsText != null)
        {
            coinsText.text = money.ToString();
            Debug.Log("Coins Number text updated: " + coinsText.text);
        }
        else
        {
            Debug.LogError("Text component not found on Coins Number object");
        }

        // Ottieni il componente Scroll dal canvas
        scrollScript = canvas.GetComponent<Scroll>();
        if (scrollScript == null)
        {
            Debug.LogError("Scroll script not found on the canvas");
            return;
        }

        // Controlla se il pulsante buy deve essere mostrato
        CheckBuyButton();
    }

    // Update is called once per frame
    void Update()
    {
        // Questo può essere utilizzato per aggiornamenti dinamici, se necessario
    }

public void CheckBuyButton()
    {
        // Ottieni l'oggetto corrente da Scroll
        GameObject currentCarObject = scrollScript.GetCurrentObject();
        if (currentCarObject == null)
        {
            Debug.LogError("No current car object found in Scroll");
            return;
        }

        CarAttributes carAttributes = currentCarObject.GetComponent<CarAttributes>();
        if (carAttributes == null)
        {
            Debug.LogError("CarAttributes component not found on the current car object");
            return;
        }

        int money = PlayerPrefs.GetInt("PlayerMoney", 0);   
        Debug.Log("Money retrieved from PlayerPrefs: " + money);

        Debug.Log("Current car found: " + currentCarObject.name + " with cost: " + carAttributes.Prezzo);

        // Confronta il valore di money con il costo della Car e verifica se è sbloccata
        if (money >= carAttributes.Prezzo && carAttributes.isUnlocked() == 0)
        {
            buy.SetActive(true);
            Debug.Log("Buy button set to active");
        }
        else
        {
            buy.SetActive(false);
            Debug.Log("Buy button set to inactive");
        }
    }

    public void Buy()
    {
        // Ottieni l'oggetto corrente da Scroll
        GameObject currentCarObject = scrollScript.GetCurrentObject();
        if (currentCarObject == null)
        {
            Debug.LogError("No current car object found in Scroll");
            return;
        }

        CarAttributes carAttributes = currentCarObject.GetComponent<CarAttributes>();
        if (carAttributes == null)
        {
            Debug.LogError("CarAttributes component not found on the current car object");
            return;
        }

        int money = PlayerPrefs.GetInt("PlayerMoney", 0);   

        // Controlla se il giocatore ha abbastanza soldi e se l'auto non è già sbloccata
        if (money >= carAttributes.Prezzo && carAttributes.isUnlocked() == 0)
        {
            // Sblocca l'auto
            carAttributes.Unlock();

            // Sottrai il prezzo dell'auto dai soldi del giocatore
            money -= carAttributes.Prezzo;
            PlayerPrefs.SetInt("PlayerMoney", money);
            PlayerPrefs.Save();

            // Aggiorna il testo dei soldi
            Text coinsText = coinsNumber.GetComponent<Text>();
            if (coinsText != null)
            {
                coinsText.text = money.ToString();
            }

            // Nascondi il pulsante buy
            buy.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Not enough money or car is already unlocked");
        }
    }
}
