//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Soheil.Model
{
    public partial class NonProductiveTask
    {
        #region Primitive Properties
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual int DurationSeconds
        {
            get;
            set;
        }
    
        public virtual System.DateTime StartDateTime
        {
            get;
            set;
        }
    
        public virtual System.DateTime EndDateTime
        {
            get;
            set;
        }
    
        public virtual string Description
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual NonProductiveTaskReport NonProductiveTaskReport
        {
            get { return _nonProductiveTaskReport; }
            set
            {
                if (!ReferenceEquals(_nonProductiveTaskReport, value))
                {
                    var previousValue = _nonProductiveTaskReport;
                    _nonProductiveTaskReport = value;
                    FixupNonProductiveTaskReport(previousValue);
                }
            }
        }
        private NonProductiveTaskReport _nonProductiveTaskReport;

        #endregion

        #region Association Fixup
    
        private void FixupNonProductiveTaskReport(NonProductiveTaskReport previousValue)
        {
            if (previousValue != null && ReferenceEquals(previousValue.NonProductiveTasks, this))
            {
                previousValue.NonProductiveTasks = null;
            }
    
            if (NonProductiveTaskReport != null)
            {
                NonProductiveTaskReport.NonProductiveTasks = this;
            }
        }

        #endregion

    }
}
