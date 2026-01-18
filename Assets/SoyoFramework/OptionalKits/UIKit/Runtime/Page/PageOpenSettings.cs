using System;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Page
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