/*
 * Secretaría Electrial
 * Copyright (C) 2013  Rafael Bailón-Ruiz <rafaebailon@ieee.org>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

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

