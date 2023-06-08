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
	[Register ("MainController")]
	partial class MainController
	{
		[Outlet]
		AppKit.NSButton addRowButton { get; set; }

		[Outlet]
		AppKit.NSButton editRowButton { get; set; }

		[Outlet]
		AppKit.NSButton excelButton { get; set; }

		[Outlet]
		AppKit.NSButton removeButton { get; set; }

		[Outlet]
		AppKit.NSComboBox searchColumnComboBox { get; set; }

		[Outlet]
		AppKit.NSTextField searchConditionLable { get; set; }

		[Outlet]
		AppKit.NSComboBox searchTypeComboBox { get; set; }

		[Outlet]
		AppKit.NSComboBox tableComboBox { get; set; }

		[Outlet]
		AppKit.NSTableView tableView { get; set; }

		[Outlet]
		AppKit.NSButton wordButton { get; set; }

		[Action ("reloadTableButton:")]
		partial void reloadTableButton (Foundation.NSObject sender);

		[Action ("removeRowButton:")]
		partial void removeRowButton (Foundation.NSObject sender);

		[Action ("resetSearchButton:")]
		partial void resetSearchButton (Foundation.NSObject sender);

		[Action ("searchButton:")]
		partial void searchButton (Foundation.NSObject sender);

		[Action ("selectSearchColumn:")]
		partial void selectSearchColumn (Foundation.NSObject sender);

		[Action ("toExcelButton:")]
		partial void toExcelButton (Foundation.NSObject sender);

		[Action ("toWordButton:")]
		partial void toWordButton (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (wordButton != null) {
				wordButton.Dispose ();
				wordButton = null;
			}

			if (excelButton != null) {
				excelButton.Dispose ();
				excelButton = null;
			}

			if (addRowButton != null) {
				addRowButton.Dispose ();
				addRowButton = null;
			}

			if (editRowButton != null) {
				editRowButton.Dispose ();
				editRowButton = null;
			}

			if (removeButton != null) {
				removeButton.Dispose ();
				removeButton = null;
			}

			if (searchColumnComboBox != null) {
				searchColumnComboBox.Dispose ();
				searchColumnComboBox = null;
			}

			if (searchConditionLable != null) {
				searchConditionLable.Dispose ();
				searchConditionLable = null;
			}

			if (searchTypeComboBox != null) {
				searchTypeComboBox.Dispose ();
				searchTypeComboBox = null;
			}

			if (tableComboBox != null) {
				tableComboBox.Dispose ();
				tableComboBox = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}
		}
	}
}
