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
using System.IO;
using System.Collections.Generic;

namespace SecretariaElectrial.FileSystem
{
    public class Category : IEnumerable<Document>
    {
        public enum DirectionCode
        {
            Entrada = 0,
            Salida = 1
        }

        public enum GroupCode
        {
            JuntaDirectiva = 0,
            AsambleaGeneral = 1
        }

		Registry parentRegistry;
        DirectionCode direction;
        GroupCode group;
        string name;
		List<Document> documents;

        public DirectionCode Direction
        {
            get{ return direction;}
            set{ direction = value;}
        }

        public GroupCode Group
        {
            get{ return group;}
            set{ group = value;}
        }

        public string Name
        {
            get{ return name;}
            set{ name = value;}
        }

		[Obsolete]
		public System.Collections.ObjectModel.ReadOnlyCollection<Document> Documents
        {
			get{ return documents.AsReadOnly();}
			//set{ documents = value;}
        }

		public Registry ParentRegistry
		{
			get{return parentRegistry; }
			set{parentRegistry = value; }
		}

        public Document this [int id]
        {
            get{ return documents.Find(x => x.Id == id);}
        }

		public int NextId {
			get{if(documents.Count>0){return documents[documents.Count - 1].Id+1;}else{return 0;}}
		}

		public Category(Registry parent,DirectionCode dir, GroupCode gro, string name)
        {
			this.parentRegistry = parent;
            this.direction = dir;
            this.group = gro;
            this.name = name;
            documents = new List<Document>();
        }

