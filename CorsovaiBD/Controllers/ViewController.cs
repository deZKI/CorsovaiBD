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
        public static int selectedRowIndex;
        public static int selectedRowId;
        private DataTable table;
        private MyTableDataSource dataSource;
        private string selectedColumnType;

        public static void selectTable()
        {

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
        private void ShowSelectedTable(object sender, EventArgs e)
        {
            try
            {
                RedactButton.Enabled = false;
                SelectedTableName = tableNames[(int)TableComboBox.SelectedIndex];
            }
            catch (Exception ex)
            {
                var alert = new NSAlert
                {
                    AlertStyle = NSAlertStyle.Critical,
                    InformativeText = "Выберите таблицу из списка",
                    MessageText = "Ошибка"
                };
                alert.RunModal();
            }
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

            SearchTypeCombobox.RemoveAll();
            SearchTypeCombobox.StringValue = string.Empty;

        }
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        partial void DeleteRowButton(NSObject sender)
        {
            try
            {
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

            if (SearchCondition.StringValue == string.Empty)
            {

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
                    string filtrColumnName = SearchColumnsComboBox.SelectedValue.ToString();
                    string filtrValue = SearchCondition.StringValue;
                    string type = SearchTypeCombobox.SelectedValue.ToString();
                    SelectedTableName = tableNames[(int)TableComboBox.SelectedIndex];
                    string filtr = "";
                    if (selectedColumnType == "System.DateTime")
                    {
                        DateTime result_date;
                        bool success_data = DateTime.TryParse(filtrValue, out result_date);
                        if (!success_data)
                        {
                            var alert = new NSAlert
                            {
                                AlertStyle = NSAlertStyle.Critical,
                                InformativeText = "Ввели не дату, а колонка с датой",
                                MessageText = "Ошибка"
                            };
                            alert.RunModal();
                        }
                        return;
                    }

                    switch (type)
                    {


                        case "По равенству":
                            filtr = "select * from " + SelectedTableName + " where " + filtrColumnName + " = @FiltrValue";
                            break;
                        case "По вхождению":
                            filtr = "select * from " + SelectedTableName + " where " + filtrColumnName + " LIKE '%" + filtrValue + "%'";
                            break;
                        case "Начинается с":
                            filtr = "select * from " + SelectedTableName + " where " + filtrColumnName + " LIKE '" + filtrValue + "%'";
                            break;
                        case "Больше":
                            filtr = "select * from " + SelectedTableName + " where " + filtrColumnName + " > " + "@FiltrValue";
                            break;
                        case "Больше равно":
                            filtr = "select * from " + SelectedTableName + " where " + filtrColumnName + " >= @FiltrValue";
                            break;
                        case "Меньше":
                            filtr = "select * from " + SelectedTableName + " where " + filtrColumnName + "<" + "@FiltrValue";
                            break;
                        case "Меньше равно":
                            filtr = "select * from " + SelectedTableName + " where " + filtrColumnName + "<= @FiltrValue";
                            break;
                    }
                    try
                    {

                        using var connection = new MySqlConnection(builder.ConnectionString);
                        connection.Open();

                        var command = new MySqlCommand(filtr, connection);
                        command.Parameters.AddWithValue("@FiltrValue", filtrValue);
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
                    catch (Exception ex)
                    {
                        var alert = new NSAlert
                        {
                            AlertStyle = NSAlertStyle.Critical,
                            InformativeText = ex.Message,
                            MessageText = "Ошибка"
                        };
                        alert.RunModal();
                    }

                }
            }
        }

        partial void SearchReset(NSObject sender)
        {
            ReLoadTable();
            SearchCondition.StringValue = string.Empty;


        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Do any additional setup after loading the view.
            LoadTableNames();

            TableComboBox.UsesDataSource = true;
            TableComboBox.DataSource = new ComboBoxDataSource(tableNames, TableComboBox);
            TableComboBox.Activated += ShowSelectedTable;
            RedactButton.Enabled = false;


            TableView.ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.Uniform;
            TableView.SizeToFit();
            TableView.Activated += getSelectedRow;
        }
        private void getSelectedRow(object sender, EventArgs e)
        {
            try
            {
                selectedRowIndex = (int)TableView.SelectedRow;
                selectedRowId = (int)table.Rows[selectedRowIndex]["id"];
                RedactButton.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

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

        partial void SelectSearchColumn(NSObject sender)
        {

            selectedColumnType = table.Columns[((int)SearchColumnsComboBox.SelectedIndex)].DataType.ToString();
            SearchTypeCombobox.RemoveAll();
            SearchTypeCombobox.StringValue = string.Empty;

            SearchTypeCombobox.Add(new NSString("По равенству"));
            switch (selectedColumnType)
            {
                case "System.Int32":
                    SearchTypeCombobox.Add(new NSString("Больше"));
                    SearchTypeCombobox.Add(new NSString("Меньше"));
                    SearchTypeCombobox.Add(new NSString("Больше равно"));
                    SearchTypeCombobox.Add(new NSString("Меньше равно"));
                    break;


                case "System.String" or "System.DateTime":
                    SearchTypeCombobox.Add(new NSString("По вхождению"));
                    SearchTypeCombobox.Add(new NSString("Начинается с"));
                    break;
            }

            SearchTypeCombobox.SelectItem(0);
            SearchColumnsComboBox.ReloadData();


        }
    }


}
