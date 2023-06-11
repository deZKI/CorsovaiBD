// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using AppKit;
using MySqlConnector;
using System.Data;
using System.Collections.Generic;
using System.IO;

using NPOI.XWPF.UserModel;
using iTextSharp.text.pdf.parser;
using NPOI.OpenXml4Net.OPC;

namespace CorsovaiBD
{
	public partial class HomeReportForm : NSViewController
	{
        public Dictionary<string, string> HomeOwnership;

        public HomeReportForm (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            try
            {
                using var connection = new MySqlConnection(MainController.builder.ConnectionString);
                connection.Open();

                var query = $@"
    SELECT ho.ID, a.City, a.Street, a.HouseNumber, d.Name, a.BlockNumber, ho.PhotoPath, ho.Notification
    FROM Home_Ownership ho
    JOIN Addresses a ON ho.Address_Id = a.ID
    JOIN District d ON a.DistrictId = d.Id
    WHERE ho.ID = {MainController.selectedRowId}";

                var adapter = new MySqlDataAdapter(query, connection);
                var dataSet = new DataSet();
                adapter.Fill(dataSet);

                var table = dataSet.Tables[0];
                var values = table.Rows[0].ItemArray;

                idField.Cell.Title = values[0].ToString();
                cityField.Cell.Title = values[1].ToString();
                streetField.Cell.Title = values[2].ToString();
                houseNumberField.Cell.Title = values[3].ToString();
                districtField.Cell.Title = values[4].ToString();
                blockField.Cell.Title = values[5].ToString();
                pathField.Cell.Title = values[6].ToString();
                notificationField.Cell.Title = values[7].ToString();


            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }


        }
        partial void createReport(NSObject sender)
        {
            HomeOwnership = new Dictionary<string, string>
{
    {"Id", idField.Cell.Title},
    {"City", cityField.Cell.Title},
    {"Street", streetField.Cell.Title},
    {"HouseNumber", houseNumberField.Cell.Title},
    {"District", districtField.Cell.Title},
    {"BlockNumber", blockField.Cell.Title},
    {"PhotoPath", pathField.Cell.Title},
    {"Notification", notificationField.Cell.Title}
};

            var openFileDialog = new NSOpenPanel();
            openFileDialog.CanChooseFiles = true;
            openFileDialog.CanChooseDirectories = false;
            openFileDialog.AllowedFileTypes = new string[] { "docx" };

            if (openFileDialog.RunModal() == (int)NSModalResponse.OK)
            {
                var templateFilePath = openFileDialog.Url.Path;

                var saveFileDialog = new NSSavePanel();
                saveFileDialog.CanCreateDirectories = true;
                saveFileDialog.AllowedFileTypes = new string[] { "docx" };

                if (saveFileDialog.RunModal() == (int)NSModalResponse.OK)
                {
                    var saveFilePath = saveFileDialog.Url.Path;
                    var wordDocument = new XWPFDocument(OPCPackage.Open(templateFilePath));

                    ReplaceWordStab("Id", HomeOwnership["Id"], wordDocument);
                    ReplaceWordStab("City", HomeOwnership["City"], wordDocument);
                    ReplaceWordStab("Street", HomeOwnership["Street"], wordDocument);
                    ReplaceWordStab("HouseNumber", HomeOwnership["HouseNumber"], wordDocument);
                    ReplaceWordStab("District", HomeOwnership["District"], wordDocument);
                    ReplaceWordStab("BlockNumber", HomeOwnership["BlockNumber"], wordDocument);
                    ReplaceWordStab("PhotoPath", HomeOwnership["PhotoPath"], wordDocument);
                    ReplaceWordStab("Notification", HomeOwnership["Notification"], wordDocument);

                    wordDocument.Write(new FileStream(saveFilePath, FileMode.Create));


                }
            }
        }

        public static void ReplaceWordStab(string stabToReplace, string text, XWPFDocument wordDocument)
        {
            foreach (var paragraph in wordDocument.Paragraphs)
            {
                foreach (var run in paragraph.Runs)
                {
                    if (run.Text.Contains(stabToReplace))
                    {
                        run.SetText(run.Text.Replace(stabToReplace, text));
                    }
                    
                }
            }
        }

    }
}