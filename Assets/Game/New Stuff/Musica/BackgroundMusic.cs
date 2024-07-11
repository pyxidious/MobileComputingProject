using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public float volume = 0.5f; // Volume desiderato (da 0 a 1)
    private AudioSource audioSource;

    void Start()
    {
        // Ottieni il riferimento all'AudioSource
        audioSource = GetComponent<AudioSource>();

        // Imposta il volume dell'AudioSource
        audioSource.volume = volume;
        
        // Avvia la riproduzione della musica
        audioSource.Play();
    }
}
