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

namespace CorsovaiBD
{
    public partial class AddRowForm : NSViewController
    {
        private readonly MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = ConfigurationManager.AppSettings["Server"],
            UserID = ConfigurationManager.AppSettings["UserID"],
            Password = ConfigurationManager.AppSettings["Password"],
            Database = ConfigurationManager.AppSettings["Database"]
        };
        public AddRowForm(IntPtr handle) : base(handle) {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Создаем и настраиваем элементы пользовательского интерфейса (метки, поля ввода, кнопку "Добавить")
            var xPos = 20f;
            var yPos = 300f;
            var labelWidth = 100f;
            var inputWidth = 150f;
            var inputHeight = 22f;
            var margin = 30f;

            using (var connection = new MySqlConnection(builder.ConnectionString))
            {
                connection.Open();
                var query = $"SELECT * FROM {ViewController.SelectedTableName}";
                var adapter = new MySqlDataAdapter(query, connection);
                var ds = new DataSet();
                adapter.Fill(ds);

                var table = ds.Tables[0];
                var columns = table.Columns;

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

                    var input = new NSTextField(new CGRect(xPos + labelWidth + margin, yPos, inputWidth, inputHeight))
                    {
                        Identifier = new NSString(columnName)
                    };
                    View.AddSubview(input);

                    yPos -= (inputHeight + margin);
                }
            }

            var addButton = new NSButton(new CGRect(xPos, yPos - margin, 100f, inputHeight))
            {
                Title = "Добавить",
                BezelStyle = NSBezelStyle.Rounded
            };
            addButton.Activated += AddRow;
            View.AddSubview(addButton);
        }

        private void AddRow(object sender, EventArgs e)
        {
            try
            {
                using (var connection = new MySqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    var query = $"INSERT INTO {ViewController.SelectedTableName} (";
                    var values = "VALUES (";

                    foreach (var subview in View.Subviews)
                    {
                        if (subview is NSTextField input && !string.IsNullOrEmpty(input.Identifier))
                        {
                            var columnName = input.Identifier.ToString();
                            var columnValue = input.StringValue;

                            query += $"{columnName}, ";
                            values += $"'{columnValue}', ";
                        }
                    }

                    //Remove trailing comma and space
                    query = query.Remove(query.Length - 2);
                    values = values.Remove(values.Length - 2);

                    query += ") " + values + ")";

                    var command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();

                    // Закрываем форму добавления новой строки
                    DismissViewController(this);
                }
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
    }
}