using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageDropdown : MonoBehaviour
{
    public Dropdown dropdown;

    void Start()
    {
        // Inizializza il Dropdown con le lingue disponibili
        dropdown.options.Clear();
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            dropdown.options.Add(new Dropdown.OptionData(locale.Identifier.CultureInfo.NativeName));
        }

        // Imposta l'evento di selezione del Dropdown
        dropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    void OnLanguageChanged(int index)
    {
        // Cambia la lingua in base alla selezione dell'utente
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}
