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
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSComboBox TableComboBox { get; set; }

		[Outlet]
		AppKit.NSTableView TableView { get; set; }

		[Action ("initDatabase:")]
		partial void initDatabase (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (TableComboBox != null) {
				TableComboBox.Dispose ();
				TableComboBox = null;
			}

			if (TableView != null) {
				TableView.Dispose ();
				TableView = null;
			}
		}
	}
}
