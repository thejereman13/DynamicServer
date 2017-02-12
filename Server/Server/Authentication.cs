using LiteDB;
using Sodium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServer {
	public class Authentication {

		/**
		 * Authentication Levels:
		 * 0 - Standard User
		 * 1 - Basic usage of most modules
		 * 2 - Admin (Ability to adjust anything with modules)
		 * 3 - Owner (Access to everything, including the physical server)
		 **/
		public class User {
			public int Id { get; set; }
			public string UserName { get; set; }
			public string Password { get; set; }
			public int AuthLvl { get; set; }
			public User() {}
		}

		public static void addUser(string userName, string password, int authLevel) {
			var user = new User { UserName = userName, AuthLvl = authLevel, Password = hashPass(password) };
			using(var db = new LiteDatabase("filename=Users.db; journal=false")) {
				var col = db.GetCollection<User>("Users");
				if(!col.Exists(Query.EQ("UserName", userName))) {
					col.Insert(user);
				}
			}
		}
		public static bool removeUser(string userName) {
			using(var db = new LiteDatabase("filename=Users.db; journal=false")) {
				var col = db.GetCollection<User>("Users");
				return (col.Delete(Query.EQ("UserName", userName)) > 0) ? true : false;
			}
		}

		public static bool userExists(string userName) {
			using(var db = new LiteDatabase("filename=Users.db; journal=false")) {
				var col = db.GetCollection<User>("Users");
				return col.Exists(Query.EQ("UserName", userName));
			}
		}

		public static User getUser(string userName) {
			using(var db = new LiteDatabase("filename=Users.db; journal=false")) {
				var col = db.GetCollection<User>("Users");
				return col.FindOne(Query.EQ("UserName", userName));
			}
		}

		public static List<string> listUsers() {
			List<string> output = new List<string>();
			using(var db = new LiteDatabase("filename=Users.db; journal=false")) {
				var col = db.GetCollection<User>("Users");
				var ls = col.FindAll().ToList();
				foreach(User u in ls) {
					output.Add(u.UserName);
				}
			}
			return output;
		}

		public static bool authUser(string userName, string password) {
			using(var db = new LiteDatabase("filename=Users.db; journal=false")) {
				var col = db.GetCollection<User>("Users");
				if(!col.Exists(Query.EQ("UserName", userName)))
					return false;
				var u = col.FindOne(Query.EQ("UserName", userName));
				if (u != null)
					return checkPass(u.Password, password);
				return false;
			}
		}

		public static void updateUser(User u) {
			using(var db = new LiteDatabase("filename=Users.db; journal=false")) {
				var col = db.GetCollection<User>("Users");
				col.Update(u);
			}
		}

		public static bool login(string client, string userName, string password) {
			bool auth = authUser(userName, password);
			if(auth) {
				ClientManagement.clients[client].authLevel = getUser(userName).AuthLvl;
				ClientManagement.clients[client].userName = userName;
			}

			return auth;
		}

		public static bool setPass(string userName, string password) {
			using(var db = new LiteDatabase("filename=Users.db; journal=false")) {
				var col = db.GetCollection<User>("Users");
				if(col.Exists(Query.EQ("UserName", userName))) {
					User u = col.FindOne(Query.EQ("UserName", userName));
					if(u != null) {
						u.Password = hashPass(password);
						col.Update(u);
						return true;
					}
				}
			}
			return false;
		}

		public static bool setAuth(string userName, int auth) {
			using(var db = new LiteDatabase("filename=Users.db; journal=false")) {
				var col = db.GetCollection<User>("Users");
				if(col.Exists(Query.EQ("UserName", userName))) {
					User u = col.FindOne(Query.EQ("UserName", userName));
					if(u != null) {
						u.AuthLvl = auth;
						col.Update(u);
						return true;
					}
				}
			}
			return false;
		}

		private static string hashPass(string password) {
			//return PasswordHash.ScryptHashString(password, PasswordHash.Strength.Medium);
			return PasswordStorage.CreateHash(password);
		}
		private static bool checkPass(string hash, string pass) {
			//return PasswordHash.ScryptHashStringVerify(hash, pass);
			return PasswordStorage.VerifyPassword(pass, hash);
		}

	}

}
