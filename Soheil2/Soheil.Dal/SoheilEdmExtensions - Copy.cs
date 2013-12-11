using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soheil.Dal
{
    #region Extensions
    public partial class AccessRule
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class ActionPlan
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class ActionPlan_Root
    {
    }

    public partial class Activity
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class ActivityGroup
    {
    }

    public partial class Cause
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class CauseL1
    {
    }

    public partial class CauseL2
    {
    }

    public partial class CauseL3
    {
    }

    public partial class ComplexDateTime
    {
        public PersianDayOfWeek DayOfWeek
        {
            get { return (PersianDayOfWeek)DayOfWeekNr; }
            set { DayOfWeekNr = (byte)value; }
        }
        public PersianMonth Month
        {
            get { return (PersianMonth)MonthNr; }
            set { MonthNr = (byte)value; }
        }
    }

    public partial class Connector
    {
    }

    public partial class CostCenter
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class Defection
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class Defection_Root
    {
    }

    public partial class DefectionReport
    {
    }

    public partial class FPC
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class FPCSet
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class GeneralSkill
    {
    }

    public partial class Machine
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class MachineFamily
    {
    }

    public partial class Operator
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class Operator_Activity
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
        public ILUO Iluo
        {
            get { return (ILUO)IluoNr; }
            set { IluoNr = (byte)value; }
        }
    }

    public partial class Operator_DefectionReport
    {
    }

    public partial class Operator_StoppageReport
    {
    }

    public partial class PPSquare
    {
        public PPFlags PPFlags
        {
            get { return (PPFlags)PPFlagsNr; }
            set { PPFlagsNr = (byte)value; }
        }
    }

    public partial class PPSubsquare
    {
        public PPFlags PPFlags
        {
            get { return (PPFlags)PPFlagsNr; }
            set { PPFlagsNr = (byte)value; }
        }
    }

    public partial class PPSubsquare_Activity
    {
    }

    public partial class PPSubsquareActivity_Operator
    {
        public OperatorRole Role
        {
            get { return (OperatorRole)RoleNr; }
            set { RoleNr = (byte)value; }
        }
    }

    public partial class PPSubsquareActivityReport
    {
    }

    public partial class PPSubsquareReport
    {
        public ConfirmationStatus ConfirmationStatus
        {
            get { return (ConfirmationStatus)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class Product
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class Product_Defection
    {
    }

    public partial class Product_Rework
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class Rework
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class Root
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class SpecialSkill
    {
    }

    public partial class State
    {
    }

    public partial class State_Activity
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class State_Station
    {
    }

    public partial class Station
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class StoppageReport
    {
    }

    public partial class User
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class User_AccessRule
    {
    }

    public partial class User_UserGroup
    {
    }

    public partial class UserGroup
    {
        public Status RecordStatus
        {
            get { return (Status)Status; }
            set { Status = (byte)value; }
        }
    }

    public partial class UserGroup_AccessRule
    {
    } 
    #endregion
}