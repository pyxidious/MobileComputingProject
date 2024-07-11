using UnityEngine;

public class Scroll : MonoBehaviour
{
    public GameObject[] gameObjects;
    private int currentIndex = 0;

    void Start()
    {
        ShowCurrentObject();
    }

    public void NextObject()
    {
        currentIndex = (currentIndex + 1) % gameObjects.Length;
        ShowCurrentObject();
    }

    public void PreviousObject()
    {
        currentIndex = (currentIndex - 1 + gameObjects.Length) % gameObjects.Length;
        ShowCurrentObject();
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
    }

    public void unselectAll(){
        foreach (GameObject obj in gameObjects)
        {
            CarAttributes macchina = obj.GetComponent<CarAttributes>();
            macchina.setSelected(0);
        }
    }
}
