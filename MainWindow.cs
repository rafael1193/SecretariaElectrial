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
using Gtk;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Reflection;
using SecretariaElectrial;
using SecretariaElectrial.FileSystem;

public partial class MainWindow: Gtk.Window
{
	const string DATABASE_PATH = "Secretaría";
	const string DEFINITION_FILE = "box-definitions.xml";
	const string DATA_FILE = "document-data.xml";
	readonly string FILE_MANAGER;
	Gdk.Pixbuf programIcon = new Gdk.Pixbuf(Assembly.GetExecutingAssembly(), "SecretariaElectrial.logo-ico");
	Registry registry;
	Category categorySelectedInComboBox = null;
	int nElementsInComboBox = 0;
	Document documentSelectedInTreeView = null;
	TreeStore documentListStore;
	TreeModelFilter filt;
	internal SettingsManager settings;
	bool firstrun;
	Emailer emailer;

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		Build();

		//Windows doesn't know anything about FreeDesktop standards
		if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			FILE_MANAGER = "explorer.exe";
		}
		else
		{
			FILE_MANAGER = "xdg-open";
		}

		Reload();

		emailButton.Label = Mono.Unix.Catalog.GetString("Send by email");

		UpdateCategoryComboBox();
		PopulateTreeView();
		UpdateTreeView();

		DocumentTreeView.Selection.Changed += OnTreeViewSelectionChanged;
	}

	void UpdateReadOnlyMode()
	{
		if (settings.ExistsKey(SettingsManager.PresetKeys.ReadOnlyMode.ToString()))
		{
			if (settings.Get(SettingsManager.PresetKeys.ReadOnlyMode.ToString()) == "True")
			{
				newAction.Sensitive = false;
				deleteButton.Sensitive = false;
			}
			else
			{
				newAction.Sensitive = true;
				deleteButton.Sensitive = true;
			}
		}
	}

	public void Reload()
	{
		//Load config file
		settings = new SettingsManager(true);
		try
		{
			registry = Registry.LoadFrom(settings.Get(SettingsManager.PresetKeys.LastFileSystem.ToString()));
			UpdateReadOnlyMode(); //Check for ReadOnlyMode and change GUI according to setting
		}
		catch (Exception e)
		{
//			Gtk.MessageDialog msg = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Close, true, string.Format(e.Message.ToString()));
//			if ((ResponseType)msg.Run() == ResponseType.Close)
//			{
//				msg.Destroy();
//			}
//			string newPath = System.IO.Path.Combine(Environment.CurrentDirectory, "registro");
//			settings.Set(SettingsManager.PresetKeys.LastFileSystem.ToString(), newPath); //FIXME: mostrar ventana de opciones
//			Registry.CreateFileSystem(newPath);
//			registry = Registry.LoadFrom(settings.Get(SettingsManager.PresetKeys.LastFileSystem.ToString()));

			preferencesAction.Activate();

			//FIXME: Set comoboBox active text index that was active before refresh
			firstrun = true;
			//preferencesAction.Activate(); //Show preferences window in order to create a filesystem
			//boxList = SecretariaElectrial.FileSystem.IO.ReadFilesystem(settings.Get(SettingsManager.PresetKeys.LastFileSystem.ToString())); //Load good filesystem
			firstrun = false;
		}
	}

	private void UpdateCategoryComboBox()
	{
		bool sel = false;
		int n = nElementsInComboBox;
		for(int i = 0; i < n; ++i)
		{
			categoryComboBox.RemoveText(0);
			--nElementsInComboBox;
		}
		foreach (var cat in registry)
		{
			categoryComboBox.AppendText(cat.ToString());
			++nElementsInComboBox;
			sel = true;
		}
		if (sel)
		{
			categoryComboBox.Active = 0;
			categorySelectedInComboBox = registry.Get(categoryComboBox.ActiveText);
		}
	}

	protected void OnCategoryComboBoxChanged(object sender, EventArgs e)
	{
		ComboBox combo = sender as ComboBox;

		if (combo != null)
		{
			if (combo.ActiveText != null)
			{
				categorySelectedInComboBox = registry.Get(combo.ActiveText);
			}
			else
			{
				categorySelectedInComboBox = null;
			}
		}
		UpdateTreeView();
	}

	void OnTreeViewSelectionChanged(object sender, EventArgs e)
	{
		Gtk.TreeSelection selection = sender as Gtk.TreeSelection;
		string data;
		Gtk.TreeModel model;
		Gtk.TreeIter iter;

		if (selection.GetSelected(out model, out iter))
		{
			int depth = model.GetPath(iter).ToString().Split(':').Length;
			if (depth == 1) //Category
			{
				int column = (int)Column.Id;
				GLib.Value val = GLib.Value.Empty;
				model.GetValue(iter, column, ref val);
				data = (string)val.Val;
				documentSelectedInTreeView = registry.Get(categorySelectedInComboBox.Direction, categorySelectedInComboBox.Group).Get(int.Parse(data));
				val.Dispose();
			}
		} 
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	private void PopulateTreeView()
	{
		Gtk.TreeViewColumn idColumn = new Gtk.TreeViewColumn();
		idColumn.Title = Mono.Unix.Catalog.GetString("Id");
		Gtk.CellRendererText idCell = new Gtk.CellRendererText();
		idColumn.PackStart(idCell, true);

		Gtk.TreeViewColumn dateColumn = new Gtk.TreeViewColumn();
		dateColumn.Title = Mono.Unix.Catalog.GetString("Date");
		Gtk.CellRendererText dateCell = new Gtk.CellRendererText();
		dateColumn.PackStart(dateCell, true);

		Gtk.TreeViewColumn nameColumn = new Gtk.TreeViewColumn();
		nameColumn.Title = Mono.Unix.Catalog.GetString("Name");
		Gtk.CellRendererText nameCell = new Gtk.CellRendererText();
		nameColumn.PackStart(nameCell, true);

		DocumentTreeView.AppendColumn(idColumn);
		DocumentTreeView.AppendColumn(dateColumn);
		DocumentTreeView.AppendColumn(nameColumn);

		idColumn.AddAttribute(idCell, "text", 0);
		dateColumn.AddAttribute(dateCell, "text", 1);
		nameColumn.AddAttribute(nameCell, "text", 2);
	}

	private void UpdateTreeView()
	{
		documentListStore = new Gtk.TreeStore(typeof(string), typeof(string), typeof(string));//Id, fecha, nombre
		DocumentTreeView.Model = documentListStore;

		if (categorySelectedInComboBox != null)
		{
			foreach (var doc in categorySelectedInComboBox)
			{
				documentListStore.AppendValues(doc.Id.ToString("0000"), doc.RegistrationDate.ToShortDateString(), doc.Name);
			}
		}
		else
		{
		}
	}

	protected void OnOpenButtonClicked(object sender, EventArgs e)
	{
		if (categorySelectedInComboBox != null)
		{
			if (documentSelectedInTreeView != null)
			{
				string p = System.IO.Path.Combine(registry.BasePath, categorySelectedInComboBox.GetDirectionString(), categorySelectedInComboBox.ToString(), documentSelectedInTreeView.ToString());
				System.Diagnostics.Process.Start(FILE_MANAGER, "\"file://" + p + "\"");
			}
		}
	}

	protected void OnDeleteButtonClicked(object sender, EventArgs e)
	{
		if (categorySelectedInComboBox != null)
		{
			if (documentSelectedInTreeView != null)
			{
				string p = System.IO.Path.Combine(registry.BasePath, categorySelectedInComboBox.GetDirectionString(), categorySelectedInComboBox.ToString(), documentSelectedInTreeView.ToString());
				string messageString = Mono.Unix.Catalog.GetString("Are you sure you want to delete this document?\n <b> \"{0}\"</b>.\n This operation <b>can not be undone</b>");
				Gtk.MessageDialog msg = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.YesNo, true, string.Format(messageString, p));
				if ((ResponseType)msg.Run() == ResponseType.Yes)
				{
					documentSelectedInTreeView.Delete();
					UpdateTreeView();
					UpdateCategoryComboBox();
				}
				msg.Destroy();
			}
		}
	}

	protected void OnNewActionActivated(object sender, EventArgs e)
	{
		AddDocument nd = new AddDocument(registry);

		nd.Icon = programIcon;
		nd.TransientFor = this;
		ResponseType resp = (ResponseType)nd.Run();
        
		if (resp == ResponseType.Ok)
		{
			UpdateTreeView();
			UpdateCategoryComboBox();
			registry.Write();
		}

		nd.Destroy();
	}

	protected void OnInfoActionActivated(object sender, EventArgs e)
	{
		Gtk.AboutDialog about = new AboutDialog();
		about.ProgramName = "Secretaría Electrial";
		about.Authors = new string[]{ "Rafael Bailón-Ruiz <rafaelbailon@ieee.org>" };
		about.TranslatorCredits = "English:\n\tRafael Bailón-Ruiz <rafaelbailon@ieee.org>\nEspañol:\n\tRafael Bailón-Ruiz <rafaelbailon@ieee.org>\nFrançais:\n\tRafael Bailón-Ruiz <rafaelbailon@ieee.org>";
		about.Copyright = "Copyright © 2013-2014 Asociación Electrial";
		about.WebsiteLabel = "http://www.ugr.es/~electrial";
		about.Version = "0.9.92";
		about.Icon = programIcon;
		about.Logo = programIcon;
		about.License = "This program is free software: you can redistribute it and/or modify\nit under the terms of the GNU General Public License as published by\nthe Free Software Foundation, either version 3 of the License, or\n(at your option) any later version.\n\nThis program is distributed in the hope that it will be useful,\nbut WITHOUT ANY WARRANTY; without even the implied warranty of\nMERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the\nGNU General Public License for more details.\n\nYou should have received a copy of the GNU General Public License\nalong with this program.  If not, see <http://www.gnu.org/licenses/>.\n";
		about.Modal = true;
		about.TransientFor = this;
		if ((ResponseType)about.Run() == ResponseType.Cancel)
		{

		}
		about.Destroy();
	}

	protected void OnPreferencesActionActivated(object sender, EventArgs e)
	{
		Preferences pref = new Preferences(settings,registry);
		pref.Icon = programIcon;
		pref.TransientFor = this;
		if ((ResponseType)pref.Run() == ResponseType.Close)
		{
			OnRefreshActionActivated(pref, new EventArgs());
		}
		pref.Destroy();
	}

	protected void OnEmailButtonClicked(object sender, EventArgs e)
	{
		if (categorySelectedInComboBox != null)
		{
			if (documentSelectedInTreeView != null)
			{
				string p = System.IO.Path.Combine(registry.BasePath, categorySelectedInComboBox.GetDirectionString(), categorySelectedInComboBox.ToString(), documentSelectedInTreeView.ToString());

				/*Selecionar archivos*/
				FileChooserDialog fileChooser = new FileChooserDialog(Mono.Unix.Catalog.GetString("Selecciona una Carpeta"), this, FileChooserAction.Open, Gtk.Stock.Cancel, Gtk.ResponseType.Cancel, Gtk.Stock.Open, Gtk.ResponseType.Ok);
				fileChooser.Modal = true;
				fileChooser.TypeHint = Gdk.WindowTypeHint.Dialog;
				fileChooser.WindowPosition = Gtk.WindowPosition.CenterOnParent;
				fileChooser.TransientFor = this;
				fileChooser.SelectMultiple = true;
				fileChooser.SetCurrentFolder(p);
				if ((Gtk.ResponseType)fileChooser.Run() == Gtk.ResponseType.Ok)
				{
					Emailer emailer = new Emailer(XdgEmailResponse);
					emailer.Subject = documentSelectedInTreeView.Name;
					emailer.Attach = fileChooser.Filenames;
					emailer.Execute();
					fileChooser.Destroy();
				}
				else
				{
					fileChooser.Destroy();
				}
			}
		}
	}

	private void XdgEmailResponse(Emailer.ExitCode exitCode)
	{
		System.Console.WriteLine("¡It Works!");
	}

	private bool FilterTree(Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		string item = model.GetValue(iter, (int)Column.Name).ToString();

		if (searchEntry.Text == "")
		{
			return true;
		}
		if (item.Contains(searchEntry.Text.ToLower()))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	protected void OnSearchEntryChanged(object sender, EventArgs e)
	{
		filt = new TreeModelFilter(documentListStore, null);
		filt.VisibleFunc = new Gtk.TreeModelFilterVisibleFunc(FilterTree);
		DocumentTreeView.Model = filt;
		filt.Refilter();
	}

	protected void OnRefreshActionActivated(object sender, EventArgs e)
	{
		Reload();

		UpdateCategoryComboBox();
		OnCategoryComboBoxChanged(categoryComboBox, new EventArgs());

		UpdateTreeView();
	}

	protected void OnClearButtonClicked(object sender, EventArgs e)
	{
		searchEntry.DeleteText(0, searchEntry.Text.Length);
	}
}
