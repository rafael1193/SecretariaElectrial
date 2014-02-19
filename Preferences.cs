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
			if(settings.ExistsKey(SettingsManager.PresetKeys.ReadOnlyMode.ToString()))
			{
				readonlyCheckbutton.Active = Convert.ToBoolean(settings.Get(SettingsManager.PresetKeys.ReadOnlyMode.ToString()));
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

		protected void OnReadonlyCheckbuttonToggled(object sender, EventArgs e)
		{
			settings.Set(SettingsManager.PresetKeys.ReadOnlyMode.ToString(), readonlyCheckbutton.Active.ToString());
		}


		protected void OnButtonCloseClicked(object sender, EventArgs e)
		{
			//Check if preferences are valid
			if (pathFileChooserButton.Filename != null)
			{
				this.Respond(Gtk.ResponseType.Close);
			}
			else
			{
				Gtk.MessageDialog mes = new Gtk.MessageDialog(this, Gtk.DialogFlags.Modal, Gtk.MessageType.Info, Gtk.ButtonsType.Ok, Mono.Unix.Catalog.GetString("You must choose a path for registry files"));
				if ((Gtk.ResponseType)mes.Run() == Gtk.ResponseType.Ok)
				{
					mes.Destroy();
				}
			}

		}
    }
}

