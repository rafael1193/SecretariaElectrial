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
using Mono.Unix;

namespace SecretariaElectrial
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Application.Init();
			Catalog.Init("l10n", "./locale");
			MainWindow win = new MainWindow();
			win.Show();
			#if !DEBUG
			GLib.ExceptionManager.UnhandledException+= HandleUnhandledException;
			#endif
			Application.Run();

		}

		static void HandleUnhandledException(GLib.UnhandledExceptionArgs args)
		{
			Exception ex = args.ExceptionObject as Exception;
			string text = String.Format(Mono.Unix.Catalog.GetString("An unhandled exception has been thrown. Please, send this error report to your software maintainer in order to prevent more errors in the future.\n\n{0}: {1}\n{2}"), ex.InnerException.GetType().ToString(), ex.InnerException.Message.ToString(), ex.InnerException.StackTrace.ToString());
			Gtk.MessageDialog msg = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, text);
			msg.UseMarkup = false;
			msg.Title = Mono.Unix.Catalog.GetString("Unhandled exception thrown in Secretaria Electrial");
			if ((ResponseType)msg.Run() == ResponseType.Ok)
			{
				Emailer em = new Emailer(x =>
				{
				});
				em.Subject = "Unhandled exception thrown in Secretaria Electrial";
				em.Body = text;
				em.Execute();
			}
			msg.Destroy();
		}
	}
}
