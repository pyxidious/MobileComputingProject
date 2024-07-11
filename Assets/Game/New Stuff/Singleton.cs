using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    // Variabili dell'oggetto che si desidera condividere tra le scene
    public string nomeMacchina;
    public float speed;
    public float rotateSpeed;
    public float HP;
    public float hurtValue;
    public float money;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float getSpeed(){
        return speed;
    }

    public float getHP(){
        return HP;
    }

    public float getDamage(){
        return hurtValue;
    }

    public float getRotate(){
        return rotateSpeed;
    }

    public float getMoney(){
        return money;
    }
}