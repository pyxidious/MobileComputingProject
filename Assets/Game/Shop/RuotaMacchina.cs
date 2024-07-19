using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 50f; // Velocità di rotazione dell'oggetto

    void Update()
    {
        // Ruota l'oggetto lungo l'asse Y
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
