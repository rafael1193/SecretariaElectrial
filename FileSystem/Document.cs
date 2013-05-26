using System;
using System.Collections.Generic;

namespace SecretariaDataBase.FileSystem
{

    public class Document
    {
        int id;
        string name;
        DateTime registrationDate;
        List<String> files;

        public int Id
        {
            get{ return id;}
            set{ id = value;}
        }

        public string Name
        {
            get{ return name;}
            set{ name = value;}
        }

        public DateTime RegistrationDate
        {
            get{ return registrationDate;}
            set{ registrationDate = value;}
        }

        public List<String> Files
        {
            get{ return files;}
            set{ files = value;}
        }

        public Document(int id, string name, DateTime registrationDate, List<String> files)
        {
            this.id = id;
            this.name = name;
            this.registrationDate = registrationDate;
            this.files = files;
        }
    }
}

