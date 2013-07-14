using System;
using System.Collections.Generic;

namespace SecretariaDataBase
{
    public partial class PreferencesDialog : Gtk.Dialog
    {
        SortedList <string, List<SecretariaDataBase.FileSystem.Box>> boxList;
        SettingsManager settings;
		public bool canCancel = true;

        public SortedList <string, List<SecretariaDataBase.FileSystem.Box>> BoxList
        {
            get{ return boxList;}
        }

        public PreferencesDialog (MainWindow parent, bool firstRun)
		{
			this.Build ();

			canCancel = !firstRun;

			settings = parent.settings;
			if (canCancel == false) {
				buttonCancel.Sensitive = false;
			}

            if (settings.ExistsKey(SettingsManager.PresetKeys.LastFileSystem.ToString()))
            {
                filechooserbutton1.SetFilename(settings.Get (SettingsManager.PresetKeys.LastFileSystem.ToString()));
            }
        }

        protected void OnButtonOkClicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(filechooserbutton1.Filename))
                {
					settings.Set(SettingsManager.PresetKeys.LastFileSystem.ToString(), filechooserbutton1.Filename);
					boxList = SecretariaDataBase.FileSystem.IO.ReadFilesystem (settings.Get (SettingsManager.PresetKeys.LastFileSystem.ToString ()));
                    Respond(Gtk.ResponseType.Ok);
                }
            } 
            catch (Exception ex)
            {

            }
        }
    }
}

