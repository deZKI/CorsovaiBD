using System;
using Foundation;
using AppKit;
using CorsovaiBD.Models;
using CoreGraphics;
using MySqlConnector;
using System.Collections.Generic;
using System.Reflection;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.IO;

namespace CorsovaiBD
{
    public partial class AddRowForm : NSViewController
    {
        public AddRowForm(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            try
            {

                // Создаем и настраиваем элементы пользовательского интерфейса (метки, поля ввода, кнопку "Добавить")
                var xPos = 20f;
                var yPos = 600f;
                var labelWidth = 100f;
                var inputWidth = 250f;
                var inputHeight = 22f;
                var margin = 30f;

                using var connection = new MySqlConnection(MainController.builder.ConnectionString);
                connection.Open();
                var query = $"SELECT * FROM {MainController.selectedTableName}";
                var adapter = new MySqlDataAdapter(query, connection);
                var ds = new DataSet();
                adapter.Fill(ds);

                var table = ds.Tables[0];
                var columns = table.Columns;

                // Получаем список внешних ключей для выбранной таблицы
                var foreignKeys = GetForeignKeys(connection, MainController.selectedTableName);

                foreach (DataColumn column in columns)
                {
                    var columnName = (string)column.ColumnName;
                  
                    var label = new NSTextField(new CGRect(xPos, yPos, labelWidth, inputHeight))
                    {
                        StringValue = columnName,
                        Editable = false,
                        Bordered = false,
                        Selectable = false,
                        DrawsBackground = false
                    };
                    View.AddSubview(label);

                    if (columnName == "Photo") // Check if the column is the photo column
                    {
                        // Create a button for selecting the photo
                        var selectPhotoButton = new NSButton(new CGRect(xPos + labelWidth + margin, yPos, 100, inputHeight))
                        {
                            Title = "Select Photo",
                            BezelStyle = NSBezelStyle.Rounded,
                            Target = this, // Set the target to the current instance
                            Action = new ObjCRuntime.Selector("SelectPhoto:"),
                            Tag = 999 // Set a tag to identify the button later
                        };

                        View.AddSubview(selectPhotoButton);

                        // Create an NSImageView for displaying the photo
                        var imageView = new NSImageView(new CGRect(xPos + labelWidth + margin + 120, yPos, inputWidth - 120, inputHeight))
                        {
                            Image = NSImage.ImageNamed(NSImageName.UserGuest), // Placeholder image
                            Editable = true,
                        };

                        // Set a tag on the image view to identify it later
                        imageView.Tag = 998;

                        View.AddSubview(imageView);
                    }

                    else if (foreignKeys.Any(fk => fk.ColumnName == columnName))
                    {
                        // Создаем комбобокс для внешнего ключа и заполняем его значениями из связанной таблицы
                        var comboBox = new NSComboBox(new CGRect(xPos + labelWidth + margin, yPos, 300, inputHeight));
                        comboBox.Identifier = new NSString(columnName);
                        comboBox.Editable = false;

                        var foreignKey = foreignKeys.First(fk => fk.ColumnName == columnName);
                        var referencedTable = foreignKey.ReferencedTableName;
                        var referencedColumn = foreignKey.ReferencedColumnName;
                        var referencedQuery = $"SELECT * FROM {referencedTable}";
                        var referencedAdapter = new MySqlDataAdapter(referencedQuery, connection);
                        var referencedDs = new DataSet();
                        referencedAdapter.Fill(referencedDs);
                        var referencedTableData = referencedDs.Tables[0];
                        foreach (DataRow row in referencedTableData.Rows)
                        {
                            string[] values = row.ItemArray.Select(x => x.ToString()).ToArray();
                            comboBox.Add(new NSString(string.Join(" ", values)));
                        }

                        View.AddSubview(comboBox);
                    }
                    else
                    {
                        // Создаем текстовое поле для обычного столбца
                        var input = new NSTextField(new CGRect(xPos + labelWidth + margin, yPos, inputWidth, inputHeight))
                        {
                            Identifier = new NSString(columnName)
                        };

                        View.AddSubview(input);
                    }

                    yPos -= (inputHeight + margin);
                }


                var addButton = new NSButton(new CGRect(450, 500f, 100f, inputHeight))
                {
                    Title = "Добавить",
                    BezelStyle = NSBezelStyle.Rounded
                };
                addButton.Activated += AddRow;
                View.AddSubview(addButton);
                
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

            }
        }

