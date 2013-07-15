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

namespace SecretariaDataBase.FileSystem
{
    public class Box
    {
        BoxDirection direction;
        int code;
        string name;
        string folderName;
        List<Document> documents;

        public BoxDirection Direction
        {
            get{ return direction;}
            set{ direction = value;}
        }

        public string DirectionString
        {
            get
            { 
                switch (direction)
                {
                    case BoxDirection.In:
                        return "Entrada";
                    case BoxDirection.Out:
                        return "Salida";
                    default:
                        return "";
                }
            }
        }

        public int Code
        {
            get{ return code;}
            set{ code = value;}
        }

        public string Name
        {
            get{ return name;}
            set{ name = value;}
        }

        public string FolderName
        {
            get{ return folderName;}
            set{ folderName = value;}
        }

        public List<Document> Documents
        {
            get{ return documents;}
            set{ documents = value;}
        }

        public Box(BoxDirection direction, int code, string name, string folderName)
        {
            this.direction = direction;
            this.code = code;
            this.name = name;
            this.folderName = folderName;
            documents = new List<Document>();
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
        public int GetPositionForNewDocument(int newId)
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

        public static void SortBoxList(List<Box> boxList)
        {
            List<Box> sortedBox = new List<Box>();
            List<Box> outBoxList = new List<Box>();
            List<Box> inBoxList = new List<Box>();

            foreach (Box box in boxList)
            {
                if(box.Direction == BoxDirection.In)
                {
                    inBoxList.Add(box);
                }
                else if(box.Direction == BoxDirection.Out)
                {
                    outBoxList.Add(box);
                }
                else if(box.Direction == BoxDirection.None)
                {
                    sortedBox.Add(box);
                }
            }

            sortedBox.Sort((x,y) => {
                if (x.Code == y.Code)
                    return 0;
                else if (x.Code < y.Code)
                    return -1;
                else
                    return 1;
            }
            );

            outBoxList.Sort((x,y) => {
                if (x.Code == y.Code)
                    return 0;
                else if (x.Code < y.Code)
                    return -1;
                else
                    return 1;
            }
            );

            inBoxList.Sort((x,y) => {
                if (x.Code == y.Code)
                    return 0;
                else if (x.Code < y.Code)
                    return -1;
                else
                    return 1;
            }
            );

            sortedBox.AddRange(inBoxList);
            sortedBox.AddRange(outBoxList);

            boxList = sortedBox;
        }

        public static int GetPositionForNewBox(List<Box> boxList, int newId)
        {
            SortBoxList(boxList);

            if (boxList.Count == 0)
            {
                return 0; //if boxList is empty, return the first index -> 0
            }

            for (int i = 0; i < boxList.Count; ++i)
            {
                if (boxList [i].Code > newId)
                {
                    return i; //if boxList[i].Code is bigger than requested id, then the new box is going to be inserted after element i
                }
            }

            return boxList.Count;
        }
    }
}

