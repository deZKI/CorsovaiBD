// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CorsovaiBD
{
	[Register ("LoginForm")]
	partial class LoginForm
	{
		[Outlet]
		AppKit.NSTextField passwordField { get; set; }

		[Outlet]
		AppKit.NSTextField usernameField { get; set; }

		[Action ("exitButton:")]
		partial void exitButton (Foundation.NSObject sender);

		[Action ("loginButton:")]
		partial void loginButton (Foundation.NSObject sender);

		[Action ("registrationFormButton:")]
		partial void registrationFormButton (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (passwordField != null) {
				passwordField.Dispose ();
				passwordField = null;
			}

			if (usernameField != null) {
				usernameField.Dispose ();
				usernameField = null;
			}
		}
	}
}
