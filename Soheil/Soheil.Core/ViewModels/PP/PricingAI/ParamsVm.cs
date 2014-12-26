using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Core.PP.PricingAI;

namespace Soheil.Core.ViewModels.PP.PricingAI
{
	public class ParamsVm : DependencyObject
	{
		/// <summary>
		/// Creates a param with default values
		/// </summary>
		public ParamsVm() { }
		/// <summary>
		/// Creates a param with given values
		/// </summary>
		/// <param name="data"></param>
		public ParamsVm(Params data)
		{
			maxRuns = data.maxRuns;
			timeLimit = data.timeLimit;
			idleCount = data.idleCount;
			maxDfss = data.maxDfss;
			mmSize = data.mmSize;
			memorySize = data.memorySize;
			maxInitPop = data.maxInitPop;
			translationFunction = data.translationFunction;
		}


		/// <summary>
		/// Gets or sets a bindable value that indicates maxRuns
		/// </summary>
		public int maxRuns
		{
			get { return (int)GetValue(maxRunsProperty); }
			set { SetValue(maxRunsProperty, value); }
		}
		public static readonly DependencyProperty maxRunsProperty =
			DependencyProperty.Register("maxRuns", typeof(int), typeof(ParamsVm), new PropertyMetadata(1));
		/// <summary>
		/// Gets or sets a bindable value that indicates timeLimit
		/// </summary>
		public int timeLimit
		{
			get { return (int)GetValue(timeLimitProperty); }
			set { SetValue(timeLimitProperty, value); }
		}
		public static readonly DependencyProperty timeLimitProperty =
			DependencyProperty.Register("timeLimit", typeof(int), typeof(ParamsVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates idleCount
		/// </summary>
		public int idleCount
		{
			get { return (int)GetValue(idleCountProperty); }
			set { SetValue(idleCountProperty, value); }
		}
		public static readonly DependencyProperty idleCountProperty =
			DependencyProperty.Register("idleCount", typeof(int), typeof(ParamsVm), new PropertyMetadata(500));
		/// <summary>
		/// Gets or sets a bindable value that indicates maxDfss
		/// </summary>
		public int maxDfss
		{
			get { return (int)GetValue(maxDfssProperty); }
			set { SetValue(maxDfssProperty, value); }
		}
		public static readonly DependencyProperty maxDfssProperty =
			DependencyProperty.Register("maxDfss", typeof(int), typeof(ParamsVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates mmSize
		/// </summary>
		public int mmSize
		{
			get { return (int)GetValue(mmSizeProperty); }
			set { SetValue(mmSizeProperty, value); }
		}
		public static readonly DependencyProperty mmSizeProperty =
			DependencyProperty.Register("mmSize", typeof(int), typeof(ParamsVm), new PropertyMetadata(30));
		/// <summary>
		/// Gets or sets a bindable value that indicates memorySize
		/// </summary>
		public int memorySize
		{
			get { return (int)GetValue(memorySizeProperty); }
			set { SetValue(memorySizeProperty, value); }
		}
		public static readonly DependencyProperty memorySizeProperty =
			DependencyProperty.Register("memorySize", typeof(int), typeof(ParamsVm), new PropertyMetadata(100));
		/// <summary>
		/// Gets or sets a bindable value that indicates maxInitPop
		/// </summary>
		public int maxInitPop
		{
			get { return (int)GetValue(maxInitPopProperty); }
			set { SetValue(maxInitPopProperty, value); }
		}
		public static readonly DependencyProperty maxInitPopProperty =
			DependencyProperty.Register("maxInitPop", typeof(int), typeof(ParamsVm), new PropertyMetadata(100, (d, e) => { }, (d, v) =>
			{
				var vm = (ParamsVm)d;
				if ((int)v < vm.mmSize) return vm.mmSize;
				return v;
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates translationFunction
		/// </summary>
		public int translationFunction
		{
			get { return (int)GetValue(translationFunctionProperty); }
			set { SetValue(translationFunctionProperty, value); }
		}
		public static readonly DependencyProperty translationFunctionProperty =
			DependencyProperty.Register("translationFunction", typeof(int), typeof(ParamsVm), new PropertyMetadata(1));


	}
}
