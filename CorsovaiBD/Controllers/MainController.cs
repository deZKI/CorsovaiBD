using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using AppKit;
using CoreGraphics;
using CorsovaiBD.Models;
using Foundation;
using MySqlConnector;
using NPOI.XWPF.UserModel;

using iTextSharp.text;
using iTextSharp.text.pdf;

using OfficeOpenXml;
using OfficeOpenXml.Table;


using System.Drawing;
using System.Drawing.Printing;

namespace CorsovaiBD
{
    public partial class MainController : NSViewController
	{
        private readonly List<string> tableNames = new List<string>();
        public User currentUser = new User("Anno", false);
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
            currentUser = ((AppDelegate)NSApplication.SharedApplication.Delegate).currentUser;
            loadTableNames();
            

            tableComboBox.UsesDataSource = true;
            tableComboBox.DataSource = new ComboBoxDataSource(tableNames, tableComboBox);
            tableComboBox.Activated += showSelectedTable;

            editRowButton.Enabled = false;
            addRowButton.Enabled = false;
            removeButton.Enabled = false;

            excelButton.Enabled = false;
            wordButton.Enabled = false;  
            printButton.Enabled = false;

            reportBuildingButton.Enabled = false;
            reportHomeButton.Enabled = false;


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
            if (currentUser.isAdmin != true)
            {
                tableNames.Remove("Users");

                addRowButton.Hidden = true;
                editRowButton.Hidden = true;
                removeButton.Hidden = true;
            }
        }
        private void showSelectedTable(object sender, EventArgs e)
        {
            try
            {
                //bacause user should choose row
                editRowButton.Enabled = false;
                removeButton.Enabled = false;

                reportHomeButton.Enabled = false;
                reportBuildingButton.Enabled = false;

                selectedTableName = tableNames[(int)tableComboBox.SelectedIndex];

                addRowButton.Enabled = true;
                excelButton.Enabled = true;

                
                wordButton.Enabled = true;
                printButton.Enabled = true;

                

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
                var query = $"DELETE FROM {selectedTableName} WHERE Id = @id"; // Replace "Id" with the actual column name representing the ID in your table

                using var connection = new MySqlConnection(builder.ConnectionString);
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", selectedRowId);

                connection.Open();
                command.ExecuteNonQuery();

                table.Rows[selectedRowIndex].Delete();
                table.AcceptChanges();

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
            reloadTable();
        }

        partial void searchButton(NSObject sender)
        {

            if (searchColumnComboBox.SelectedIndex == -1)
            {
                showAlert("Не выбрано имя столбца для поиска", "Ошибка");
                return;
            }

            string filtrColumnName = searchColumnComboBox.SelectedValue.ToString();
            string filtrValue = searchConditionLable.StringValue;
            string type = searchTypeComboBox.SelectedValue.ToString();

            if (searchConditionLable.StringValue == string.Empty &&
                type != "Диапазон")
            {
                showAlert("Не введено условие для поиска", "Ошибка");
                return;
            }

            selectedTableName = tableNames[(int)tableComboBox.SelectedIndex];
            string filtr = "";

            if (selectedColumnType == "System.DateTime")
            {
                DateTime result_date;
                bool success_data = DateTime.TryParse(filtrValue, out result_date);
                if (!success_data)
                {
                    showAlert("Ввели не дату, а колонку с датой", "Ошибка");
                    return;
                }
            }

            switch (type)
            {
                case "По равенству":
                    filtr = $"{filtrColumnName} = '{filtrValue}'";
                    break;
                case "По вхождению":
                    filtr = $"{filtrColumnName} LIKE '%{filtrValue}%'";
                    break;
                case "Начинается с":
                    filtr = $"{filtrColumnName} LIKE '{filtrValue}%'";
                    break;
                case "Больше":
                    filtr = $"{filtrColumnName} > '{filtrValue}'";
                    break;
                case "Больше равно":
                    filtr = $"{filtrColumnName} >= '{filtrValue}'";
                    break;
                case "Меньше":
                    filtr = $"{filtrColumnName} < '{filtrValue}'";
                    break;
                case "Меньше равно":
                    filtr = $"{filtrColumnName} <= '{filtrValue}'";
                    break;
                case "Диапазон":
                    filtr = $"{filtrColumnName} >= '{diapFrom.StringValue}' and  {filtrColumnName} <= '{diapTo.StringValue}'";
                    break;
            }

            try
            {
                DataRow[] helpDataRows = table.Select(filtr);
                DataTable filteredTable = table.Clone();
                foreach (var row in helpDataRows)
                {
                    filteredTable.ImportRow(row);
                }
                table = filteredTable;
                tableView.DataSource = new MyTableDataSource(filteredTable);
                tableView.ReloadData();
            }
            catch (Exception ex)
            {
                showAlert(ex.Message, "Ошибка");
            }
        }

        private void showAlert(string message, string title)
        {
            var alert = new NSAlert
            {
                AlertStyle = NSAlertStyle.Critical,
                InformativeText = message,
                MessageText = title
            };
            alert.RunModal();
        }


        partial void toExcelButton(NSObject sender)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Create a new Excel package
            using (var excelPackage = new ExcelPackage())
            {
                // Create a worksheet in the Excel package
                var worksheet = excelPackage.Workbook.Worksheets.Add("Table Data");

                // Get the selected table data from the database
                // Write the column headers to the worksheet
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = table.Columns[i].ColumnName;
                }

                // Write the data rows to the worksheet
                for (int row = 0; row < table.Rows.Count; row++)
                {
                    for (int col = 0; col < table.Columns.Count; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = table.Rows[row][col];
                    }
                }

                // Check if the table has primary keys
                if (table.PrimaryKey.Length > 0)
                {
                    // Get the primary key column names
                    var primaryKeyColumns = table.PrimaryKey.Select(pk => pk.ColumnName).ToArray();

                    // Write the primary key column headers to the worksheet
                    for (int i = 0; i < primaryKeyColumns.Length; i++)
                    {
                        worksheet.Cells[1, i + table.Columns.Count + 1].Value = primaryKeyColumns[i];
                    }

                    // Write the primary key values to the worksheet
                    for (int row = 0; row < table.Rows.Count; row++)
                    {
                        for (int col = 0; col < primaryKeyColumns.Length; col++)
                        {
                            worksheet.Cells[row + 2, col + table.Columns.Count + 1].Value = table.Rows[row][primaryKeyColumns[col]];
                        }
                    }
                }

                // Create an Excel table from the data
                var tableRange = worksheet.Cells[1, 1, table.Rows.Count + 1, table.Columns.Count];
                var excelTable = worksheet.Tables.Add(tableRange, "Table");

                // Set the table style
                excelTable.TableStyle = OfficeOpenXml.Table.TableStyles.Light1;

                // Create a save panel
                var savePanel = new NSSavePanel
                {
                    Title = "Save Excel File",
                    AllowedFileTypes = new[] { "xlsx" },
                    NameFieldStringValue = $"{selectedTableName}.xlsx"
                };

                // Display the save panel
                if (savePanel.RunModal() == 1)
                {
                    var filePath = savePanel.Url.Path;

                    // Save the Excel package to the selected file
                    var excelData = excelPackage.GetAsByteArray();
                    var nsData = NSData.FromArray(excelData);
                    nsData.Save(filePath, true);
                }
            }

        }

        partial void toWordButton(NSObject sender)
        {
            var doc = new XWPFDocument();

                // Create a new table in the document
                var tableWord = doc.CreateTable(table.Rows.Count + 1, table.Columns.Count);

                // Write the column headers to the table
                for (int col = 0; col < table.Columns.Count; col++)
                {
                    var cell = tableWord.GetRow(0).GetCell(col);
                    var paragraph = cell.Paragraphs[0];
                    var run = paragraph.CreateRun();
                    run.SetText(table.Columns[col].ColumnName);
                }

                // Write the data rows to the table
                for (int row = 0; row < table.Rows.Count; row++)
                {
                    for (int col = 0; col < table.Columns.Count; col++)
                    {
                        var cell = tableWord.GetRow(row + 1).GetCell(col);
                        var paragraph = cell.Paragraphs[0];
                        var run = paragraph.CreateRun();
                        run.SetText(table.Rows[row][col].ToString());
                    }
                }

                // Add the table to the document

                // Create a save panel
                var savePanel = new NSSavePanel
                {
                    Title = "Save Word Document",
                    AllowedFileTypes = new[] { "docx" },
                    NameFieldStringValue = $"{selectedTableName}.docx"
                };

                // Display the save panel
                if (savePanel.RunModal() == 1)
                {
                    var filePath = savePanel.Url.Path;

                    // Save the Word document
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        doc.Write(fileStream);
                        doc.Close();
                        
                    }
                }
        }


        partial void toPrintButton(NSObject sender)
        {
            // Create a print info object
            var printInfo = NSPrintInfo.SharedPrintInfo;

            // Configure the print settings
            printInfo.Orientation = NSPrintingOrientation.Landscape;
            printInfo.HorizontalPagination = NSPrintingPaginationMode.Auto;
            printInfo.VerticalPagination = NSPrintingPaginationMode.Auto;
            printInfo.LeftMargin = 36;
            printInfo.RightMargin = 36;
            printInfo.TopMargin = 36;
            printInfo.BottomMargin = 36;

            // Create a text container for the document
            var textContainer = new NSTextContainer();
            textContainer.LineFragmentPadding = 0;

            var printOperation = NSPrintOperation.FromView(tableView, printInfo);

            // Set the completion handler for the print operation
            printOperation.RunOperationModal(this.View.Window, tableView, new ObjCRuntime.Selector("printOperationDidRun:success:contextInfo:"), IntPtr.Zero);
        }

        [Export("printOperationDidRun:success:contextInfo:")]
        private void PrintOperationDidRun(NSPrintOperation printOperation, bool success, IntPtr contextInfo)
        {
            // Handle the completion of the print operation
            if (success)
            {
                Console.WriteLine("Print operation completed successfully");
            }
            else
            {
                Console.WriteLine("Print operation failed");
            }
        }


        partial void resetSearchButton(NSObject sender)
        {
            reloadTable();
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

                if (selectedTableName == "Home_Ownership"){
                    reportHomeButton.Enabled = true;
                }
                else if (selectedTableName == "Buildings")
                {
                    reportBuildingButton.Enabled = true;
                }

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

        public void reloadTable()
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
                if (ex.Message != "Index was out of range. Must be non-negative and less than the size of the collection.")
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
            try
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
                        searchTypeComboBox.Add(new NSString("Диапазон"));
                        break;


                    case "System.String" or "System.DateTime":
                        searchTypeComboBox.Add(new NSString("По вхождению"));
                        searchTypeComboBox.Add(new NSString("Начинается с"));
                        break;
                }

                searchTypeComboBox.SelectItem(0);
            }
            catch (Exception ex)
            {
                if (ex.Message != "Object reference not set to an instance of an object")
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
}
