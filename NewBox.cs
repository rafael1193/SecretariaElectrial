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
    public partial class NewBox : Gtk.Dialog
    {
        List<Box> boxes;
        Box newBox;
        int insertingPosition;

        public Box CreatedBox
        {
            get { return newBox;}
        }

        public int InsertingPosition
        {
            get { return insertingPosition;}
        }

        public NewBox(Gtk.Window parent, List<SecretariaDataBase.FileSystem.Box> availableBoxes)
        {
            this.Build();
			this.TransientFor = parent;

            boxes = availableBoxes;
            OnInRadiobuttonToggled(inRadiobutton, new EventArgs());
        }

        protected void OnButtonOkClicked (object sender, EventArgs e)
        {
                SecretariaDataBase.FileSystem.Box existingBox = boxes.Find(x => { 
                    if (x.Name == nameEntry.Text /*Check also direction?*/)
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                }
                );

                if (existingBox == null) //El nombre no está repetido
                {
                    if (!string.IsNullOrEmpty(idEntry.Text))
                    {
                        int newPos = Box.GetPositionForNewBox(boxes,int.Parse(idEntry.Text));

                        if (newPos >= 0)
                        {

                        BoxDirection newBoxDirection;
                        if(inRadiobutton.Active)
                        {
                            newBoxDirection= BoxDirection.In;
                        }else if (outRadiobutton.Active){
                            newBoxDirection= BoxDirection.Out;
                        }else{
                            newBoxDirection= BoxDirection.None;
                        }
                        foreach(char c in System.IO.Path.GetInvalidPathChars())
                        {
                            if (nameEntry.Text.Contains(c.ToString()))
                            {
                                return;
                            }
                        }
                        //Que la ventana que llama se encargue de añadir el nuevo documento al box elegido en la posicion elegida
                            newBox = new Box(newBoxDirection,int.Parse(idEntry.Text), nameEntry.Text,nameEntry.Text);
                            insertingPosition = newPos;
                            this.Respond(Gtk.ResponseType.Ok);
                        }
                    }
                }
        }        

        protected void OnInRadiobuttonToggled (object sender, EventArgs e)
        {
             if (inRadiobutton.Active)
            {
                Box b = boxes.FindLast(x => {
                    if (x.Direction == BoxDirection.In)
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                });

                if (b ==null)
                {
                    idEntry.Text = "0";
                }
                else
                {
                    idEntry.Text = (b.Code + 1).ToString();
                }
            }
        }

        protected void OnOutRadiobuttonToggled (object sender, EventArgs e)
        {
            if (outRadiobutton.Active)
            {
                Box b = boxes.FindLast(x => {
                    if (x.Direction == BoxDirection.Out)
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                });

                if (b ==null)
                {
                    idEntry.Text = "0";
                }
                else
                {
                    idEntry.Text = (b.Code + 1).ToString();
                }
            }
        }        
    }
}

