using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.MessageCenter
{
	public class PMVm : NotificationVm
	{
		public Model.MachinePartMaintenance Model { get; set; }
		public PMVm(Model.MachinePartMaintenance mpm)
		{
			Model = mpm;
			if(mpm.CalculatedDiffDays == 0)
			{
				NotificationType = Common.NotificationType.Alarm;
				Message = string.Format(
					"تاریخ PM ماشین {0} امروز است.",
					mpm.MachinePart.Machine.Name);
				//Message = string.Format(
				//	"Maintenance date for machine {0} is today.",
				//	mpm.MachinePart.Machine.Name);
				if (mpm.MachinePart.Part == null)
					MoreInfo = string.Format("تاریخ PM {0} ماشین {1} امروز است", mpm.Maintenance.Name, mpm.MachinePart.Machine.Name);
				else
					MoreInfo = string.Format("تاریخ PM {0} قطعه {1} از ماشین {2} امروز است", mpm.Maintenance.Name, mpm.MachinePart.Part.Name, mpm.MachinePart.Machine.Name);
			}
			else if (mpm.CalculatedDiffDays < 0)
			{
				NotificationType = Common.NotificationType.Critical;
				Message = string.Format(
					"تاریخ PM ماشین {0} {1} بود.",
					mpm.MachinePart.Machine.Name,
					mpm.CalculatedDiffDays == -1 ? "دیروز" :
					string.Format("{0} روز پیش", -mpm.CalculatedDiffDays));
				//Message = string.Format(
				//	"Maintenance date for machine {0} was {1}.",
				//	mpm.MachinePart.Machine.Name,
				//	mpm.CalculatedDiffDays == -1 ? "yesterday" :
				//	string.Format("{0} days ago", -mpm.CalculatedDiffDays));
				if (mpm.MachinePart.Part == null)
					MoreInfo = string.Format("{0} روز از تاریخ PM {1} ماشین {2} گذشته است", -mpm.CalculatedDiffDays, mpm.Maintenance.Name, mpm.MachinePart.Machine.Name);
				else
					MoreInfo = string.Format("{0} روز از تاریخ PM {1} قطعه {2} از ماشین {3} گذشته است", -mpm.CalculatedDiffDays, mpm.Maintenance.Name, mpm.MachinePart.Part.Name, mpm.MachinePart.Machine.Name);
			}
			else
			{
				NotificationType = Common.NotificationType.Info;
				Message = string.Format(
					"تاریخ PM ماشین {0} {1} است.",
					mpm.MachinePart.Machine.Name,
					mpm.CalculatedDiffDays == 1 ? "فردا" :
					string.Format("{0} روز بعد", mpm.CalculatedDiffDays));
				//Message = string.Format(
				//	"Maintenance date for machine {0} is {1}.",
				//	mpm.MachinePart.Machine.Name,
				//	mpm.CalculatedDiffDays == 1 ? "tomorrow" :
				//	string.Format("in {0} days", mpm.CalculatedDiffDays));
				if (mpm.MachinePart.Part == null)
					MoreInfo = string.Format("{0} روز دیگر تاریخ PM {1} ماشین {2} خواهد بود", mpm.CalculatedDiffDays, mpm.Maintenance.Name, mpm.MachinePart.Machine.Name);
				else
					MoreInfo = string.Format("{0} روز دیگر تاریخ PM {1} قطعه {2} از ماشین {3} خواهد بود", mpm.CalculatedDiffDays, mpm.Maintenance.Name, mpm.MachinePart.Part.Name, mpm.MachinePart.Machine.Name);
			}
		}
	}
}
