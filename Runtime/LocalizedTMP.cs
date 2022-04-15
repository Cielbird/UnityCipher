using TMPro;

namespace UnityCipher
{
    /// <summary>
    /// An extention of TextMeshProUGUI that can be localized.
    /// </summary>
    public class LocalizedTMP : TextMeshProUGUI
    {
        // unimplemented. this would only be usefull with an editor for this
        // class, and it's only necesarry if you want to temporaraly disable
        // localization for one single tmp text. leaving it here for later.
        public bool isLocalized = true;
    }
}
