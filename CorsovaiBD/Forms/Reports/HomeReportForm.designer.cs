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
	[Register ("HomeReportForm")]
	partial class HomeReportForm
	{
		[Outlet]
		AppKit.NSTextField blockField { get; set; }

		[Outlet]
		AppKit.NSTextField cityField { get; set; }

		[Outlet]
		AppKit.NSTextField districtField { get; set; }

		[Outlet]
		AppKit.NSTextField houseNumberField { get; set; }

		[Outlet]
		AppKit.NSTextField idField { get; set; }

		[Outlet]
		AppKit.NSTextField notificationField { get; set; }

		[Outlet]
		AppKit.NSTextField pathField { get; set; }

		[Outlet]
		AppKit.NSTextField streetField { get; set; }

		[Action ("createReport:")]
		partial void createReport (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (cityField != null) {
				cityField.Dispose ();
				cityField = null;
			}

			if (idField != null) {
				idField.Dispose ();
				idField = null;
			}

			if (streetField != null) {
				streetField.Dispose ();
				streetField = null;
			}

			if (houseNumberField != null) {
				houseNumberField.Dispose ();
				houseNumberField = null;
			}

			if (districtField != null) {
				districtField.Dispose ();
				districtField = null;
			}

			if (blockField != null) {
				blockField.Dispose ();
				blockField = null;
			}

			if (pathField != null) {
				pathField.Dispose ();
				pathField = null;
			}

			if (notificationField != null) {
				notificationField.Dispose ();
				notificationField = null;
			}
		}
	}
}
