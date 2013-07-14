using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;

namespace SecretariaDataBase.FileSystem
{
    public static class IO
    {
        const string DATABASE_PATH = "Secretaría";
        const string BOX_FILE = ".box.xml";
        const string DOCUMENT_FILE = ".document.xml";
        const string FILE_MANAGER = "nautilus";

        public static SortedList<string, List<Box>> ReadFilesystem()
        {
            return ReadFilesystem(DATABASE_PATH);
        }

        public static SortedList<string, List<Box>> ReadFilesystem(string databasePath)
        {
            List<string> yearDirectories = new List<string>(System.IO.Directory.EnumerateDirectories(databasePath));
            if (yearDirectories.Count < 1)
            {
				CreateFileSystem(databasePath);
				yearDirectories = new List<string>(System.IO.Directory.EnumerateDirectories(databasePath));
            }
            SortedList <string, List<Box>> boxList = new SortedList<string, List<Box>>();

            //1er nivel: años
            foreach (string yearDir in yearDirectories)
            {
                boxList.Add(yearDir, new List<Box>());

                if (System.IO.Directory.EnumerateDirectories(yearDir) != null)
                {
                    //el 2º nivel de subcarpetas son las boxes
                    foreach (string boxDir in System.IO.Directory.EnumerateDirectories(yearDir))
                    {
                        XDocument boxFile = XDocument.Load(System.IO.Path.Combine(boxDir, BOX_FILE));
                        XElement rootElement = boxFile.Element("Box");

                        BoxDirection direction;
                        if (rootElement.Element("Direction").Value == ((int)BoxDirection.In).ToString())
                        {
                            direction = SecretariaDataBase.FileSystem.BoxDirection.In;
                        } else if (rootElement.Element("Direction").Value == ((int)BoxDirection.Out).ToString())
                        {
                            direction = SecretariaDataBase.FileSystem.BoxDirection.Out;
                        } else
                        {
                            direction = SecretariaDataBase.FileSystem.BoxDirection.None;
                        }

                        Box newBox = new Box(direction, int.Parse(rootElement.Element("Id").Value), rootElement.Element("Name").Value, boxDir);
                        boxList [yearDir].Add(newBox);

                        //el 3er nivel de subcarpetas son los documents
                        foreach (string documentDir in System.IO.Directory.EnumerateDirectories(boxDir))
                        {
                            XDocument xmlDocument = XDocument.Load(System.IO.Path.Combine(documentDir, DOCUMENT_FILE));
                            XElement docXml = xmlDocument.Element("Document");
                            newBox.Documents.Add(new Document(int.Parse(docXml.Element("Id").Value.ToString()), docXml.Element("Name").Value.ToString(), DateTime.Parse(docXml.Element("RegistrationDate").Value), new List<string>()));
                        }
                        //Ordenar los documentos añadidios por nº de código
                        newBox.SortDocuments();
                    }
                }
                Box.SortBoxList(boxList [yearDir]);
            }

            foreach (string yearDir in yearDirectories)
            {
                Box.SortBoxList(boxList [yearDir]);
            }

            return boxList;
        }

		public static void CreateFileSystem (string databasePath)
		{
			System.IO.Directory.CreateDirectory(System.IO.Path.Combine(databasePath,DateTime.Now.Year.ToString()));
		}

        public static void CreateNewDocument(string year, Box containingBox, Document newDoc)
        {
            XDocument docXmlFile = new XDocument();

            docXmlFile.Add(new XElement("Document",
                                           new XElement("Id", newDoc.Id.ToString()),
                                           new XElement("Name", newDoc.Name),
                                           new XElement("RegistrationDate", newDoc.RegistrationDate.ToString()))
            );

            StreamWriter sw = null;
            try
            {
                string path = Path.Combine(containingBox.FolderName, newDoc.Name);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                sw = new StreamWriter(Path.Combine(path, DOCUMENT_FILE));
                sw.Write(docXmlFile.ToString());
            } finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        public static void DestroyDocument(string year, Box containingBox, Document doc)
        {
            string p = System.IO.Path.Combine(System.Environment.CurrentDirectory, containingBox.FolderName, doc.Name);
            containingBox.Documents.Remove(doc);
            System.IO.Directory.Delete(p, true);
        }

        public static void CreateBox(string year, Box newBox)
        {
            XDocument boxXmlFile = new XDocument();

            boxXmlFile.Add(new XElement("Box",
                                           new XElement("Id", newBox.Code.ToString()),
                                           new XElement("Direction", (int)newBox.Direction),
                                           new XElement("Name", newBox.Name))
            );

            StreamWriter sw = null;
            try
            {
                string path = newBox.FolderName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                sw = new StreamWriter(Path.Combine(path, BOX_FILE));
                sw.Write(boxXmlFile.ToString());
            } finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        public static void DestroyBox(string year, List<Box> boxList, Box box)
        {
            string p = System.IO.Path.Combine(System.Environment.CurrentDirectory, box.FolderName);
            boxList.Remove(box);
            System.IO.Directory.Delete(p, true);
        }

        public static Dictionary<string, string> ReadConfigurationFile()
        {
            Dictionary<string,string> dict = new Dictionary<string, string>();
            try
            {
                XDocument configFile = XDocument.Load(System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".config", "secretaria-electrial", "preferences.xml"));
                XElement rootElement = configFile.Element("Preferences");
                foreach (var elem in rootElement.Elements())
                {
                    dict.Add(elem.Name.ToString(), elem.Value.ToString());
                }
            } catch (Exception ex)
            {
            }
            return dict;
        }

        public static void WriteConfigurationFile(Dictionary<string, string> preferences)
        {
            XDocument prefXmlFile = new XDocument();
            XElement[] elements = new XElement[preferences.Count];

            int i = 0;
            foreach (var item in preferences)
            {
                elements [i] = new XElement(item.Key, item.Value);
                ++i;
            }

            prefXmlFile.Add(new XElement("Preferences", elements));

            StreamWriter sw = null;
            try
            {
                string path = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal),".config", "secretaria-electrial");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                sw = new StreamWriter(System.IO.Path.Combine(path,"preferences.xml"), false);
                sw.Write(prefXmlFile.ToString());
            } finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }       
    }
}