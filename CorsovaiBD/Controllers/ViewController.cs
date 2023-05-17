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
        public static readonly MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
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

        partial void DeleteRowButton(NSObject sender)
        {
            try { 
            var selectedRow = TableView.SelectedRow;
            var query = $"SELECT * FROM {SelectedTableName}";

            using var connection = new MySqlConnection(builder.ConnectionString);
            connection.Open();

            var adapter = new MySqlDataAdapter(query, connection);
            var ds = new DataSet();
            adapter.Fill(ds);
            var table = ds.Tables[0];


                // Создаем объект MySqlCommandBuilder на основе адаптера, чтобы автоматически генерировать InsertCommand
            var builderr = new MySqlCommandBuilder(adapter);
            ds.Tables[0].Rows[((int)selectedRow)].Delete();
            adapter.Update(ds.Tables[0]);
            ds.Tables[0].AcceptChanges();
            dataSource = new MyTableDataSource(table);
            TableView.DataSource = dataSource;
            TableView.ReloadData();

            }
            catch (Exception ex)
            {
                // Отображаем окно с ошибкой
                var alert = new NSAlert
                {
                    AlertStyle = NSAlertStyle.Critical,
                    InformativeText = ex.Message,
                    MessageText = "Ошибка"
                };
                alert.RunModal();
            }
        }

        partial void ReloadButton(NSObject sender)
        {
            try
            {
                SelectedTableName = tableNames[(int)TableComboBox.SelectedIndex];
                var query = $"SELECT * FROM {SelectedTableName}";

                using var connection = new MySqlConnection(builder.ConnectionString);
                connection.Open();

                var adapter = new MySqlDataAdapter(query, connection);
                var ds = new DataSet();
                adapter.Fill(ds);

                var table = ds.Tables[0];
                dataSource = new MyTableDataSource(table);
                TableView.DataSource = dataSource;
                TableView.ReloadData();
            }
            catch (Exception ex)
            {
                // Отображаем окно с ошибкой
                var alert = new NSAlert
                {
                    AlertStyle = NSAlertStyle.Critical,
                    InformativeText = ex.Message,
                    MessageText = "Ошибка"
                };
                alert.RunModal();
            }
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
