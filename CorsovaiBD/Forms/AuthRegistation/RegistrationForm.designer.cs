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
	[Register ("RegistrationForm")]
	partial class RegistrationForm
	{
		[Outlet]
		AppKit.NSTextField passwordField { get; set; }

		[Outlet]
		AppKit.NSTextField repeatPasswordField { get; set; }

		[Outlet]
		AppKit.NSTextField usernameField { get; set; }

		[Action ("exitButton:")]
		partial void exitButton (Foundation.NSObject sender);

		[Action ("loginFormButton:")]
		partial void loginFormButton (Foundation.NSObject sender);

		[Action ("registrationButton:")]
		partial void registrationButton (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (usernameField != null) {
				usernameField.Dispose ();
				usernameField = null;
			}

			if (passwordField != null) {
				passwordField.Dispose ();
				passwordField = null;
			}

			if (repeatPasswordField != null) {
				repeatPasswordField.Dispose ();
				repeatPasswordField = null;
			}
		}
	}
}
