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
using System.IO;
using System.Collections.Generic;

namespace SecretariaElectrial.FileSystem
{
	public class Document
	{
		int id;
		string name;
		DateTime registrationDate;
		List<String> files;
		string separator = "_";
		Category parentCategory;

		public int Id
		{
			get{ return id; }
			set{ id = value; }
		}

		public string Name
		{
			get{ return name; }
			set{ name = value; }
		}

		public DateTime RegistrationDate
		{
			get{ return registrationDate; }
			set{ registrationDate = value; }
		}

		public List<String> Files
		{
			get{ return files; }
			set{ files = value; }
		}

		public Category ParentCategory
		{
			get{ return parentCategory; }
			set{ parentCategory = value; }
		}

		public Document(Category parent, int id, string name, DateTime registrationDate, List<String> files)
		{
			this.parentCategory = parent;
			this.id = id;
			this.name = name;
			this.registrationDate = registrationDate;
			this.files = files;
		}

		public Document(Category parent, int id, string name, DateTime registrationDate)
		{
			this.parentCategory = parent;
			this.id = id;
			this.name = name;
			this.registrationDate = registrationDate;
			this.files = new List<string>();
		}

		public void Write(string basePath)
		{
			if (!Directory.Exists(Path.Combine(basePath, this.ToString())))
			{
				Directory.CreateDirectory(Path.Combine(basePath, this.ToString()));
			}
		}

		public void Delete()
		{
			string p = System.IO.Path.Combine(parentCategory.ParentRegistry.BasePath, parentCategory.GetDirectionString(), parentCategory.ToString(), ToString());
			System.IO.Directory.Delete(p);
			ParentCategory.Remove(this);
		}

	
		public static List<Document> LoadFrom(Category parent, string path)
		{
			List<Document> documents = new List<Document>();

			//Los documentos pueden estar en directorios o solo un archivo
			//0000_12_noviembre_2012_nombre guay
			List<string> directories = new List<string>(Directory.EnumerateDirectories(path));

			foreach (var dire in directories)
			{
				int pos = dire.LastIndexOf(Path.DirectorySeparatorChar);
				string dir = dire.Substring(pos + 1);

				string[] pieces = dir.Split('_');
				bool ok = true;

				if (pieces.Length == 5)
				{
					int id = 0;
					if (!int.TryParse(pieces[0], out id))
					{
						ok = false;
					}

					DateTime regDate = DateTime.Now;

					if (!DateTime.TryParse(pieces[1] + "/" + pieces[2] + "/" + pieces[3], System.Globalization.CultureInfo.CreateSpecificCulture("ES-es").DateTimeFormat, System.Globalization.DateTimeStyles.AssumeLocal, out regDate))
					{
						ok = false;
					}

					string nam = pieces[4];
					if (ok == true)
					{
						Document doc = new Document(parent, id, nam, regDate);

						documents.Add(doc);
					}
				}
			}

			List<string> files = new List<string>(Directory.EnumerateFiles(path));

			foreach (var file in files)
			{
				int lastPoint = file.LastIndexOf('.');
				string fil = file;

				if (lastPoint >= 0)
				{
					fil = file.Substring(0, lastPoint - 1);
				}

				string[] pieces = fil.Split('_');
				bool ok = true;

				if (pieces.Length == 5)
				{
					int id = 0;
					if (!int.TryParse(pieces[0], out id))
					{
						ok = false;
					}

					DateTime regDate = DateTime.Now;

					if (!DateTime.TryParse(pieces[1] + "/" + pieces[2] + "/" + pieces[3], System.Globalization.CultureInfo.CreateSpecificCulture("ES-es").DateTimeFormat, System.Globalization.DateTimeStyles.AssumeLocal, out regDate))
					{
						ok = false;
					}

					string nam = pieces[4];
					if (ok == true)
					{
						Document doc = new Document(parent, id, nam, regDate);

						documents.Add(doc);
					}
				}
			}

			return documents;
		}

		public override string ToString()
		{
			return string.Format(id.ToString("0000") + separator +
			registrationDate.Day.ToString() + separator +
			System.Globalization.CultureInfo.CreateSpecificCulture("es-ES").DateTimeFormat.MonthNames[registrationDate.Month - 1] +
			separator + registrationDate.Year.ToString() + separator + name.ToString().ToLower());
		}
	}
}