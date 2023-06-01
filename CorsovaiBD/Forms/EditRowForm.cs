using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using AppKit;
using CoreGraphics;
using CorsovaiBD.Models;
using Foundation;
using MySqlConnector;

namespace CorsovaiBD
{
    public partial class EditRowForm : NSViewController
	{
		public EditRowForm (IntPtr handle) : base (handle)
		{
           
		}
        private int columnCount;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
    
            //var d = a.getSelectedRow;
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

                columns.Remove("id");
                columnCount = columns.Count;
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


                var editButton = new NSButton(new CGRect(450, 500f, 100f, inputHeight))
                {
                    Title = "Изменить",
                    BezelStyle = NSBezelStyle.Rounded
                };
                editButton.Activated += EditRow;
                View.AddSubview(editButton);

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


        private void EditRow(object sender, EventArgs e)
        {
            try
            {
                using (var connection = new MySqlConnection(ViewController.builder.ConnectionString))
                {
                    connection.Open();

                    var adapter = new MySqlDataAdapter($"SELECT * FROM {ViewController.SelectedTableName}", connection);
                    var ds = new DataSet();
                    adapter.Fill(ds);
                    var dataTable = ds.Tables[0];
                    // Fill the columns of the DataTable based on NSTextField'

                    // Create a new row and populate it with values from NSTextField's
                    int countEmptyRow = 0;
                    foreach (var subview in View.Subviews)
                    {
                        if (subview is NSTextField input && !string.IsNullOrEmpty(input.Identifier))
                        {
                            var columnName = input.Identifier.ToString();
                            var columnValue = input.StringValue;
                            if (columnValue == "")
                            {
                                countEmptyRow += 1;
                            }
                            dataTable.Rows[ViewController.selectedRowIndex][columnName] = input.StringValue.Split(" ")[0];
                        }
                    }

                    if (countEmptyRow == columnCount)
                    {
                        var alert = new NSAlert
                        {
                            AlertStyle = NSAlertStyle.Critical,
                            InformativeText = "Пустая строка",
                            MessageText = "Ошибка"
                        };
                        alert.RunModal();
                    }
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
                // Display an error window
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

