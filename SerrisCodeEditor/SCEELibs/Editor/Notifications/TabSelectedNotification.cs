﻿namespace SCEELibs.Editor.Notifications
{
    public enum ContactTypeSCEE
    {
        GetCodeForTab,
        SetCodeForEditor,
        SetCodeForEditorWithoutUpdate
    }

    public sealed class TabSelectedNotification
    {
        public int tabID { get; set; }
        public int tabsListID { get; set; }

        public ContactTypeSCEE contactType { get; set; }
        public string code { get; set; }
        public string typeCode { get; set; }
        public string typeLanguage { get; set; }
    }
}
