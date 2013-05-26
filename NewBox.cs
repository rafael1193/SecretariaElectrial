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

        public NewBox(List<SecretariaDataBase.FileSystem.Box> availableBoxes)
        {
            this.Build();

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

