using UnityEngine;
using UnityEngine.UI;

public class ScrollingObjects : MonoBehaviour
{
    public GameObject[] gameObjects;
    private int currentIndex = 0;

    void Start()
    {
        ShowCurrentObject();
    }

    public void NextObject()
    {
        Debug.Log("NextObject called");
        currentIndex = (currentIndex + 1) % gameObjects.Length;
        ShowCurrentObject();
    }

    public void PreviousObject()
    {
        Debug.Log("PrevObject called");
        currentIndex = (currentIndex - 1 + gameObjects.Length) % gameObjects.Length;
        ShowCurrentObject();
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
}
