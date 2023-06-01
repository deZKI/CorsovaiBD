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
		AppKit.NSButton AddRow { get; set; }

		[Outlet]
		AppKit.NSButton RedactButton { get; set; }

		[Outlet]
		AppKit.NSComboBox SearchColumnsComboBox { get; set; }

		[Outlet]
		AppKit.NSTextField SearchCondition { get; set; }

		[Outlet]
		AppKit.NSComboBox SearchTypeCombobox { get; set; }

		[Outlet]
		AppKit.NSComboBox TableComboBox { get; set; }

		[Outlet]
		AppKit.NSTableView TableView { get; set; }

		[Action ("Check:")]
		partial void Check (Foundation.NSObject sender);

		[Action ("DeleteRowButton:")]
		partial void DeleteRowButton (Foundation.NSObject sender);

		[Action ("ReloadButton:")]
		partial void ReloadButton (Foundation.NSObject sender);

		[Action ("Search:")]
		partial void Search (Foundation.NSObject sender);

		[Action ("SearchReset:")]
		partial void SearchReset (Foundation.NSObject sender);

		[Action ("SelectSeacrColumn:")]
		partial void SelectSeacrColumn (Foundation.NSObject sender);

		[Action ("SelectSearchColumn:")]
		partial void SelectSearchColumn (Foundation.NSObject sender);

		[Action ("SelectSearchType:")]
		partial void SelectSearchType (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (RedactButton != null) {
				RedactButton.Dispose ();
				RedactButton = null;
			}

			if (AddRow != null) {
				AddRow.Dispose ();
				AddRow = null;
			}

			if (SearchColumnsComboBox != null) {
				SearchColumnsComboBox.Dispose ();
				SearchColumnsComboBox = null;
			}

			if (SearchCondition != null) {
				SearchCondition.Dispose ();
				SearchCondition = null;
			}

			if (SearchTypeCombobox != null) {
				SearchTypeCombobox.Dispose ();
				SearchTypeCombobox = null;
			}

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
