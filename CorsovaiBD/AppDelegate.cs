using AppKit;
using Foundation;
using System.Configuration;

using MySqlConnector;

using CorsovaiBD.Models;

namespace CorsovaiBD
{
    [Register ("AppDelegate")]
	public class AppDelegate : NSApplicationDelegate
	{
        public string connectionString { get; private set; }
        public User currentUser;
        public override void DidFinishLaunching(NSNotification notification)
        {
            // Create the MySQL connection string builder
             MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
             {
                 Server = ConfigurationManager.AppSettings["Server"],
                 UserID = ConfigurationManager.AppSettings["UserID"],
                 Password = ConfigurationManager.AppSettings["Password"],
                 Database = ConfigurationManager.AppSettings["Database"]
             };
            // Create the MySQL connection using the connection string
            connectionString = builder.ConnectionString;

        }

        //Close app when there is no opened window
        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }
        public override void WillTerminate(NSNotification notification)
        {
        }
    }
}

