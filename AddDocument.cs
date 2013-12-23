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
	public partial class AddDocument : Gtk.Dialog
	{
		Document doc = null;
		Category cat = null;
		Registry registry;

		public AddDocument(Registry reg)
		{
			this.Build();
			rememberLabel.LabelProp = Mono.Unix.Catalog.GetString("Remember you must stamp, sign and scan this document after registering it");
			registry = reg;
			foreach (var cat in registry)
			{
				categoryComboBox.AppendText(cat.ToString());
			}
		}

		/// <summary>
		/// Gets the name with a correct format
		/// </summary>
		/// <returns>Formatted name</returns>
		/// <param name="name">Raw name</param>
		private string GetName(string name)
		{
			string formattedName = name;
			if (name != "")
			{
				formattedName = name.ToLower();
				char[] notValid = System.IO.Path.GetInvalidFileNameChars();
				foreach (char ch in notValid)
				{
					if (name.Contains(ch.ToString()))
					{
						throw new System.ArgumentException(String.Format("Document name can't contain any \"{0}\"", ch.ToString()));
					}
				}
				string[,] substitute = new string[,]{ { "_", "-" }, { "á", "a" }, { "é","e" },  { "í", "i" },  { "ó", "o" }, { "ú", "u" }, { "ü", "u" }, { "ï", "i" } };
				for(int i = 0;i < substitute.GetLength(0);++i)
				{
					formattedName=formattedName.Replace(substitute[i,0], substitute[i,1]);
					//throw new System.ArgumentException(String.Format("Document name can't contain any \"{0}\"", ch.ToString()));
				}
			}
			else
			{
				throw new System.ArgumentException("A document must have a name");
			}

			return formattedName;
		}

		/// <summary>
		/// Gets the id if it is correct
		/// </summary>
		/// <returns>Id as integer</returns>
		/// <param name="strId">Id as string</param>
		private int GetId(Category cat, string strId)
		{
			int id = 0;
			if (idEntry.Text != "" && int.TryParse(idEntry.Text, out id))
			{
				if (cat.Get(id) == null)
				{
					return id;
				}
				else
				{
					throw new Exception("Id must be unique");
				}
			}
			else
			{
				throw new Exception("Id must be a number");
			}
		}

		protected void OnButtonCancelActivated(object sender, EventArgs e)
		{
			this.Respond(Gtk.ResponseType.Cancel);
		}

		protected void OnButtonOkClicked(object sender, EventArgs e)
		{
			try
			{
				if (cat != null)
				{
					int id = GetId(cat, idEntry.Text);
					string name = GetName(nameEntry.Text);
					doc = new Document(cat, id, name, dateCalendar.Date);
					cat.Add(doc);
					this.Respond(Gtk.ResponseType.Ok);
				}
				else
				{
					throw new Exception("You must select a category");
				}
			}
			catch (Exception ex)
			{
				Gtk.MessageDialog msg = new Gtk.MessageDialog(this, Gtk.DialogFlags.Modal, Gtk.MessageType.Error, Gtk.ButtonsType.Close, true, String.Format(ex.Message.ToString()));
				if ((Gtk.ResponseType)msg.Run() == Gtk.ResponseType.Close)
				{
					msg.Destroy();
				}
			}
		}

		protected void OnCategoryComboBoxChanged(object sender, EventArgs e)
		{
			cat = null;
			if (categoryComboBox.ActiveText != null)
			{
				cat = registry.Get(categoryComboBox.ActiveText);
				idEntry.Text = cat.NextId.ToString("0000");
			}
		}
	}
}

