using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Timers;

namespace Soheil.Common
{
	/// <summary>
	/// This class will help classes with no reference to WindowsBase.dll notify UI about commit
	/// </summary>
	public static class CommitNotifierHelper
	{
		/// <summary>
		/// Make IsCommit True for three seconds
		/// </summary>
		public static void Commit()
		{
			CommitNotifier.Singleton.IsCommit = false;
			CommitNotifier.Singleton.IsCommit = true;
		}
	}

	/// <summary>
	/// This class will link a UOW to View
	/// </summary>
	public class CommitNotifier : DependencyObject
	{
		Timer _t;
		static CommitNotifier _singleton;

		/// <summary>
		/// Gets the only instance of this class
		/// </summary>
		public static CommitNotifier Singleton
		{
			get
			{
				if (_singleton == null) _singleton = new CommitNotifier();
				return _singleton;
			}
		}

		private CommitNotifier()
		{
			_t = new Timer(3000);
			_t.Elapsed += (s, e) =>
			{
				_t.Stop();
				Dispatcher.Invoke(() => IsCommit = false);
			};
		}

		/// <summary>
		/// Gets or sets a value that indicates whether the context is commited
		/// <para>The value turns back to False three seconds after being set to True</para>
		/// </summary>
		public bool IsCommit
		{
			get { return (bool)GetValue(IsCommitProperty); }
			set
			{
				SetValue(IsCommitProperty, value);
				if (value) _t.Start();
				else _t.Stop();
			}
		}
		public static readonly DependencyProperty IsCommitProperty =
			DependencyProperty.Register("IsCommit", typeof(bool), typeof(CommitNotifier), new UIPropertyMetadata(false));
	}
}
