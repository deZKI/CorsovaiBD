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
        public AddRowForm(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
          
            // Создаем и настраиваем элементы пользовательского интерфейса (метки, поля ввода, кнопку "Добавить")
            var xPos = 20f;
            var yPos = 500f;
            var labelWidth = 100f;
            var inputWidth = 150f;
            var inputHeight = 22f;
            var margin = 30f;

            using var connection = new MySqlConnection(builder.ConnectionString);
            connection.Open();
            var query = $"SELECT * FROM {ViewController.SelectedTableName}";
            var adapter = new MySqlDataAdapter(query, connection);
            var ds = new DataSet();
            adapter.Fill(ds);

            var table = ds.Tables[0];
            var columns = table.Columns;

            // Получаем список внешних ключей для выбранной таблицы
            var foreignKeys = GetForeignKeys(connection, ViewController.SelectedTableName);

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

                if (foreignKeys.Any(fk => fk.ColumnName == columnName))
                {
                    // Создаем комбобокс для внешнего ключа и заполняем его значениями из связанной таблицы
                    var comboBox = new NSComboBox(new CGRect(xPos + labelWidth + margin, yPos, inputWidth, inputHeight));
                    comboBox.Identifier = new NSString(columnName);
                    comboBox.Editable = true;

                    var foreignKey = foreignKeys.First(fk => fk.ColumnName == columnName);
                    var referencedTable = foreignKey.ReferencedTableName;
                    var referencedColumn = foreignKey.ReferencedColumnName;
                    var referencedQuery = $"SELECT {referencedColumn} FROM {referencedTable}";
                    var referencedAdapter = new MySqlDataAdapter(referencedQuery, connection);
                    var referencedDs = new DataSet();
                    referencedAdapter.Fill(referencedDs);
                    var referencedTableData = referencedDs.Tables[0];
                    foreach (DataRow row in referencedTableData.Rows)
                    {
                        comboBox.Add(new NSString(row[referencedColumn].ToString()));
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


            var addButton = new NSButton(new CGRect(350, 500f, 100f, inputHeight))
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
