using UnityEngine;

public class Scroll : MonoBehaviour
{
    public GameObject[] gameObjects;
    private int currentIndex = 0;
    private ShopController shopController; // Riferimento al ShopController

    void Start()
    {
        // Ottieni il riferimento al ShopController
        shopController = FindObjectOfType<ShopController>();

        ShowCurrentObject();
    }

    public void NextObject()
    {
        currentIndex = (currentIndex + 1) % gameObjects.Length;
        ShowCurrentObject();
        NotifyShopController(); // Notifica il ShopController del cambiamento
    }

    public void PreviousObject()
    {
        currentIndex = (currentIndex - 1 + gameObjects.Length) % gameObjects.Length;
        ShowCurrentObject();
        NotifyShopController(); // Notifica il ShopController del cambiamento
    }

    private void NotifyShopController()
    {
        // Assicurati che il ShopController sappia quando cambi auto
        ShopController shopController = FindObjectOfType<ShopController>();
        if (shopController != null)
        {
            shopController.CheckBuyButton();
        }
    }


    public GameObject GetCurrentObject()
    {
        // Restituisci l'oggetto corrente
        return gameObjects[currentIndex];
    }

    private void ShowCurrentObject()
    {
        // Nascondi tutti gli oggetti
        foreach (GameObject obj in gameObjects)
        {
            obj.SetActive(false);
        }

        // Mostra l'oggetto corrente
        gameObjects[currentIndex].SetActive(true);

        // Aggiorna il pulsante buy nel ShopController
        if (shopController != null)
        {
            shopController.CheckBuyButton();
        }
    }

    public void unselectAll()
    {
        foreach (GameObject obj in gameObjects)
        {
            CarAttributes macchina = obj.GetComponent<CarAttributes>();
            macchina.setSelected(0);
        }
    }
}
