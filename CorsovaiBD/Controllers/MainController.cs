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
	public partial class MainController : NSViewController
	{
        private readonly List<string> tableNames = new List<string>();

        public static string selectedTableName;
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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Do any additional setup after loading the view.
            loadTableNames();

            tableComboBox.UsesDataSource = true;
            tableComboBox.DataSource = new ComboBoxDataSource(tableNames, tableComboBox);
            tableComboBox.Activated += showSelectedTable;

            editRowButton.Enabled = false;
            addRowButton.Enabled = false;
            removeButton.Enabled = false;

            tableView.ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.Uniform;
            tableView.SizeToFit();
            tableView.Activated += getSelectedRow;
        }

        private void loadTableNames()
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
        private void showSelectedTable(object sender, EventArgs e)
        {
            try
            {
                editRowButton.Enabled = false;
                selectedTableName = tableNames[(int)tableComboBox.SelectedIndex];
                addRowButton.Enabled = true;
                removeButton.Enabled = false;
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
            var query = $"SELECT * FROM {selectedTableName}";

            using var connection = new MySqlConnection(builder.ConnectionString);
            connection.Open();

            var adapter = new MySqlDataAdapter(query, connection);
            var ds = new DataSet();
            adapter.Fill(ds);

            table = ds.Tables[0];

            while (tableView.TableColumns().Length > 0)
            {
                tableView.RemoveColumn(tableView.TableColumns()[0]);
            }


            foreach (DataColumn column in table.Columns)
            {
                var columnId = new NSString(column.ColumnName);
                var tableColumn = new NSTableColumn(columnId)
                {
                    HeaderCell = new NSTableHeaderCell { Title = column.ColumnName },
                    Identifier = column.ColumnName
                };
                if (column.ColumnName == "Photo")
                {
                    // Create a button cell for the column
                    var buttonCell = new NSButtonCell
                    {
                        Title = "Show Photo",
                        Action = new ObjCRuntime.Selector("ShowPhotoButtonClicked:"),
                        Target = this // Set the target to the current instance
                    };

                    // Set the button cell as the data cell for the column
                    tableColumn.DataCell = buttonCell;
                }

                tableView.AddColumn(tableColumn);

            }

            dataSource = new MyTableDataSource(table);
            tableView.DataSource = dataSource;
            tableView.ReloadData();


            makeColumnNameList(table);

            searchTypeComboBox.RemoveAll();
            searchTypeComboBox.StringValue = string.Empty;

        }

        [Action("ShowPhotoButtonClicked:")]
        private void ShowPhotoButtonClicked(NSObject sender)
        {

            // Get the clicked row
            var clickedRow = tableView.ClickedRow;

            // Retrieve the photo data for the clicked row
            var photoData = GetPhotoDataForRowIndex(((int)clickedRow));

            // Show the photo in another window
            ShowPhotoWindow(photoData);
        }

        private byte[] GetPhotoDataForRowIndex(int rowIndex)
        {
            // Implement your logic to retrieve the photo data for the given row index
            // Replace this with your actual code to fetch the photo data

            // Assuming you have a DataTable named 'table' and a column named 'photo'
            var row = table.Rows[rowIndex];
            var photoData = (byte[])row["photo"];

            return photoData;
        }

        private void ShowPhotoWindow(byte[] photoData)
        {
            var viewController = this.Storyboard.InstantiateControllerWithIdentifier("PhotoViewController") as PhotoViewController;
            if (viewController != null)
            {
                viewController.loadPhoto(photoData);
                PresentViewControllerAsModalWindow(viewController);
            }
        }




        public MainController(IntPtr handle) : base(handle)
        {
        }

        partial void removeRowButton(NSObject sender)
        {
            try
            {
                var query = $"SELECT * FROM {selectedTableName}";

                using var connection = new MySqlConnection(builder.ConnectionString);
                connection.Open();

                var adapter = new MySqlDataAdapter(query, connection);
                var ds = new DataSet();
                adapter.Fill(ds);
                var table = ds.Tables[0];

                var builderr = new MySqlCommandBuilder(adapter);
                ds.Tables[0].Rows[selectedRowIndex].Delete();
                adapter.Update(ds.Tables[0]);
                ds.Tables[0].AcceptChanges();
                dataSource = new MyTableDataSource(table);
                tableView.DataSource = dataSource;
                tableView.ReloadData();

                editRowButton.Enabled = false;
                removeButton.Enabled = false;

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




        partial void reloadTableButton(NSObject sender)
        {
            reLoadTable();
        }

        partial void searchButton(NSObject sender)
        {
            DataTable filtrTable;
            if (searchConditionLable.StringValue == string.Empty)
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

                if (searchColumnComboBox.SelectedIndex == -1)
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
                    string filtrColumnName = searchColumnComboBox.SelectedValue.ToString();
                    string filtrValue = searchConditionLable.StringValue;
                    string type = searchTypeComboBox.SelectedValue.ToString();
                    selectedTableName = tableNames[(int)tableComboBox.SelectedIndex];
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
                            filtr = filtrColumnName + "= '" + filtrValue + "'";
                            break;
                        case "По вхождению":
                            filtr = filtrColumnName + " LIKE '%" + filtrValue + "%'";
                            break;

                        case "Начинается с":
                            filtr = filtrColumnName + " LIKE '" + filtrValue + "%'";
                            break;

                        case "Больше":
                            filtr = filtrColumnName + " > '" + filtrValue + "'";
                            break;

                        case "Больше равно":
                            filtr = filtrColumnName + " >= '" + filtrValue + "'";
                            break;

                        case "Меньше":
                            filtr = filtrColumnName + " < '" + filtrValue + "'";
                            break;

                        case "Меньше равно":
                            filtr = filtrColumnName + " <= '" + filtrValue + "'";
                            break;

                    }
                    try
                    {
                        DataRow[] HelpDataRows = table.Select(filtr);
                        filtrTable = table.Clone();
                        foreach (var row in HelpDataRows)
                        {
                            filtrTable.ImportRow(row);
                        }
                        tableView.DataSource = new MyTableDataSource(filtrTable);
                        tableView.ReloadData();
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

        partial void resetSearchButton(NSObject sender)
        {
            reLoadTable();
            searchConditionLable.StringValue = string.Empty;


        }

      
        private void getSelectedRow(object sender, EventArgs e)
        {
            try
            {
                selectedRowIndex = (int)tableView.SelectedRow;
                selectedRowId = (int)table.Rows[selectedRowIndex]["Id"];
                editRowButton.Enabled = true;
                removeButton.Enabled = true;
            }
            catch (Exception ex)
            {
                if (ex.Message == "There is no row at position -1.")
                {
                    editRowButton.Enabled = false;
                    removeButton.Enabled = false;
                }
            }

        }
        private void reLoadTable()
        {
            try
            {
                selectedTableName = tableNames[(int)tableComboBox.SelectedIndex];
                var query = $"SELECT * FROM {selectedTableName}";

                using var connection = new MySqlConnection(builder.ConnectionString);
                connection.Open();

                var adapter = new MySqlDataAdapter(query, connection);
                var ds = new DataSet();
                adapter.Fill(ds);

                table = ds.Tables[0];
                dataSource = new MyTableDataSource(table);
                tableView.DataSource = dataSource;
                tableView.ReloadData();

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
        private void makeColumnNameList(DataTable Helptable)
        {

            searchColumnComboBox.RemoveAll();

            foreach (DataColumn HelpColumn in Helptable.Columns)
            {
                searchColumnComboBox.Add(new NSString(HelpColumn.ToString()));
            }
            searchColumnComboBox.StringValue = string.Empty;
            searchColumnComboBox.ReloadData();
        }

        partial void selectSearchColumn(NSObject sender)
        {

            selectedColumnType = table.Columns[((int)searchColumnComboBox.SelectedIndex)].DataType.ToString();
            searchTypeComboBox.RemoveAll();
            searchTypeComboBox.StringValue = string.Empty;

            searchTypeComboBox.Add(new NSString("По равенству"));
            switch (selectedColumnType)
            {
                case "System.Int32":
                    searchTypeComboBox.Add(new NSString("Больше"));
                    searchTypeComboBox.Add(new NSString("Меньше"));
                    searchTypeComboBox.Add(new NSString("Больше равно"));
                    searchTypeComboBox.Add(new NSString("Меньше равно"));
                    break;


                case "System.String" or "System.DateTime":
                    searchTypeComboBox.Add(new NSString("По вхождению"));
                    searchTypeComboBox.Add(new NSString("Начинается с"));
                    break;
            }

            searchTypeComboBox.SelectItem(0);


        }
    }
}
