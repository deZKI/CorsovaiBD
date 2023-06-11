using System;
namespace CorsovaiBD.Models
{
	public class User
	{
		public string username;
		public bool isAdmin;
		public User(string username, bool isAdmin)
		{
			this.username = username;
			this.isAdmin = isAdmin;
		}
		public User(User copy) {
			this.username = copy.username;
			this.isAdmin = copy.isAdmin;
		}

	}
}

