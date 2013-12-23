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
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;

namespace SecretariaElectrial.FileSystem
{
	public class Registry : IEnumerable<Category>
    {
        string basePath = "registro";
        public List<Category> categories;

        public string BasePath
        {
            get{ return basePath;}
            set{ basePath = value;}
        }

        public List<Category> Categories
        {
            get{ return categories;}
            set{ categories = value;}
        }

        public Category this [Category.DirectionCode dir, Category.GroupCode gro]
        {
            get{ return categories.Find(x => x.Direction == dir && x.Group == gro);}
        }

        public Registry(string path)
        {
            basePath = path;
            categories = new List<Category>();
        }

		public IEnumerator<Category> GetEnumerator()
        {
            return this.categories.GetEnumerator();
        }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public static void CreateFileSystem(string basePath)
        {
            if (!System.IO.Directory.Exists(Path.Combine(basePath, "entrada")))
            {
                Directory.CreateDirectory(Path.Combine(basePath, "entrada"));
            }

            if (!Directory.Exists(Path.Combine(basePath, "salida")))
            {
                Directory.CreateDirectory(Path.Combine(basePath, "salida"));
            }
        }

        public void Write()
        {
			CreateFileSystem(basePath);

            foreach (var cat in categories)
            {
                cat.Write(basePath);
            }
        }

        public static Registry LoadFrom(string path)
        {
            List<string> directories = new List<string>(Directory.EnumerateDirectories(path));

            Registry reg = new Registry(path);

            foreach (var dire in directories)
            {
                int pos = dire.LastIndexOf(Path.DirectorySeparatorChar);
                string dir = dire.Substring(pos + 1);
                switch (dir)
                {
                    case "entrada":
						reg.Categories.AddRange(Category.LoadFrom(reg,Path.Combine(path, "entrada")));
                        break;
                    case "salida":
						reg.Categories.AddRange(Category.LoadFrom(reg,Path.Combine(path, "salida")));
                        break;
                    default:
                        break;
                }
            }

            return reg;
        }

        public void Add(Category newCat)
        {
            Category cat = categories.Find(x => x.Direction == newCat.Direction && x.Group == newCat.Group);
            if (cat != null)
            {
                categories.Add(newCat);
            }
        }

        /// <summary>
        /// Gets the category determined by its DirectionCode and GroupCode. Returns null when a matching category doesn't exists
        /// </summary>
        /// <param name='dir'>
        /// DirectionCode.
        /// </param>
        /// <param name='gro'>
        /// GroupCode.
        /// </param>
        public Category Get(Category.DirectionCode dir, Category.GroupCode gro)
        {
            return categories.Find(x => x.Direction == dir && x.Group == gro);
        }
		/// <summary>
		/// Get the specified repr.
		/// </summary>
		/// <param name="repr">Repr.</param>
		public Category Get(string repr)
		{
			string[] pieces = repr.Split('_');
			if (pieces.Length == 2)
			{
				char[] codes = pieces[0].ToCharArray();
				int dirInt = 0;
				Category.DirectionCode dir;
				if (!int.TryParse(codes[0].ToString(), out dirInt))
				{
					throw new InvalidDataException();
				}
				else
				{
						dir = (Category.DirectionCode)dirInt;
				}

				int groInt = 0;
				Category.GroupCode gro;
				if (!int.TryParse(codes[1].ToString(), out groInt))
				{
					throw new InvalidDataException();
				}
				else
				{
					gro = (Category.GroupCode)groInt;
				}

				string nam = pieces[1].ToLower();

				Category cat = categories.Find(x => x.Direction == dir && x.Group == gro && x.Name == nam);

				return cat;
			}
			throw new InvalidDataException();
		}

        public bool Contains(Category cat)
        {
            return categories.Contains(cat);
        }

        public bool Contains(Category.DirectionCode dir, Category.GroupCode gro)
        {
            return categories.Exists(x => x.Direction == dir && x.Group == gro);
        }

        public void Remove(Category cat)
        {
            categories.Remove(cat);
        }

        public void Remove(Category.DirectionCode dir, Category.GroupCode gro)
        {
            categories.RemoveAll(x => x.Direction == dir && x.Group == gro);
        }
    }
}