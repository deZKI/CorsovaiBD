using AppKit;
using Foundation;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

using CorsovaiBD.Models;
using CoreGraphics;
using DotNetEnv;
using System.Text;

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

        private DataTable table;

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

            table = ds.Tables[0];
       
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


            MakeColumnNameList(table);

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
            ReLoadTable();
        }

        partial void Search(NSObject sender)
        {
            DataTable filtrTable;
            string filtrValue;
            string filtrColumnnName;
            string filtr;
            if (SearchCondition.StringValue == string.Empty) {

                var alert = new NSAlert
                {
                    AlertStyle = NSAlertStyle.Critical,
                    InformativeText = "Не введено условие для поиска",
                    MessageText = "Ошибка"
                };
                alert.RunModal();
            }
            else
            {
                filtrValue = SearchCondition.StringValue;
                if (SearchColumnsComboBox.SelectedIndex == -1)
                {
                    var alert = new NSAlert
                    {
                        AlertStyle = NSAlertStyle.Critical,
                        InformativeText = "Не выбрано имя стобца для поиска",
                        MessageText = "Ошибка"
                    };
                    alert.RunModal();
                }
                else
                {
                    filtrColumnnName = SearchColumnsComboBox.SelectedValue.ToString();
                    filtr = filtrColumnnName + "='" + filtrValue + "'";

                    DataRow[] HelpDataRows = table.Select(filtr);
                    filtrTable = table.Clone();
                    foreach(var row in HelpDataRows)
                    {
                        filtrTable.ImportRow(row);
                    }
                    TableView.DataSource = new MyTableDataSource(filtrTable);
                    TableView.ReloadData();
                }
            }
        }

        partial void SearchReset(NSObject sender)
        {
            ReLoadTable();
            SearchCondition.StringValue = string.Empty;
            

        }
        partial void Check(NSObject sender)
        {
            if (sender is NSButton button)
            {
                Console.OutputEncoding = Encoding.UTF8;
                string buttonTitle = button.Title.ToString();
                Console.WriteLine(buttonTitle);
                // дальнейшая обработка значения кнопки
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
        private void ReLoadTable()
        {
            try
            {
                SelectedTableName = tableNames[(int)TableComboBox.SelectedIndex];
                Console.WriteLine(SelectedTableName);
                var query = $"SELECT * FROM {SelectedTableName}";

                using var connection = new MySqlConnection(builder.ConnectionString);
                connection.Open();

                var adapter = new MySqlDataAdapter(query, connection);
                var ds = new DataSet();
                adapter.Fill(ds);

                table = ds.Tables[0];
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
        private void MakeColumnNameList(DataTable Helptable)
        {

            SearchColumnsComboBox.RemoveAll();
            
            foreach (DataColumn HelpColumn in Helptable.Columns)
            {
                SearchColumnsComboBox.Add(new NSString(HelpColumn.ToString()));
            }
            SearchColumnsComboBox.StringValue = string.Empty;
            SearchColumnsComboBox.ReloadData();
        }
    }

    
}
