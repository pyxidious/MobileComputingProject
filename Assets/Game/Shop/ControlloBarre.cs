using UnityEngine;
using UnityEngine.UI;

public class GestioneBarre : MonoBehaviour
{
    public Image healthbarImage;
    public Image speedbarImage;
    public Image rotatebarImage;
    public Image damagebarImage;
    private Scroll scrollScript;
    public Text nomeMacchina;
    public Text prezzoMacchina;
    public GameObject prezzoMacchinaObj;
    public GameObject locked;
    public GameObject selected;
    public CarAttributes macchina;

    void Start()
    {
        TrovaOggetti();
    }

    void TrovaOggetti()
    {
        scrollScript = GameObject.Find("Canvas").GetComponent<Scroll>();

        GameObject nomeMacchinaObj = GameObject.Find("Canvas/BG/Informazioni/Nome");
        if (nomeMacchinaObj != null)
        {
            nomeMacchina = nomeMacchinaObj.GetComponent<Text>();
        }
        else
        {
            Debug.LogError("Oggetto Nome non trovato.");
        }

        prezzoMacchinaObj = GameObject.Find("Canvas/BG/Informazioni/Costo");
        if (prezzoMacchinaObj != null)
        {
            prezzoMacchina = prezzoMacchinaObj.GetComponent<Text>();
        }
        else
        {
            Debug.LogError("Oggetto Costo non trovato.");
        }

        selected = GameObject.Find("Canvas/BG/Select");

        locked = GameObject.Find("Canvas/BG/Lock");

        GameObject healthbar = GameObject.Find("Canvas/BG/Bars/Health/Full");
        healthbarImage = healthbar.GetComponent<Image>();

        GameObject speedbar = GameObject.Find("Canvas/BG/Bars/Speed/Full");
        speedbarImage = speedbar.GetComponent<Image>();

        GameObject rotatebar = GameObject.Find("Canvas/BG/Bars/RotateSpeed/Full");
        rotatebarImage = rotatebar.GetComponent<Image>();

        GameObject damagebar = GameObject.Find("Canvas/BG/Bars/Damage/Full");
        damagebarImage = damagebar.GetComponent<Image>();
    }

    void Update()
    {
        if (scrollScript != null)
        {
            macchina = scrollScript.GetCurrentObject().GetComponent<CarAttributes>();

            if (macchina != null)
            {
                healthbarImage.fillAmount = macchina.GetHP() / 100f;
                speedbarImage.fillAmount = macchina.GetSpeed() / 100f;
                rotatebarImage.fillAmount = macchina.GetRotate() / 100f;
                damagebarImage.fillAmount = macchina.GetDamage() / 100f;

                if (nomeMacchina != null)
                {
                    nomeMacchina.text = macchina.GetName();
                }

            }
            else
            {
                Debug.LogError("Componente CarAttributes non trovato sull'oggetto corrente.");
            }
        }
        else
        {
            Debug.LogError("Componente Scroll non trovato sull'oggetto Canvas.");
        }
        ControllaStatoLock();
        isSelected();
    }

    void ControllaStatoLock()
    {
        if (prezzoMacchina != null && macchina != null && macchina.isUnlocked() != 1)
        {
            locked.SetActive(true);
            prezzoMacchina.text = macchina.GetPrezzo();
            prezzoMacchinaObj.SetActive(true);  // Mostra l'oggetto se la macchina è sbloccata
        }
        else
        {
            // Nascondi l'oggetto se prezzoMacchina non è valido o la macchina è bloccata
            prezzoMacchinaObj.SetActive(false);
            locked.SetActive(false);
        }
    }

    void isSelected()
    {
        if (macchina != null && macchina.isUnlocked() != 0 && macchina.isSelected() != 1)
        {
            selected.SetActive(true);
        }
        else
        {
            // Nascondi l'oggetto se prezzoMacchina non è valido o la macchina è bloccata
            selected.SetActive(false);
        }
    }


    public void Select() 
    {
        // Ottieni un riferimento all'ObjectManager nell'oggetto Variabili Globali
        ObjectManager objectManager = GameObject.FindObjectOfType<ObjectManager>();

        // Verifica che l'ObjectManager sia stato trovato
        if (objectManager != null) 
        {
            // Imposta il campo NomeMacchina sull'ObjectManager con il nome della macchina corrente
            objectManager.nomeMacchina = nomeMacchina.text;
            objectManager.speed = macchina.GetSpeed();
            objectManager.rotateSpeed = macchina.GetRotate();
            objectManager.hurtValue = macchina.GetDamage();
            objectManager.HP = macchina.GetHP();
        }
        else 
        {
            Debug.LogError("ObjectManager non trovato in Variabili Globali.");
        }
        scrollScript.unselectAll();
        macchina.setSelected(1);
    }
}