		public IEnumerator<Document> GetEnumerator()
		{
			return this.documents.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

        public void Write(string basePath, bool recursive)
        {
            string folder = null;
            switch (direction)
            {
                case DirectionCode.Entrada:
                    folder = "entrada";
                    break;
                case DirectionCode.Salida:
                    folder = "salida";
                    break;
            }
            //1st hierarchy level "In"/"Out"
            if (!System.IO.Directory.Exists(Path.Combine(basePath, folder)))
            {
                Directory.CreateDirectory(Path.Combine(basePath, folder));
            }

            //2nd level
			string secondfolder = this.ToString();
            if (!Directory.Exists(Path.Combine(basePath, folder, secondfolder)))
            {
                Directory.CreateDirectory(Path.Combine(basePath, folder, secondfolder));
            }
            if (recursive)
            {
                foreach (var doc in documents)
                {
                    doc.Write(Path.Combine(basePath, folder, secondfolder));
                }
            }

        }

        public void Write(string basePath)
        {
            Write(basePath, true);
        }

		public static List<Category> LoadFrom(Registry parent,string path)
        {
            List<string> directories = new List<string>(Directory.EnumerateDirectories(path));
            List<Category> cats = new List<Category>();

            foreach (var directorio in directories)
            {
                int pos = directorio.LastIndexOf(Path.DirectorySeparatorChar);
                string dir = directorio.Substring(pos + 1);

                string[] pieces = dir.Split('_');
                bool ok = true;

                if (pieces.Length == 2)
                {
                    DirectionCode dire = DirectionCode.Entrada;
                    if (!DirectionCode.TryParse(pieces [0] [0].ToString(), out dire))
                    {
                        ok = false;
                    }

                    GroupCode grou = GroupCode.AsambleaGeneral;
                    if (!GroupCode.TryParse(pieces [0] [1].ToString(), out grou))
                    {
                        ok = false;
                    }

                    string nam = pieces [1];
                    if (ok == true)
                    {
						Category cat = new Category(parent,dire, grou, nam);
						cat.documents.AddRange(Document.LoadFrom(cat,Path.Combine(path, dir)));
                        cats.Add(cat);
                    }
                }
            }

            return cats;

        }

		public string GetDirectionString()
		{
			switch (direction)
			{
				case Category.DirectionCode.Entrada:
					return "entrada";
				case Category.DirectionCode.Salida:
					return "salida";
				default:
					throw new Exception(); //FIXME throw a better exception type
			}
		}

        public void SortDocuments()
        {
            documents.Sort((x,y) => {
                if (x.Id == y.Id)
                    return 0;
                else if (x.Id < y.Id)
                    return -1;
                else
                    return 1;
            }
            );
        }

        /// <summary>
        /// Gets the position for new document.
        /// </summary>
        /// <returns>
        /// The position for new document. 
        /// </returns>
        /// <param name='newId'>
        /// New identifier.
        /// </param>
        public int GetPositionForAddDocument(int newId)
        {
            SortDocuments();

            if (documents.Count == 0)
            {
                return 0; //if documents is empty, return the first index -> 0
            }

            for (int i = 0; i < documents.Count; ++i)
            {
                if (documents [i].Id > newId)
                {
                    return i; //if document[i].Id is bigger than requested id, then the new element is going to be inserted after element i
                }
            }

            return documents.Count;
        }

        [Obsolete]
        public static void SortBoxList(List<Category> boxList)
        {
            List<Category> sortedBox = new List<Category>();
            List<Category> outBoxList = new List<Category>();
            List<Category> inBoxList = new List<Category>();

            foreach (Category box in boxList)
            {
//                if(box.Direction == BoxDirection.In)
//                {
//                    inBoxList.Add(box);
//                }
//                else if(box.Direction == BoxDirection.Out)
//                {
//                    outBoxList.Add(box);
//                }
//                else if(box.Direction == BoxDirection.None)
//                {
//                    sortedBox.Add(box);
//                }
            }
//
//            sortedBox.Sort((x,y) => {
//                if (x.Code == y.Code)
//                    return 0;
//                else if (x.Code < y.Code)
//                    return -1;
//                else
//                    return 1;
//            }
//            );
//
//            outBoxList.Sort((x,y) => {
//                if (x.Code == y.Code)
//                    return 0;
//                else if (x.Code < y.Code)
//                    return -1;
//                else
//                    return 1;
//            }
//            );
//
//            inBoxList.Sort((x,y) => {
//                if (x.Code == y.Code)
//                    return 0;
//                else if (x.Code < y.Code)
//                    return -1;
//                else
//                    return 1;
//            }
//            );

            sortedBox.AddRange(inBoxList);
            sortedBox.AddRange(outBoxList);

            boxList = sortedBox;
        }

        [Obsolete]
        public static int GetPositionForNewBox(List<Category> boxList, int newId)
        {
            SortBoxList(boxList);

            if (boxList.Count == 0)
            {
                return 0; //if boxList is empty, return the first index -> 0
            }

            for (int i = 0; i < boxList.Count; ++i)
            {
//                if (boxList [i].Code > newId)
//                {
//                    return i; //if boxList[i].Code is bigger than requested id, then the new box is going to be inserted after element i
//                }
            }

            return boxList.Count;
        }
        
        public void Add(Document newDoc)
        {
            if (newDoc == null)
            {
                throw new ArgumentNullException();
            }

            Document cat = documents.Find(x => x.Id == newDoc.Id);
            if (cat == null)
            {
                documents.Insert(GetPositionForAddDocument(newDoc.Id), newDoc);
            } else
            {
                throw new ArgumentException("There is another document with the same Id");
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
        public Document Get(int id)
        {
            return documents.Find(x => x.Id == id);
        }

        public List<Document> Find(DateTime startDate, DateTime endDate)
        {
            TimeSpan tSpan = endDate - startDate;
            return documents.FindAll(x => x.RegistrationDate - startDate <= tSpan ? true : false);
        }
        /// <summary>
        /// Find documents containing "name".
        /// </summary>
        /// <param name='name'>
        /// Name.
        /// </param>
        public List<Document> Find(string name)
        {
            return documents.FindAll(x => x.Name.Contains(name) ? true : false);
        }
        /// <summary>
        /// Find documents with ids between firstId and lastId.
        /// </summary>
        /// <param name='firstId'>
        /// First identifier.
        /// </param>
        /// <param name='lastId'>
        /// Last identifier.
        /// </param>
        public List<Document> Find(int firstId, int lastId)
        {
            return documents.FindAll(x => (x.Id >= firstId) && (x.Id <= lastId) ? true : false);
        }

        public bool Contains(Document doc)
        {
            return documents.Contains(doc);
        }

        public bool Contains(int id)
        {
            return documents.Exists(x => x.Id == id);
        }

        public void Remove(Document doc)
        {
            documents.Remove(doc);
        }

        public void Remove(int id)
        {
            documents.RemoveAll(x => x.Id == id);
        }

		public override string ToString()
		{
			return string.Format(((int)direction).ToString() + ((int)group).ToString() + "_" + name.ToLower().ToString());
		}
    }
}

