﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Tablet
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		static string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Soheil");

		protected override void OnStartup(StartupEventArgs e)
		{
			//Exceptions.log
			if (Environment.UserName != "Bizz")
			{
				Application.Current.DispatcherUnhandledException +=
					new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
			}
			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);

			//debug mode
			if (e.Args.Length > 0)
				Soheil.Core.LoginInfo.IsDebugMode = e.Args[0].ToUpper() == "D";
			else
				Soheil.Core.LoginInfo.IsDebugMode = false;

			base.OnStartup(e);
		}
		void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			using (var log = new StreamWriter(Path.Combine(folder, "Exceptions.log"), append: true))
			{
				log.WriteLine(DateTime.Now.ToShortTimeString());
				log.Write("Ex = ");
				log.WriteLine(e.Exception.Message);
				if (e.Exception.InnerException != null)
				{
					log.Write("InnerEx = ");
					log.WriteLine(e.Exception.InnerException.Message);
				}
				log.Write("Source = ");
				log.WriteLine(e.Exception.Source);
				log.Write("Trace = ");
				log.WriteLine(e.Exception.StackTrace);
				log.WriteLine();
				log.Close();
			}
			e.Handled = true;
		}
	}
}