using System;
using AppKit;
using Foundation;
using System.Configuration;

using DotNetEnv;
using MySqlConnector;


namespace CorsovaiBD
{
	public partial class ViewController : NSViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad();

			// Do any additional setup after loading the view.
		}

		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
        partial void initDatabase(NSButton sender)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = ConfigurationManager.AppSettings["Server"],
                UserID = ConfigurationManager.AppSettings["UserID"],
                Password = ConfigurationManager.AppSettings["Password"],
                Database = ConfigurationManager.AppSettings["Database"]
            };

            Console.WriteLine(builder.Server);
            // open a connection asynchronously
            var connection = new MySqlConnection(builder.ConnectionString);

            try
            {
                Console.WriteLine("Connecting to MySQL...");
                connection.Open();
                //Perform database operations
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            string query = "SELECT * FROM Addresses";
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine(reader["City"].ToString());
            }
        }

    }
}
