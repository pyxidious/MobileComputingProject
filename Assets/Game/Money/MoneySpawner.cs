using UnityEngine;

public class MoneySpawner : MonoBehaviour
{
    public GameObject moneyPrefab; // Prefab dell'oggetto da spawnare
    public float spawnInterval = 3f; // Intervallo di tempo tra uno spawn e l'altro
    public float spawnRangeX = 5f; // Range orizzontale sull'asse X
    public float spawnRangeZ = 5f; // Range orizzontale sull'asse Z
    public int maxMoneyAllowed = 5; // Numero massimo di money consentiti per questo spawner

    private float timer; // Timer per tenere traccia del tempo

    void Start()
    {
        timer = spawnInterval; // Inizializza il timer al valore dell'intervallo per farlo spawnare subito all'avvio
    }

    void Update()
    {
        timer -= Time.deltaTime; // Decrementa il timer ogni frame

        // Conta quanti money sono attualmente presenti
        int currentMoneyCount = CountSpawnedMoney();

        // Spawn solo se il numero di money attualmente presenti è inferiore al massimo consentito
        if (timer <= 0 && currentMoneyCount < maxMoneyAllowed)
        {
            SpawnMoney(); // Chiamiamo il metodo di spawn solo se non abbiamo raggiunto il numero massimo di money
            timer = spawnInterval; // Resettiamo il timer all'intervallo di spawn
        }
    }

    void SpawnMoney()
    {
        // Calcola una posizione casuale all'interno del range specificato
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        float randomZ = Random.Range(-spawnRangeZ, spawnRangeZ);
        Vector3 spawnPosition = transform.position + new Vector3(randomX, 0f, randomZ);

        Debug.Log("Spawn position: " + spawnPosition); // Debug per verificare la posizione di spawn

        // Instantiate del moneyPrefab
        Instantiate(moneyPrefab, spawnPosition, Quaternion.identity);
    }

    int CountSpawnedMoney()
    {
        GameObject[] moneyObjects = GameObject.FindGameObjectsWithTag("Money");

        int count = 0;
        foreach (GameObject moneyObj in moneyObjects)
        {
            // Controlla se l'oggetto moneyObj è nella zona di spawn di questo MoneySpawner
            if (Vector3.Distance(moneyObj.transform.position, transform.position) < spawnRangeX)
            {
                count++;
            }
        }

        return count;
    }
}
