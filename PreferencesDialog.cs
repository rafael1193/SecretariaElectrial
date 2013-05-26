using System;
using System.Collections.Generic;

namespace SecretariaDataBase
{
    public partial class PreferencesDialog : Gtk.Dialog
    {
        SortedList <string, List<SecretariaDataBase.FileSystem.Box>> boxList;
        Dictionary<string,string> preferences;

        public SortedList <string, List<SecretariaDataBase.FileSystem.Box>> BoxList
        {
            get{ return boxList;}
        }

        public PreferencesDialog(MainWindow parent)
        {
            this.Build();

            preferences = parent.Preferences;

            if (preferences.ContainsKey(ConfigKeys.LastFileSystem.ToString()))
            {
                filechooserbutton1.SetFilename(preferences [ConfigKeys.LastFileSystem.ToString()]);
            }
        }

        protected void OnButtonOkClicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(filechooserbutton1.Filename))
                {
                    boxList = SecretariaDataBase.FileSystem.IO.ReadFilesystem(filechooserbutton1.Filename);

                    if (!preferences.ContainsKey(ConfigKeys.LastFileSystem.ToString()))
                    {
                        preferences.Add(ConfigKeys.LastFileSystem.ToString(), filechooserbutton1.Filename);
                    } else
                    {
                        preferences [ConfigKeys.LastFileSystem.ToString()] = filechooserbutton1.Filename;
                    }

                    Respond(Gtk.ResponseType.Ok);
                }
            } 
//            catch (Exception ex)
//            {
//
//            }
            finally
            {
            }

        }

    }
}

