using System;
using Gtk;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Reflection;
using SecretariaDataBase;

public partial class MainWindow: Gtk.Window
{   
    const string DATABASE_PATH = "Secretaría";
    const string DEFINITION_FILE = "box-definitions.xml";
    const string DATA_FILE = "document-data.xml";
    const string FILE_MANAGER = "xdg-open";
    Gdk.Pixbuf programIcon = new Gdk.Pixbuf(Assembly.GetExecutingAssembly(), "SecretariaDataBase.logo-ico");
    SortedList <string, List<SecretariaDataBase.FileSystem.Box>> boxList;
    string currentBoxList;
    string currentSelectedItemPath;

    Dictionary<string,string> preferences;

    public Dictionary<string,string> Preferences
    {
        get{return preferences;}
        set{preferences = value;}
    }

    public MainWindow(): base (Gtk.WindowType.Toplevel)
    {
        Build();
        try
        {
            //Load config file
            preferences = SecretariaDataBase.FileSystem.IO.ReadConfigurationFile();

            if (preferences.ContainsKey(ConfigKeys.LastFileSystem.ToString()))
            {
                boxList = SecretariaDataBase.FileSystem.IO.ReadFilesystem(preferences [ConfigKeys.LastFileSystem.ToString()]);
                if (boxList == null)
                {
                    throw new System.IO.IOException();
                }
            }else
			{
				preferencesAction.Activate();
			}
        } catch (Exception ex)
        {
            preferencesAction.Activate();
        }
        foreach (string key in boxList.Keys)
        {
            combobox1.AppendText(key);
        }

        //Load last combobox.Active from config file
        if (preferences.ContainsKey(ConfigKeys.LastComboboxActive.ToString()))
        {
            if (boxList.ContainsKey(preferences [ConfigKeys.LastComboboxActive.ToString()]))
            {
                combobox1.Active = boxList.IndexOfKey(preferences [ConfigKeys.LastComboboxActive.ToString()]);
                if(combobox1.Active<0)
                {
                    combobox1.Active = 0;
                }
            } 
            else
            {
                combobox1.Active = 0;
            }
        } else
        {
            combobox1.Active = 0;
        }

        currentBoxList = combobox1.ActiveText;

        PopulateTreeView();
        SecretariaDataBase.FileSystem.Box.SortBoxList(boxList [currentBoxList]);
        UpdateTreeView(boxList [currentBoxList]);

        treeview1.Selection.Changed += OnTreeViewSelectionChanged;
    }

    void OnTreeViewSelectionChanged(object sender, EventArgs e)
    {
        Gtk.TreeSelection selection = sender as Gtk.TreeSelection;
        string data;
        Gtk.TreeModel model;
        Gtk.TreeIter iter;

        if (selection.GetSelected(out model, out iter))
        {
            int column = 0;
            GLib.Value val = GLib.Value.Empty;
            model.GetValue(iter, column, ref val);
            //currentSelectedItemPath = model.GetPath(iter).ToString();
            data = (string)val.Val;
            System.Console.WriteLine(data); //DEBUG
            val.Dispose();
        } 
    }
    
    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        SecretariaDataBase.FileSystem.IO.WriteConfigurationFile(preferences);

