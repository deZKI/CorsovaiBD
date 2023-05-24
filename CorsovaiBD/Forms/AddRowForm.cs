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

                using var connection = new MySqlConnection(ViewController.builder.ConnectionString);
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
                        var comboBox = new NSComboBox(new CGRect(xPos + labelWidth + margin, yPos, 300, inputHeight));
                        comboBox.Identifier = new NSString(columnName);
                        comboBox.Editable = true;

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


        private void AddRow(object sender, EventArgs e)
        {
            try
            {
                using (var connection = new MySqlConnection(ViewController.builder.ConnectionString))
                {
                    connection.Open();

                    var dataTable = new DataTable(ViewController.SelectedTableName);

                    // Заполняем столбцы DataTable на основе NSTextField'ов
                    foreach (var subview in View.Subviews)
                    {
                        if (subview is NSTextField input && !string.IsNullOrEmpty(input.Identifier))
                        {
                            var columnName = input.Identifier.ToString();
                            dataTable.Columns.Add(columnName);
                        }
                    }

                    // Создаем новую строку и заполняем ее значениями из NSTextField'ов
                    var row = dataTable.NewRow();
                    foreach (var subview in View.Subviews)
                    {
                        if (subview is NSTextField input && !string.IsNullOrEmpty(input.Identifier))
                        {
                            var columnName = input.Identifier.ToString();
                            Console.WriteLine(input.StringValue.Split(" ")[0]);
                            row[columnName] = input.StringValue.Split(" ")[0];
                        }
                    }

                    // Добавляем новую строку в таблицу
                    dataTable.Rows.Add(row);

                    var adapter = new MySqlDataAdapter($"SELECT * FROM {ViewController.SelectedTableName}", connection);

                    // Создаем объект MySqlCommandBuilder на основе адаптера, чтобы автоматически генерировать InsertCommand
                    var builder = new MySqlCommandBuilder(adapter);



                    // Добавляем новую строку в таблицу
                    adapter.Update(dataTable);

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
