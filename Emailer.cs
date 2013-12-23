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
using System.Text;
using System.Collections.Generic;

namespace SecretariaElectrial
{
	public class Emailer
	{
		public enum ExitCode
		{
			Success = 0,
			SyntaxError = 1,
			FileDontExist = 2,
			SoftwareNotFound = 3,
			ActionFailed = 4,
			InsufficientFilePermissions = 5
		}

		const string EMAIL_COMMAND = "thunderbird";

		private string[] cc;
		private string[] bcc;
		private string subject;
		private string body;
		private string[] attach;
		private string[] to;
		private System.Diagnostics.Process proc;
		public delegate void OnExit (ExitCode exitCode);

		OnExit onExitCallback;

		public string[] Cc {
			get{ return cc;}
			set{ cc = value;}
		}

		public string[] Bcc {
			get{ return bcc;}
			set{ bcc = value;}
		}

		public string Subject {
			get{ return subject;}
			set{ subject = value;}
		}

		public string Body {
			get{ return body;}
			set{ body = value;}
		}

		public string[] Attach {
			get{ return attach;}
			set{ attach = value;}
		}

		public string[] To {
			get{ return to;}
			set{ to = value;}
		}

		public Emailer (OnExit onExitCallback)
		{
			this.onExitCallback = onExitCallback;
		}

		public void Execute ()
		{
			StringBuilder args = new StringBuilder ("-compose \"");

			//TO
			args.Append ("to='");
			if (to != null) {
				if (to.Length > 0) {
					for (int i = 0; i < to.Length -1; ++i) {
						args.Append (to [i]).Append (",");
					}
					args.Append (to [to.Length - 1]);
				}
			}
			args.Append ("',");

			//CC
			args.Append ("cc='");
			if (cc != null) {
				if (cc.Length > 0) {
					for (int i = 0; i < cc.Length -1; ++i) {
						args.Append (cc [i]).Append (",");
					}
					args.Append (cc [cc.Length - 1]);
				}
			}
			args.Append ("',");
			//BCC
			args.Append ("bcc='");
			if (bcc != null) {
				if (bcc.Length > 0) {
					for (int i = 0; i < bcc.Length -1; ++i) {
						args.Append (bcc [i]).Append (",");
					}
					args.Append (bcc [bcc.Length - 1]);
				}
			}
			args.Append ("',");

			//SUBJECT
			args.Append ("subject='");
			if (!String.IsNullOrEmpty (subject)) {
				args.Append (subject);
			}
			args.Append ("',");

			//BODY
			args.Append ("body='");
			if (!String.IsNullOrEmpty (body)) {
				args.Append (body);
			}
			args.Append ("',");

			//ATTACHMENT
			args.Append ("attachment='");
			if (attach != null) {
				if (attach.Length > 0) {
					for (int i = 0; i < attach.Length -1; ++i) {
						args.Append (attach [i]).Append (",");
					}
					args.Append (attach [attach.Length - 1]);
				}
			}
			args.Append ("'\"");
            
			try {
				proc = System.Diagnostics.Process.Start (EMAIL_COMMAND, args.ToString ());
				proc.Exited += HandleExited;
			} catch (System.ComponentModel.Win32Exception e) {
				if (e.NativeErrorCode == (int)ExitCode.FileDontExist) {
					if (onExitCallback != null) {
				onExitCallback (ExitCode.FileDontExist);
			}
				} else if (e.NativeErrorCode == (int)ExitCode.InsufficientFilePermissions) {
					if (onExitCallback != null) {
				onExitCallback (ExitCode.InsufficientFilePermissions);
			}
				}
			}
		}

		void HandleExited (object sender, EventArgs e)
		{
			if (onExitCallback != null) {
				onExitCallback ((ExitCode)proc.ExitCode);
			}
		}
	}
}

