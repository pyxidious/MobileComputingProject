using UnityEngine;

public class CarAttributes : MonoBehaviour
{
    // Variabili pubbliche modificabili nell'editor
    public float Speed = 10f;
    public float RotateSpeed = 5f;
    public float HP = 100f;
    public float HurtValue = 20f;
    public string Nome = "";
    public int Prezzo = 0;
    public int unlocked = 0;
    public int selected = 0;

    public float GetSpeed()
    {
        // Restituisci la speed
        return this.Speed;
    }

    public float GetRotate()
    {
        // Restituisci la rs
        return this.RotateSpeed;
    }

    public float GetHP()
    {
        // Restituisci gli HP
        return this.HP;
    }

    public float GetDamage()
    {
        // Restituisci il damage
        return this.HurtValue;
    }

    public string GetName()
    {
        // Restituisci il nome
        return this.Nome;
    }

    public string GetPrezzo()
    {
        // Restituisci il prezzo
        return this.Prezzo.ToString();
    }

    public int isUnlocked()
    {
        // Controlla se la macchina e' sbloccata
        return this.unlocked;
    }

    public int isSelected()
    {
        // Controlla se la macchina e' sbloccata
        return this.selected;
    }

    public void setSelected(int select)
    {
        this.selected = select;
    }
}
