using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UnityCipher
{
    /// <summary>
    /// A component to add to a dropdown (TMP) gameobject for a
    /// language selection dropdown.
    ///
    /// Populates the dropdown with the available languages and updates the
    /// localizer's language.
    /// </summary>
    public class LangTMPDropdown : MonoBehaviour
    {
        TMP_Dropdown dropdown;

        private void Start()
        {
            dropdown = GetComponent<TMP_Dropdown>();
            dropdown.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>();
            foreach (string lang in Localizer.instance.GetAvailLanguages())
            {
                options.Add(new TMP_Dropdown.OptionData(lang));
            }
            dropdown.AddOptions(options);

            var loc = Localizer.instance;
            dropdown.value = loc.GetAvailLanguages().IndexOf(loc.CurLang);

            dropdown.onValueChanged.AddListener(OnLangChanged);
        }

        void OnLangChanged(int val)
        {
            var loc = Localizer.instance;
            loc.SetLanguage(loc.GetAvailLanguages()[val]);
        }
    }
}
