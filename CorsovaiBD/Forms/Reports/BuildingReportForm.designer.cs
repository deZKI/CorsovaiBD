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
	[Register ("BuildingReportForm")]
	partial class BuildingReportForm
	{
		[Outlet]
		AppKit.NSTextField blockField { get; set; }

		[Outlet]
		AppKit.NSTextField cityField { get; set; }

		[Outlet]
		AppKit.NSTextField costField { get; set; }

		[Outlet]
		AppKit.NSTextField dateField { get; set; }

		[Outlet]
		AppKit.NSTextField districtField { get; set; }

		[Outlet]
		AppKit.NSTextField houseField { get; set; }

		[Outlet]
		AppKit.NSTextField idField { get; set; }

		[Outlet]
		AppKit.NSButton inhabitedField { get; set; }

		[Outlet]
		AppKit.NSButton legalityField { get; set; }

		[Outlet]
		AppKit.NSTextField squareField { get; set; }

		[Outlet]
		AppKit.NSTextField storeyField { get; set; }

		[Outlet]
		AppKit.NSTextField streetField { get; set; }

		[Outlet]
		AppKit.NSTextField typeField { get; set; }

		[Outlet]
		AppKit.NSTextField wallField { get; set; }

		[Action ("createReport:")]
		partial void createReport (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (idField != null) {
				idField.Dispose ();
				idField = null;
			}

			if (typeField != null) {
				typeField.Dispose ();
				typeField = null;
			}

			if (legalityField != null) {
				legalityField.Dispose ();
				legalityField = null;
			}

			if (squareField != null) {
				squareField.Dispose ();
				squareField = null;
			}

			if (inhabitedField != null) {
				inhabitedField.Dispose ();
				inhabitedField = null;
			}

			if (wallField != null) {
				wallField.Dispose ();
				wallField = null;
			}

			if (costField != null) {
				costField.Dispose ();
				costField = null;
			}

			if (storeyField != null) {
				storeyField.Dispose ();
				storeyField = null;
			}

			if (cityField != null) {
				cityField.Dispose ();
				cityField = null;
			}

			if (streetField != null) {
				streetField.Dispose ();
				streetField = null;
			}

			if (houseField != null) {
				houseField.Dispose ();
				houseField = null;
			}

			if (districtField != null) {
				districtField.Dispose ();
				districtField = null;
			}

			if (blockField != null) {
				blockField.Dispose ();
				blockField = null;
			}

			if (dateField != null) {
				dateField.Dispose ();
				dateField = null;
			}
		}
	}
}
