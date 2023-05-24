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
		AppKit.NSButton rbBegin { get; set; }

		[Outlet]
		AppKit.NSButton rbContain { get; set; }

		[Outlet]
		AppKit.NSButton rbEqual { get; set; }

		[Outlet]
		AppKit.NSButton rbLess { get; set; }

		[Outlet]
		AppKit.NSButton rbLessEqual { get; set; }

		[Outlet]
		AppKit.NSButton rbMore { get; set; }

		[Outlet]
		AppKit.NSButton rbMoreEqual { get; set; }

		[Outlet]
		AppKit.NSComboBox SearchColumnsComboBox { get; set; }

		[Outlet]
		AppKit.NSTextField SearchCondition { get; set; }

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
		
		void ReleaseDesignerOutlets ()
		{
			if (AddRow != null) {
				AddRow.Dispose ();
				AddRow = null;
			}

			if (rbBegin != null) {
				rbBegin.Dispose ();
				rbBegin = null;
			}

			if (rbContain != null) {
				rbContain.Dispose ();
				rbContain = null;
			}

			if (rbEqual != null) {
				rbEqual.Dispose ();
				rbEqual = null;
			}

			if (rbLess != null) {
				rbLess.Dispose ();
				rbLess = null;
			}

			if (rbLessEqual != null) {
				rbLessEqual.Dispose ();
				rbLessEqual = null;
			}

			if (rbMore != null) {
				rbMore.Dispose ();
				rbMore = null;
			}

			if (rbMoreEqual != null) {
				rbMoreEqual.Dispose ();
				rbMoreEqual = null;
			}

			if (SearchColumnsComboBox != null) {
				SearchColumnsComboBox.Dispose ();
				SearchColumnsComboBox = null;
			}

			if (SearchCondition != null) {
				SearchCondition.Dispose ();
				SearchCondition = null;
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
