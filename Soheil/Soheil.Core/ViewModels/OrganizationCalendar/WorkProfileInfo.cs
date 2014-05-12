using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.OrganizationCalendar
{
	/// <summary>
	/// A simple but informative ViewModel for WorkProfile
	/// <para>It contains Name and Number of shifts</para>
	/// </summary>
    public class WorkProfileInfo : DependencyObject
    {
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(WorkProfileInfo), new PropertyMetadata(""));
        public static readonly DependencyProperty NumberOfShiftsProperty = DependencyProperty.Register("NumberOfShiftsModifier", typeof(int), typeof(WorkProfileInfo), new PropertyMetadata(1));

        public WorkProfileInfo(Model.WorkProfile model)
        {
            Model = model;
            if (model != null)
            {
                Id = model.Id;
                Name = model.Name;
                NumberOfShiftsModifier = model.WorkShiftPrototypes.Count();
            }
        }

        public Model.WorkProfile Model { get; private set; }
        public int Id { get; private set; }
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public int NumberOfShiftsModifier
        {
            get { return (int)GetValue(NumberOfShiftsProperty); }
            set { SetValue(NumberOfShiftsProperty, value); }
        }
    }
}
