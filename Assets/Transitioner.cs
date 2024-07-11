using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class IncreaseOpacity : MonoBehaviour
{
    public Image i; // Utilizza 'i' invece di 'image'
    public float duration = 1f; // Durata dell'animazione in secondi
    public int steps = 100; // Numero di passaggi per l'animazione

    void Start()
    {
    }

    public void StartOpacity(){
        StartCoroutine(IncreaseOpacityOverTime()); // Avvia il metodo come coroutine  
    }

    IEnumerator IncreaseOpacityOverTime()
    {
        // Calcola l'incremento di opacità per ogni passaggio
        float opacityIncrement = 1f / steps;

        // Esegui l'animazione per il numero di passaggi specificato
        for (int j = 0; j <= steps; j++) // Usa un nome diverso per l'indice del ciclo
        {
            // Calcola l'opacità attuale basata sull'incremento e sull'indice corrente
            float currentOpacity = opacityIncrement * j;

            // Imposta l'opacità dell'immagine
            Color color = i.color; // Usa 'i' invece di 'image'
            color.a = currentOpacity;
            i.color = color; // Usa 'i' invece di 'image'

            // Attendi un frame prima di passare al passaggio successivo
            float timePerStep = duration / steps;
            float elapsedTime = 0f;
            while (elapsedTime < timePerStep)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        // Assicura che l'opacità sia esattamente quella massima
        Color finalColor = i.color; // Usa 'i' invece di 'image'
        finalColor.a = 1f;
        i.color = finalColor; // Usa 'i' invece di 'image'
        SceneManager.LoadScene(0);
    }
}
