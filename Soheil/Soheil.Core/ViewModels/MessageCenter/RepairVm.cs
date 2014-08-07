using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Common;

namespace Soheil.Core.ViewModels.MessageCenter
{
	public class RepairVm : NotificationVm
	{
		public Model.Repair Model { get; set; }
		public RepairVm(Model.Repair repair)
		{
			if (repair.RepairStatus == (byte)RepairStatus.NotDone)
			{
				NotificationType = Common.NotificationType.Info;
				Message = string.Format("ماشین {0} در حال تعمیر است.", repair.MachinePart.Machine.Name);
				if(repair.MachinePart.Part == null)
					MoreInfo = string.Format("ماشین {0} در تاریخ {1} به منظور تعمیر از تولید خارج شد و هنوز تعمیر نشده است", repair.MachinePart.Machine.Name, repair.AcquiredDate);
				else
					MoreInfo = string.Format("قطعه {0} از ماشین {1} در تاریخ {2} به منظور تعمیر از تولید خارج شد و هنوز تعمیر نشده است", repair.MachinePart.Part.Name, repair.MachinePart.Machine.Name, repair.AcquiredDate);
			}
			else if(repair.RepairStatus == (byte)RepairStatus.Reported)
			{
				NotificationType = Common.NotificationType.Alarm;
				Message = string.Format("ماشین {0} نیاز به تعمیر دارد.", repair.MachinePart.Machine.Name);
				if (repair.MachinePart.Part == null)
					MoreInfo = string.Format("برای ماشین {0} در تاریخ {1} درخواست تعمیر ثبت شد و هنوز از تولید خارج نشده است", repair.MachinePart.Machine.Name, repair.CreatedDate);
				else
					MoreInfo = string.Format("برای قطعه {0} از ماشین {1} در تاریخ {2} درخواست تعمیر ثبت شد و هنوز از تولید خارج نشده است", repair.MachinePart.Part.Name, repair.MachinePart.Machine.Name, repair.CreatedDate);
			}
			//Message = string.Format("Machine {0} is being repaired.", repair.MachinePart.Machine.Name);
		}
	}
}
