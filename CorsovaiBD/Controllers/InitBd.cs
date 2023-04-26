using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;

using AppKit;
using Foundation;
using MySqlConnector;

namespace CorsovaiBD
{
    public partial class ViewController : NSViewController
    {

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //// Do any additional setup after loading the view.
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }

        partial void initDatabase(NSButton sender)
        {


            var builder = new MySqlConnectionStringBuilder
            {
                Server = ConfigurationManager.AppSettings["Server"],
                UserID = ConfigurationManager.AppSettings["UserID"],
                Password = ConfigurationManager.AppSettings["Password"],
                Database = ConfigurationManager.AppSettings["Database"]
            };
            string query = "SELECT * FROM Addresses";
            MySqlDataAdapter adapter = new MySqlDataAdapter(query, builder.ConnectionString);
            DataSet ds = new DataSet("Lab_11_ver_@");
            adapter.Fill(ds);

            var table = ds.Tables[0];

            foreach (DataColumn column in table.Columns)
            {
                var columnId = new NSString(column.ColumnName);
                var tableColumn = new NSTableColumn(columnId)
                {
                    HeaderCell = new NSTableHeaderCell { Title = column.ColumnName },
                    Identifier = column.ColumnName
                };
                TableView.AddColumn(tableColumn);
            }

            var dataSource = new MyTableDataSource(table);
            TableView.DataSource = dataSource;

        }



        }
}

public class MyTableDataSource : NSTableViewDataSource
{
    private readonly DataTable _table;

    public MyTableDataSource(DataTable table)
    {
        _table = table;
    }

    public override nint GetRowCount(NSTableView tableView)
    {
        return _table.Rows.Count;
    }

    public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, nint row)
    {
        string columnName = tableColumn.Identifier;
        return new NSString(_table.Rows[(int)row][columnName].ToString());
    }
}

