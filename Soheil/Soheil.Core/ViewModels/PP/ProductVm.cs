﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.PP
{
	public class ProductVm : DependencyObject
	{
		/// <summary>
		/// can also be used in JobEditor
		/// </summary>
		/// <param name="model"></param>
		/// <param name="parentVm"></param>
		public ProductVm(Model.Product model, ProductGroupVm parentVm)
		{
			if (model == null) return;
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			Color = model.Color;
			Group = parentVm;
			foreach (var pr_model in model.ProductReworks)
			{
				ProductReworks.Add(new ProductReworkVm(pr_model, this));
			}
			CreateNewJob = new Commands.Command
				(vm =>
					((Soheil.Core.ViewModels.PP.Editor.PPJobEditorVm)vm).JobList.Add(
						Soheil.Core.ViewModels.PP.Editor.PPEditorJob.CreateForProduct(model)
					)
				);
		}

		public int Id { get; protected set; }
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ProductVm), new UIPropertyMetadata(null));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(ProductVm), new UIPropertyMetadata(null));
		//Color Dependency Property
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(ProductVm), new UIPropertyMetadata(Colors.White));
		
		//Group Dependency Property
		public ProductGroupVm Group
		{
			get { return (ProductGroupVm)GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}
		public static readonly DependencyProperty GroupProperty =
			DependencyProperty.Register("Group", typeof(ProductGroupVm), typeof(ProductVm), new UIPropertyMetadata(null));
		
		//ProductReworks Observable Collection
		public ObservableCollection<ProductReworkVm> ProductReworks { get { return _productReworks; } }
		private ObservableCollection<ProductReworkVm> _productReworks = new ObservableCollection<ProductReworkVm>();

		//CreateNewJob Dependency Property
		public Commands.Command CreateNewJob
		{
			get { return (Commands.Command)GetValue(CreateNewJobProperty); }
			set { SetValue(CreateNewJobProperty, value); }
		}
		public static readonly DependencyProperty CreateNewJobProperty = 
			DependencyProperty.Register("CreateNewJob", typeof(Commands.Command), typeof(ProductVm), new UIPropertyMetadata(null));
	}
}
