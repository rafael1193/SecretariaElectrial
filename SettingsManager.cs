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
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;

namespace SecretariaElectrial
{
	public class SettingsManager
	{
		public enum PresetKeys
		{        
			LastFileSystem
		}

		string path;
		bool autoflush;
		Dictionary<string,string> preferences;

		public SettingsManager (bool autoWrite)
		{
			this.autoflush = autoWrite;
			try {
				preferences = ReadSettingsFile ();
			} catch {
				preferences = new Dictionary<string, string> ();
				WriteConfigurationFile ();
			}
		}

		private Dictionary<string,string> ReadSettingsFile ()
		{
			Dictionary<string,string> dict = new Dictionary<string, string> ();
			XDocument configFile = XDocument.Load (System.IO.Path.Combine (System.Environment.GetFolderPath (Environment.SpecialFolder.Personal), ".config", "secretaria-electrial", "preferences.xml"));
			XElement rootElement = configFile.Element ("Preferences");
			foreach (var elem in rootElement.Elements()) {
				dict.Add (elem.Name.ToString (), elem.Value.ToString ());
			}
			return dict;
		}

		private void WriteConfigurationFile ()
		{
			XDocument prefXmlFile = new XDocument ();
			XElement[] elements = new XElement[preferences.Count];

			int i = 0;
			foreach (var item in preferences) {
				elements [i] = new XElement (item.Key, item.Value);
				++i;
			}

			prefXmlFile.Add (new XElement ("Preferences", elements));

			StreamWriter sw = null;
			try {
				string path = System.IO.Path.Combine (System.Environment.GetFolderPath (Environment.SpecialFolder.Personal), ".config", "secretaria-electrial");
				if (!Directory.Exists (path)) {
					Directory.CreateDirectory (path);
				}

				sw = new StreamWriter (System.IO.Path.Combine (path, "preferences.xml"), false);
				sw.Write (prefXmlFile.ToString ());
			} finally {
				if (sw != null) {
					sw.Close ();
				}
			}
		}

		public void Flush ()
		{
			WriteConfigurationFile ();
		}

		public string Get (string key)
		{
			if (!string.IsNullOrEmpty (key)) {
				if (preferences.ContainsKey (key)) {
					return preferences [key];
				} else {
					return null;
				}
			} else {
				return null;
			}
		}

		public void Set (string key, string value)
		{
			if (!string.IsNullOrEmpty (key)) {
				if (preferences.ContainsKey (key)) {
					preferences [key] = value;
				} else {
					preferences.Add (key, value);
				}
				if (autoflush) {
					WriteConfigurationFile ();
				}
			}
		}

		public void Delete (string key)
		{
			if (!string.IsNullOrEmpty (key)) {
				if (preferences.ContainsKey (key)) {
					preferences.Remove (key);
					if (autoflush) {
						WriteConfigurationFile ();
					}
				}
			}
		}

		public bool ExistsKey (string key)
		{
			if (!string.IsNullOrEmpty (key)) {
				if (preferences.ContainsKey (key)) {
					return true;
				} else {
					return false;
				} 
			} else {
				return false;
			}
		}
	}
}

