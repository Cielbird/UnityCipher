using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityCipher
{
    /// <summary>
    /// A monobeheviour to add to any scene to support localization.
    ///
    /// Loads the localization data from `source` and updates any field in
    /// the scene marked with [Localized].
    /// </summary>
    public class Localizer : MonoBehaviour
    {
        public static Localizer instance;

        /// <summary>
        /// A csv source file with the following format:
        ///
        /// Each collumn is a language, each row is an entry (a string
        /// field/property marked with [Localized]).
        ///
        /// The header contains the culture codes (en-US by default) for each
        /// language.
        ///
        /// If the entry is in quotes, the commas are ignored. Use a csv
        /// highlighter or export from a spreadsheet editor for easy use.
        /// See examples.
        /// </summary>
        public TextAsset source;
        public string defaultLang;
        public bool localizeTMP;


        Translator translator;
        string curLang;
        public string CurLang
        {
            get { return curLang; }
        }

        private void Awake()
        {
            if (instance != null)
                Destroy(gameObject);
            instance = this;

            translator = new Translator(source.text);

            curLang = defaultLang;
        }

        /// <summary>
        /// Returns a list of culture codes in the order they appear in the
        /// source csv.
        /// </summary>
        public List<string> GetAvailLanguages()
        {
            return translator.GetAvailableLangs();
        }

        /// <summary>
        /// Translates the game using `source` to the given language.
        /// </summary>
        /// <param name="lang">The culture code of the new language to
        /// translate to</param>
        public void SetLanguage(string lang)
        {
            TranslateAll(lang);
            curLang = lang;
        }

        /// <summary>
        /// Uses reflection to find all fields and properties marked with
        /// [Localized], and sets them to the translation (from `curLang`
        /// to `lang`) if a translation is found.
        /// If no matching translation is found, a unity warning is thrown.
        /// If the translation is null, the field or property is left
        /// untouched.
        /// </summary>
        /// <param name="toLang"></param>
        public void TranslateAll(string toLang)
        {
            Object[] all = FindObjectsOfType(typeof(Object));
            foreach (var obj in all)
            {
                // TMP
                if(localizeTMP && obj is LocalizedTMP)
                {
                    TranslateTMPText((LocalizedTMP)obj, curLang, toLang);
                    continue;
                }

                var type = obj.GetType();
                var fields = type.GetFields();
                var properties = type.GetProperties(BindingFlags.Instance
                                                  | BindingFlags.NonPublic);
                
                foreach (FieldInfo field in fields)
                {
                    bool isLocalized = field.GetCustomAttributes(false)
                                            .Any(a => a is Localized);
                    if (field.FieldType != typeof(string) || !isLocalized)
                        continue;

                    TranslateField(field, obj, curLang, toLang);
                }
                foreach (PropertyInfo prop in properties)
                {
                    bool isLocalized = prop.GetCustomAttributes(false)
                                           .Any(a => a is Localized);
                    if (prop.PropertyType != typeof(string) || !isLocalized)
                        continue;

                    TranslateProperty(prop, obj, curLang, toLang);
                }
            }

        }

        /// <summary>
        /// Translates the given field to the given language
        /// </summary>
        void TranslateField(FieldInfo field, Object obj,
                            string fromLang, string toLang)
        {
            string original = "[Not a string]";
            try
            {
                original = (string)field.GetValue(obj);
                string trans = translator.Translate(original, fromLang, toLang);
                field.SetValue(obj, trans);
            }
            catch
            {
                Debug.LogWarning($"The following field couldn't be " +
                    $"translated: {original}({field})\n" +
                    $"Run AddNewFieldsToSource() to remove this " +
                    $"message and for help adding translations.");
            }
        }

        /// <summary>
        /// Translates the given property to the given language
        /// </summary>
        void TranslateProperty(PropertyInfo prop, Object obj,
                               string fromLang, string toLang)
        {
            string original = "[Not a string]";
            try
            {
                original = (string)prop.GetValue(obj);
                string trans = translator.Translate(original, fromLang, toLang);
                prop.SetValue(obj, trans);
            }
            catch
            {
                Debug.LogWarning($"The following property couldn't be " +
                    $"translated: {original}({prop})\n" +
                    $"Run AddNewFieldsToSource() to remove this " +
                    $"message and for help adding translations.");
            }
        }

        void TranslateTMPText(LocalizedTMP tmp, string fromLang, string toLang)
        {
            string original = tmp.text;
            string trans = "";
            try
            {
                trans = translator.Translate(original, fromLang, toLang);
            }
            catch
            {
                Debug.LogWarning($"The following TMP text couldn't be " +
                    $"translated: {original}({tmp})\n" +
                    $"Run AddNewFieldsToSource() to remove this " +
                    $"message and for help adding translations.");
            }
            tmp.text = trans;
        }

        public void AddNewFieldsToSource()
        {
            List<string> untranslatedFields = new List<string>();
            Object[] all = FindObjectsOfType(typeof(Object));
            foreach (var obj in all)
            {
                var fields = from f in obj.GetType().GetFields()
                             where f.GetCustomAttributes(false).Any(a => a is Localized)
                                && f.FieldType == typeof(string)
                             select f;

                foreach (FieldInfo field in fields)
                {
                    string original = (string)field.GetValue(obj);
                    untranslatedFields.Add(original);
                }
            }
            throw new System.NotImplementedException("still needs to be " +
                                                    "written to the source");
        }
    }
}
