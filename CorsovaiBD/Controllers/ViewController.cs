using AppKit;
using Foundation;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

using CorsovaiBD.Models;
using CoreGraphics;

namespace CorsovaiBD
{
    public partial class ViewController : NSViewController
    {
        private readonly List<string> tableNames = new List<string>();
        public static string SelectedTableName;
        private readonly MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = ConfigurationManager.AppSettings["Server"],
            UserID = ConfigurationManager.AppSettings["UserID"],
            Password = ConfigurationManager.AppSettings["Password"],
            Database = ConfigurationManager.AppSettings["Database"]
        };
        private MyTableDataSource dataSource;
        private void LoadTableNames()
        {
            using var connection = new MySqlConnection(builder.ConnectionString);
            connection.Open();
            MySqlCommand command = new MySqlCommand("SHOW TABLES", connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string tableName = reader.GetString(0);
                tableNames.Add(tableName);
            }
        }
        private void ShowSelectedTable(object sender, EventArgs e)
        {
            SelectedTableName = tableNames[(int)TableComboBox.SelectedIndex];
            var query = $"SELECT * FROM {SelectedTableName}";

            using var connection = new MySqlConnection(builder.ConnectionString);
            connection.Open();

            var adapter = new MySqlDataAdapter(query, connection);
            var ds = new DataSet();
            adapter.Fill(ds);

            var table = ds.Tables[0];

            while (TableView.TableColumns().Length > 0)
            {
                TableView.RemoveColumn(TableView.TableColumns()[0]);
            }


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

            dataSource = new MyTableDataSource(table);
            TableView.DataSource = dataSource;
            TableView.ReloadData();

           
        }
        public ViewController(IntPtr handle) : base(handle)
        {
        }



        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Do any additional setup after loading the view.
            LoadTableNames();

            TableComboBox.UsesDataSource = true;
            TableComboBox.DataSource = new ComboBoxDataSource(tableNames, TableComboBox);
            TableComboBox.Activated += ShowSelectedTable;

            TableView.ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.Uniform;
            TableView.SizeToFit();
        }

    }

    
}
