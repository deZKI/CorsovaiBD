using AppKit;
using Foundation;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

using CorsovaiBD.Models;
namespace CorsovaiBD
{
    public partial class ViewController : NSViewController
    {
        private readonly List<string> tableNames = new List<string>();
        private readonly MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = ConfigurationManager.AppSettings["Server"],
            UserID = ConfigurationManager.AppSettings["UserID"],
            Password = ConfigurationManager.AppSettings["Password"],
            Database = ConfigurationManager.AppSettings["Database"]
        };
        private MyTableDataSource dataSource;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            LoadTableNames();
           
            ComboBox.UsesDataSource = true;
            ComboBox.DataSource = new ComboBoxDataSource(tableNames, ComboBox);
            ComboBox.Activated += ComboBox_Activated;

            TableView.ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.Uniform;
            TableView.SizeToFit();
        }

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

        private void ComboBox_Activated(object sender, EventArgs e)
        {
            var selectedTableName = tableNames[(int)ComboBox.SelectedIndex];
            var query = $"SELECT * FROM {selectedTableName}";

            using var connection = new MySqlConnection(builder.ConnectionString);
            connection.Open();

            var command = new MySqlCommand(query, connection);
            var reader = command.ExecuteReader();

            while (TableView.TableColumns().Length > 0)
            {
                TableView.RemoveColumn(TableView.TableColumns()[0]);
            }
            var dataRows = new DataTable();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var columnType = reader.GetFieldType(i);
                dataRows.Columns.Add(new DataColumn(columnName, columnType));

                var columnId = new NSString(columnName);
                var tableColumn = new NSTableColumn(columnId)
                {
                    HeaderCell = new NSTableHeaderCell { Title = columnName },
                    Identifier = columnName
                };
                TableView.AddColumn(tableColumn);
            }

            
            while (reader.Read())
            {
                var row = dataRows.NewRow();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[i] = reader.GetValue(i);
                }
                dataRows.Rows.Add(row);
            }

            dataSource = new MyTableDataSource(dataRows);
            TableView.DataSource = dataSource;
            TableView.ReloadData();

        }

    }


}
