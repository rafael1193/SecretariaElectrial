/*
 * Secretaría Electrial
 * Copyright (C) 2013 Rafael Bailón-Ruiz <rafaebailon@ieee.org>
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
using SecretariaElectrial.FileSystem;

namespace SecretariaElectrial
{
    public partial class Preferences : Gtk.Dialog
    {
		Registry registry;
		SettingsManager settings;

		public Preferences(SettingsManager settings, Registry reg)
        {
            this.Build();
			this.settings = settings;
			registry = reg;
			if(settings.ExistsKey(SettingsManager.PresetKeys.LastFileSystem.ToString()))
			{
				pathFileChooserButton.SetFilename(settings.Get(SettingsManager.PresetKeys.LastFileSystem.ToString()));
			}
        }

		protected void OnPathFileChooserButtonSelectionChanged(object sender, EventArgs e)
		{
			if (pathFileChooserButton.Filename != null)
			{
				registry = Registry.LoadFrom(pathFileChooserButton.Filename);
				settings.Set(SettingsManager.PresetKeys.LastFileSystem.ToString(), pathFileChooserButton.Filename);
			}
		}
    }
}