        [Action("SelectPhoto:")]
        private void SelectPhoto(NSObject sender)
        {
            var button = sender as NSButton;
            if (button != null && button.Tag == 999)
            {
                var openPanel = NSOpenPanel.OpenPanel;
                openPanel.AllowedFileTypes = new string[] { "public.image" }; // Limit to image files
                openPanel.CanChooseFiles = true;
                openPanel.CanChooseDirectories = false;
                openPanel.AllowsMultipleSelection = false;

                openPanel.BeginSheet(View.Window, result =>
                {
                    if (result == 1)
                    {
                        var selectedUrl = openPanel.Urls.FirstOrDefault();
                        if (selectedUrl != null)
                        {
                            // Get the path of the selected image file
                            var imagePath = selectedUrl.Path;

                            // Find the corresponding image view based on the tag
                            var imageView = View.ViewWithTag(998) as NSImageView;
                            if (imageView != null)
                            {
                                // Update the image view with the selected photo
                                var image = new NSImage(imagePath);
                                imageView.Image = image;
                            }
                        }
                    }
                });
            }
        }


        private void SavePhotoToDatabase(byte[] photoData)
        {
            try
            {
                using var connection = new MySqlConnection(MainController.builder.ConnectionString);
                connection.Open();

                // Prepare the SQL statement to insert the photo data into the database
                var query = $"INSERT INTO {MainController.selectedTableName} (Photo) VALUES (@photoData)";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@photoData", photoData);

                // Execute the SQL statement
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Handle the exception or display an error message
                Console.WriteLine($"Error saving photo to database: {ex.Message}");
            }
        }


        private void AddRow(object sender, EventArgs e)
        {
            try
            {
                using (var connection = new MySqlConnection(MainController.builder.ConnectionString))
                {
                    connection.Open();

                    var dataTable = new DataTable(MainController.selectedTableName);

                    // Add columns to the DataTable based on NSTextField inputs
                    foreach (var subview in View.Subviews)
                    {
                        if (subview is NSTextField input && !string.IsNullOrEmpty(input.Identifier))
                        {
                            var columnName = input.Identifier.ToString();
                            dataTable.Columns.Add(columnName);
                        }
                    }

                    // Create a new row and populate it with values from NSTextField inputs
                    var row = dataTable.NewRow();
                    foreach (var subview in View.Subviews)
                    {
                        if (subview is NSTextField input && !string.IsNullOrEmpty(input.Identifier))
                        {
                            var columnName = input.Identifier.ToString();
                            row[columnName] = input.StringValue.Split(" ")[0];
                        }
                    }

                    // Add the new row to the DataTable
                    dataTable.Rows.Add(row);

                    // Handle the photo input separately
                    var photoImageView = View.Subviews.OfType<NSImageView>().FirstOrDefault(iv => iv.Identifier == "Photo");
                    if (photoImageView != null && photoImageView.Image != null)
                    {
                        var tiffData = photoImageView.Image.AsTiff();

                        // Convert the TIFF data to a byte array
                        using (var tiffStream = new MemoryStream())
                        {
                            tiffData.AsStream().CopyTo(tiffStream);
                            var photoData = tiffStream.ToArray();

                            row["Photo"] = photoData;
                        }
                    }

                    var adapter = new MySqlDataAdapter($"SELECT * FROM {MainController.selectedTableName}", connection);

                    // Create a MySqlCommandBuilder based on the adapter to automatically generate the InsertCommand
                    var builder = new MySqlCommandBuilder(adapter);

                    // Update the database with the new row
                    adapter.Update(dataTable);

                    // Close the add row form
                    DismissViewController(this);
                }
            }
            catch (Exception ex)
            {
                // Display an error message
                var alert = new NSAlert
                {
                    AlertStyle = NSAlertStyle.Critical,
                    InformativeText = ex.Message,
                    MessageText = "Ошибка"
                };
                alert.RunModal();
            }
        }





        public static List<ForeignKeyColumn> GetForeignKeys(MySqlConnection connection, string tableName)
        {
            var foreignKeys = new List<ForeignKeyColumn>();

            var query = $@"
        SELECT 
            COLUMN_NAME, 
            REFERENCED_TABLE_NAME, 
            REFERENCED_COLUMN_NAME 
        FROM information_schema.KEY_COLUMN_USAGE 
        WHERE 
            TABLE_SCHEMA = '{connection.Database}' AND 
            TABLE_NAME = '{tableName}' AND 
            REFERENCED_TABLE_NAME IS NOT NULL;
    ";
            var command = new MySqlCommand(query, connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var columnName = reader.GetString("COLUMN_NAME");
                var referencedTableName = reader.GetString("REFERENCED_TABLE_NAME");
                var referencedColumnName = reader.GetString("REFERENCED_COLUMN_NAME");

                var foreignKeyColumn = new ForeignKeyColumn(columnName, referencedTableName, referencedColumnName);
                foreignKeys.Add(foreignKeyColumn);
            }

            return foreignKeys;
        }

    }
}
