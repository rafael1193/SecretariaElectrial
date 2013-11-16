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
using SecretariaDataBase.FileSystem;

namespace SecretariaDataBase
{
    public partial class NewRecord : Gtk.Dialog
    {
        List<Box> boxes;
        Document newDocument;
        Box selectedBox;
        int insertingPosition;

        public Document NewDocument
        {
            get { return newDocument;}
        }

        public Box SelectedBox
        {
            get { return selectedBox;}
        }

        public int InsertingPosition
        {
            get { return insertingPosition;}
        }

        public NewRecord(Gtk.Window parent,List<SecretariaDataBase.FileSystem.Box> availableBoxes)
        {
            this.Build();
			this.TransientFor = parent;
            boxes = availableBoxes;

            foreach (var item in boxes)
            {
                this.categoryCombobox.AppendText(item.DirectionString + "/" + item.Name);
            }
        }

        protected void OnButtonCancelClicked(object sender, EventArgs e)
        {
            this.Respond(Gtk.ResponseType.Cancel);
        }

        protected void OnButtonOkClicked(object sender, EventArgs e)
        {
            if (categoryCombobox.Active > -1)
            {
                string[] splited = categoryCombobox.ActiveText.Split('/');
                string direction = splited [0];
                string box = splited [1];

                SecretariaDataBase.FileSystem.Box resultingBox = boxes.Find(x => { 
                    if (x.Name == box /*Check also direction?*/)
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                }
                );

                SecretariaDataBase.FileSystem.Document documentExists = resultingBox.Documents.Find(x => { 
                    if (x.Name == nameEntry.Text /*Check also direction?*/)
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                }
                );

                if (documentExists == null) //El nombre no está repetido
                {
                    if (!string.IsNullOrEmpty(idEntry.Text))
                    {
                        int newPos = resultingBox.GetPositionForNewDocument(int.Parse(idEntry.Text));

                        if (newPos >= 0)
                        {
                            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                            {
                                if (nameEntry.Text.Contains(c.ToString()))
                                {
                                    return;
                                }
                            }
                            //Que la ventana que llama se encargue de añadir el nuevo documento al box elegido en la posicion elegida
                            newDocument = new SecretariaDataBase.FileSystem.Document(int.Parse(idEntry.Text), nameEntry.Text, registrationDateCalendar.GetDate(), null);
                            insertingPosition = newPos;
                            selectedBox = resultingBox;
                            this.Respond(Gtk.ResponseType.Ok);
                        }
                    }
                }

            }
        }

        protected void OnCategoryComboboxChanged(object sender, EventArgs e)
        {
            if (categoryCombobox.Active > -1)
            {
                string[] splited = categoryCombobox.ActiveText.Split('/');
                string direction = splited [0];
                string box = splited [1];

                SecretariaDataBase.FileSystem.Box selectedBox = boxes.Find(x => { 
                    if (x.Name == box /*Check also direction?*/)
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                }
                );

                if (selectedBox.Documents != null)
                {
                    selectedBox.SortDocuments();

                    if (selectedBox.Documents.Count > 0)
                    {
                        idEntry.Text = (selectedBox.Documents [selectedBox.Documents.Count - 1].Id + 1).ToString();
                    } else
                    {
                        idEntry.Text = "0";
                    }
                }
            }
        }
    }
}

