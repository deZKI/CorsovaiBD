using System;
namespace CorsovaiBD.Models
{
	public class User
	{
		private string username;
		private bool isAdmin;
		public User(string username, bool isAdmin)
		{
			this.username = username;
			this.isAdmin = isAdmin;
		}
	}
}

