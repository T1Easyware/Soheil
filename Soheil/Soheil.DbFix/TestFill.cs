using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Data.SqlClient;

namespace Soheil.DbFix
{
	static class TestFill
	{
		public static void Run(string name)
		{
			string sqlConnectionString = @"Integrated Security=True;MultipleActiveResultSets=True;Initial Catalog=SoheilDb;Data Source=.";
			SqlConnection conn = new SqlConnection(sqlConnectionString);
			conn.Open();
			FileInfo file = new FileInfo("..\\..\\..\\Soheil.Dal\\" + name + ".sql");
			var stream = file.OpenText();
			string script = stream.ReadToEnd();
			
			SqlCommand cmd = new SqlCommand(script, conn);
			
			cmd.ExecuteNonQuery();
			stream.Close();
		}
		public static void RunAll()
		{
			/*Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("1. Creating Database from script...");
			Run("SoheilEdm.edmx");

			Console.WriteLine("2. Creating basic data...");
			Run("SqlScripts\\MultiInsertAccessRules.edmx");

			Console.WriteLine("3. Creating advanced test data...");
			Run("SqlScripts\\InitDatabaseforTest.edmx");*/
		}
	}
}
