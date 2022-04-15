using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnityCipher
{
    public class Translator
    {
        /// <summary>
        /// Each key is a language, and
        /// each list is the texts translated in that language. The lists are all
        /// aligned, so item `i` of list `a` is a translation of item `i` of
        /// list `b`
        /// </summary>
        Dictionary<string, List<string>> translations;

        /// <summary>
        /// Creates a new translator from the source csv text
        /// </summary>
        /// <param name="csvText">
        /// The contents of a csv file with the translation info.
        /// Each row is a list of translations for the same text, with quotes.
        /// Each collumn is a language.
        /// The header row contains the culture codes of each language,
        /// without quotes.
        /// </param>
        public Translator(string csvText)
        {
            if (csvText == null)
                throw new ArgumentException("csvText cannot be null!");

            string[] lines = csvText.Split("\n");
            if (lines.Length == 0)
                throw new System.Exception("Source text cannot be empty!");

            translations = new Dictionary<string, List<string>>();
            // header
            string[] langs = Regex.Replace(lines[0], @"\s*", "").Split(",");
            foreach (string lang in langs)
                translations.Add(lang, new List<string>());

            // entries
            for (int i = 1; i < lines.Length; i++)
            {
                MatchCollection matches = Regex.Matches(lines[i],
                            @"(?:\""([^\""\n]*)\""|([^,\""\n]*))\s*(?:,\s*|\n|$)");
                for (int j = 0; j < langs.Length; j++)
                {
                    string text = "";
                    if (matches.Count > j)
                        text = matches[j].Groups[1].Value;

                    translations[langs[j]].Add(text);
                }
            }
        }

        /// <summary>
        /// Translates a text from one language to another
        /// </summary>
        /// <param name="fromText">The original text to translate</param>
        /// <param name="fromLang">The culture code of the origin language</param>
        /// <param name="toLang">The culture code of the language to translate
        /// to</param>
        /// <returns>The translated text</returns>
        public string Translate(string fromText, string fromLang, string toLang)
        {
            if (!translations.ContainsKey(fromLang))
                throw new ArgumentException($"Language {fromLang} is not found in " +
                    "the data");
            if (!translations.ContainsKey(toLang))
                throw new ArgumentException($"Language {toLang} is not found in " +
                    "the data");

            int entryIndex = translations[fromLang].IndexOf(fromText);
            if (entryIndex == -1)
                throw new ArgumentException($"No entry was found in the " +
                                $"translations matching `fromText` {fromText}");
            return translations[toLang][entryIndex];
        }

        /// <summary>
        /// Get the text in csv format of this translator. Usefull for saving
        /// translation info.
        /// </summary>
        /// <returns>The translations in csv format</returns>
        public string GetCSVText()
        {
            List<string> langs = new List<string>(translations.Keys);
            if (langs.Count == 0)
                return "";

            // header
            string text = "";
            for (int i = 0; i < langs.Count; i++)
            {
                text += langs[i];
                if (i < langs.Count - 1)
                    text += ",";
            }

            // entires
            int entryCount = translations[langs[0]].Count;
            for (int r = 0; r < entryCount; r++)
            {
                for (int c = 0; c < langs.Count; c++)
                {
                    string lang = langs[c];
                    text += translations[lang][r];
                    if (c < langs.Count - 1)
                        text += ",";
                }
                text += "\n";
            }

            return text;
        }

        /// <summary>
        /// Gets an array of available languages
        /// </summary>
        /// <returns>An array of culture codes for the available languges</returns>
        public List<string> GetAvailableLangs()
        {
            return new List<string>(translations.Keys);
        }
    }
}