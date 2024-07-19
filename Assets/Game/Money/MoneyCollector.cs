using UnityEngine;

public class MoneyCollector : MonoBehaviour
{
    public AudioClip collectSound; // AudioClip da riprodurre quando il money viene collezionato

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger with player detected.");

            // Aumenta il valore di money nei PlayerPrefs
            int currentMoney = PlayerPrefs.GetInt("PlayerMoney", 0); // Legge il valore attuale, 0 è il valore di default
            currentMoney += 1; // Incrementa il valore
            PlayerPrefs.SetInt("PlayerMoney", currentMoney); // Salva il nuovo valore
            PlayerPrefs.Save(); // Salva effettivamente le modifiche

            Debug.Log("Money collected. Current money: " + currentMoney);

            // Riproduci il suono tramite un oggetto dedicato
            AudioManager.Instance.PlayCollectSound(collectSound);

            Destroy(gameObject);
        }
    }
}
