using System;
using AppKit;
using Foundation;
using System.Data;

namespace CorsovaiBD.Models
{
    public class MyTableDataSource : NSTableViewDataSource
    {
        private DataTable table;

        public MyTableDataSource(DataTable table)
        {
            this.table = table;
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return table.Rows.Count;
        }

        public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            var columnName = tableColumn.Identifier;
            var rowObject = table.Rows[(int)row];
            return new NSString(rowObject[columnName].ToString());
        }
    }
}

