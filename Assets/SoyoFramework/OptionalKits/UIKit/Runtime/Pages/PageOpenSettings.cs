using System;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Pages
{
    [Serializable]
    public class PageOpenSettings
    {
        public bool NotCallOnInit;

        public PageOpenSettings()
        {
            NotCallOnInit = false;
        }

        public PageOpenSettings(bool notCallOnInit)
        {
            NotCallOnInit = notCallOnInit;
        }
    }
}