        Application.Quit();
        a.RetVal = true;
    }

    private void PopulateTreeView()
    {
        Gtk.TreeViewColumn nameColumn = new Gtk.TreeViewColumn();
        nameColumn.Title = "Documento";
        Gtk.CellRendererText nameCell = new Gtk.CellRendererText();
        nameColumn.PackStart(nameCell, true);

        Gtk.TreeViewColumn directionColumn = new Gtk.TreeViewColumn();
        directionColumn.Title = "Dirección";
        Gtk.CellRendererText directionCell = new Gtk.CellRendererText();
        directionColumn.PackStart(directionCell, true);

        Gtk.TreeViewColumn idColumn = new Gtk.TreeViewColumn();
        idColumn.Title = "Id";
        Gtk.CellRendererText idCell = new Gtk.CellRendererText();
        idColumn.PackStart(idCell, true);

        Gtk.TreeViewColumn dateColumn = new Gtk.TreeViewColumn();
        dateColumn.Title = "Fecha";
        Gtk.CellRendererText dateCell = new Gtk.CellRendererText();
        dateColumn.PackStart(dateCell, true);
 
        treeview1.AppendColumn(nameColumn);
        treeview1.AppendColumn(directionColumn);
        treeview1.AppendColumn(idColumn);
        treeview1.AppendColumn(dateColumn);
 
        nameColumn.AddAttribute(nameCell, "text", 0);
        directionColumn.AddAttribute(directionCell, "text", 1);
        idColumn.AddAttribute(idCell, "text", 2);
        dateColumn.AddAttribute(dateCell, "text", 3);
    }

    private void UpdateTreeView(List<SecretariaDataBase.FileSystem.Box> boxesWithDocuments)
    {
        Gtk.TreeStore documentListStore = new Gtk.TreeStore(typeof(string), typeof(string), typeof(string), typeof(string));//Nombre, dirección, id, fecha

        if (boxesWithDocuments != null)
        {
            Gtk.TreeIter iter;
            foreach (var box in boxesWithDocuments)
            {
                iter = documentListStore.AppendValues(box.Name, box.DirectionString.ToString(), box.Code.ToString());
                foreach (var doc in box.Documents)
                {
                    documentListStore.AppendValues(iter, doc.Name, "", doc.Id.ToString(), doc.RegistrationDate.ToShortDateString());
                }
            }
            treeview1.Model = documentListStore;
        }
    }

    protected void OnCombobox1Changed(object sender, EventArgs e)
    {
        ComboBox combo = sender as ComboBox;
        if (sender == null)
            return;

        if (combo.Active >= 0)
        {
            currentBoxList = combo.ActiveText;
            UpdateTreeView(boxList [combo.ActiveText]);
            if(!preferences.ContainsKey(SecretariaDataBase.ConfigKeys.LastComboboxActive.ToString()))
            {
            preferences.Add(SecretariaDataBase.ConfigKeys.LastComboboxActive.ToString(),combo.ActiveText.ToString());
            }
            else
            {
                preferences[SecretariaDataBase.ConfigKeys.LastComboboxActive.ToString()] = combo.ActiveText.ToString();
            }
        }
    }

    protected void OnButton22Clicked(object sender, EventArgs e)
    {
        Gtk.TreeModel model;
        Gtk.TreeIter iter;
        string data;

        if (treeview1.Selection.GetSelected(out model, out iter))
        {
            GLib.Value val = GLib.Value.Empty;

            string[] t = model.GetPath(iter).ToString().Split(':');

            if (t.Length == 1) //Si se ha seleccionado un box
            {
                model.GetValue(iter, (int)SecretariaDataBase.Column.Name, ref val); //obtener name del box
                data = (string)val.Val;
                System.Console.WriteLine(data); //DEBUG

                if (currentBoxList != null)
                {
                    SecretariaDataBase.FileSystem.Box b = boxList [currentBoxList].Find(x => {
                        if (x.Name.ToString() == data)
                        {
                            return true;
                        } else
                        {
                            return false;
                        }}
                    );

                    if (b != null)
                    {
                        string p = System.IO.Path.Combine(System.Environment.CurrentDirectory, b.FolderName);
                        Gtk.MessageDialog msg = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.YesNo, true, "Esta operación <b>destruirá todos los archivos</b> en:\n" + p + "\ny <b>no se puede deshacer</b>.\n¿Está seguro de que desea continuar?");
                        if ((ResponseType)msg.Run() == ResponseType.Yes)
                        {
                            System.Console.WriteLine("Eliminar!");

                            SecretariaDataBase.FileSystem.IO.DestroyBox(currentBoxList, boxList [currentBoxList], b);
                            UpdateTreeView(boxList [currentBoxList]);
                        }
                        msg.Destroy();
                    }
                }
                val.Dispose();
            }
            if (t.Length == 2) //Si se ha seleccionado un documento
            {
                model.GetValue(iter, (int)SecretariaDataBase.Column.Id, ref val); //obtener id del documento
                data = (string)val.Val;
                System.Console.WriteLine(data); //DEBUG

                if (currentBoxList != null)
                {
                    SecretariaDataBase.FileSystem.Document doc = boxList [currentBoxList] [int.Parse(t [0])].Documents.Find(x => {
                        if (x.Id.ToString() == data)
                        {
                            return true;
                        } else
                        {
                            return false;
                        }}
                    );

                    if (doc != null)
                    {
                        string p = System.IO.Path.Combine(System.Environment.CurrentDirectory, boxList [currentBoxList] [int.Parse(t [0])].FolderName, doc.Name);
                        Gtk.MessageDialog msg = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.YesNo, true, "Esta operación <b>destruirá todos los archivos</b> en:\n" + p + "\ny <b>no se puede deshacer</b>.\n¿Está seguro de que desea continuar?");
                        if ((ResponseType)msg.Run() == ResponseType.Yes)
                        {
                            System.Console.WriteLine("Eliminar!");
                            SecretariaDataBase.FileSystem.IO.DestroyDocument(currentBoxList, boxList [currentBoxList] [int.Parse(t [0])], doc);
                            UpdateTreeView(boxList [currentBoxList]);
                        }
                        msg.Destroy();
                    }
                }
                val.Dispose();
            }
        } 
    }

    protected void OnButton23Clicked(object sender, EventArgs e)
    {
        Gtk.TreeModel model;
        Gtk.TreeIter iter;
        string data;

        if (treeview1.Selection.GetSelected(out model, out iter))
        {
            GLib.Value val = GLib.Value.Empty;

            string[] t = model.GetPath(iter).ToString().Split(':');

            if (t.Length == 1) //Si se ha seleccionado un box
            {
                model.GetValue(iter, (int)SecretariaDataBase.Column.Name, ref val); //obtener name del box
                data = (string)val.Val;
                System.Console.WriteLine(data); //DEBUG

                if (currentBoxList != null)
                {
                    SecretariaDataBase.FileSystem.Box b = boxList [currentBoxList].Find(x => {
                        if (x.Name.ToString() == data)
                        {
                            return true;
                        } else
                        {
                            return false;
                        }}
                    );

                    if (b != null)
                    {
                        string p = System.IO.Path.Combine(System.Environment.CurrentDirectory, b.FolderName);
                        System.Diagnostics.Process.Start(FILE_MANAGER, "\"" + p + "\"");
                    }
                }
                val.Dispose();
            }
            if (t.Length == 2) //Si se ha seleccionado un documento
            {
                model.GetValue(iter, (int)SecretariaDataBase.Column.Id, ref val); //obtener id del documento
                data = (string)val.Val;
                System.Console.WriteLine(data); //DEBUG

                if (currentBoxList != null)
                {
                    SecretariaDataBase.FileSystem.Document doc = boxList [currentBoxList] [int.Parse(t [0])].Documents.Find(x => {
                        if (x.Id.ToString() == data)
                        {
                            return true;
                        } else
                        {
                            return false;
                        }}
                    );

                    if (doc != null)
                    {
                        string p = System.IO.Path.Combine(System.Environment.CurrentDirectory, boxList [currentBoxList] [int.Parse(t [0])].FolderName, doc.Name);
                        System.Diagnostics.Process.Start(FILE_MANAGER, "\"file://" + p + "\"");
                    }
                }
                val.Dispose();
            }
        } 
    }

    protected void OnNewActionActivated(object sender, EventArgs e)
    {
        if (boxList != null)
        {
            SecretariaDataBase.NewRecord nd = new SecretariaDataBase.NewRecord(boxList [currentBoxList]);
            nd.Icon = programIcon;
            ResponseType resp = (ResponseType)nd.Run();
        
            if (resp == ResponseType.Ok)
            {
                System.Console.WriteLine("respuesta ok");

                nd.SelectedBox.Documents.Insert(nd.InsertingPosition, nd.NewDocument);
                SecretariaDataBase.FileSystem.IO.CreateNewDocument(currentBoxList, nd.SelectedBox, nd.NewDocument);

                UpdateTreeView(boxList [currentBoxList]);
            } else if (resp == ResponseType.Cancel)
            {
                System.Console.WriteLine("respuesta cancel");
            }

            nd.Destroy();
        }
    }

    protected void OnInfoActionActivated(object sender, EventArgs e)
    {
        Gtk.AboutDialog about = new AboutDialog();
        about.ProgramName = "Secretaría Electrial";
        about.Authors = new string[]{"Rafael Bailón-Ruiz <rafaelbailon@ieee.org>"};
        about.Copyright = "Copyright © 2013 Asociación Electrial";
        about.WebsiteLabel = "http://aselectrial.blogspot.com";
        about.Version = "0.2";
        about.Icon = programIcon;
        about.Logo = programIcon;
        about.License = "This program is free software: you can redistribute it and/or modify\nit under the terms of the GNU General Public License as published by\nthe Free Software Foundation, either version 3 of the License, or\n(at your option) any later version.\n\nThis program is distributed in the hope that it will be useful,\nbut WITHOUT ANY WARRANTY; without even the implied warranty of\nMERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the\nGNU General Public License for more details.\n\nYou should have received a copy of the GNU General Public License\nalong with this program.  If not, see <http://www.gnu.org/licenses/>.\n";
        about.Modal = true;

        if ((ResponseType)about.Run() == ResponseType.Cancel)
        {

        }
        about.Destroy();
    }

    protected void OnDirectoryActionActivated(object sender, EventArgs e)
    {
        if (boxList != null)
        {
            SecretariaDataBase.NewBox nb = new SecretariaDataBase.NewBox(boxList [currentBoxList]);
            nb.Icon = programIcon;
            ResponseType resp = (ResponseType)nb.Run();
        
            if (resp == ResponseType.Ok)
            {
                System.Console.WriteLine("respuesta ok");
                nb.CreatedBox.FolderName = System.IO.Path.Combine(currentBoxList, nb.CreatedBox.FolderName);
                boxList [currentBoxList].Insert(nb.InsertingPosition, nb.CreatedBox);
                SecretariaDataBase.FileSystem.IO.CreateBox(currentBoxList, nb.CreatedBox);

                UpdateTreeView(boxList [currentBoxList]);

            } else if (resp == ResponseType.Cancel)
            {
                System.Console.WriteLine("respuesta cancel");
            }

            nb.Destroy();
        }
    }

    protected void OnPreferencesActionActivated(object sender, EventArgs e)
    {
        SecretariaDataBase.PreferencesDialog pd = new SecretariaDataBase.PreferencesDialog(this);
        pd.Icon = programIcon;
        ResponseType resp = (ResponseType)pd.Run();
        
        if (resp == ResponseType.Ok)
        {
            System.Console.WriteLine("respuesta ok");
            if (boxList != null)
            {
                //I don't know why, but deleting items twice is the only form of really delete all them
                combobox1.Active = -1;
                for (int i = 0; i < boxList.Count; ++i)
                {
                    combobox1.RemoveText(i);
                }
                combobox1.Active = -1;
                for (int i = 0; i < boxList.Count; ++i)
                {
                    combobox1.RemoveText(i);
                }

            }

            boxList = pd.BoxList;

            foreach (string key in boxList.Keys)
            {
                combobox1.AppendText(key);
            }
            combobox1.Active = 0;
            currentBoxList = combobox1.ActiveText;

            SecretariaDataBase.FileSystem.Box.SortBoxList(boxList [currentBoxList]);
            UpdateTreeView(boxList [currentBoxList]);

            treeview1.Selection.Changed += OnTreeViewSelectionChanged;

        } else if (resp == ResponseType.Cancel)
        {
            System.Console.WriteLine("respuesta cancel");
        }

        pd.Destroy();
    }
}

