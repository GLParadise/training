using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers
{
	public class User
	{
		public string email
		{
			get;
			set;
		}

		public string firstName
		{
			get;
			set;
		}

		public Guid id
		{
			get;
			set;
		}

		public string lastName
		{
			get;
			set;
		}

		public StinvUser stinvUser
		{
			get;
			set;
		}

		public string userName
		{
			get;
			set;
		}

		public User()
		{
		}
	}
}