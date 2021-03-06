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
    public partial class OperatorDefectionReport
    {
        #region Primitive Properties
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual string Code
        {
            get;
            set;
        }
    
        public virtual int ModifiedBy
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual DefectionReport DefectionReport
        {
            get { return _defectionReport; }
            set
            {
                if (!ReferenceEquals(_defectionReport, value))
                {
                    var previousValue = _defectionReport;
                    _defectionReport = value;
                    FixupDefectionReport(previousValue);
                }
            }
        }
        private DefectionReport _defectionReport;
    
        public virtual Operator Operator
        {
            get { return _operator; }
            set
            {
                if (!ReferenceEquals(_operator, value))
                {
                    var previousValue = _operator;
                    _operator = value;
                    FixupOperator(previousValue);
                }
            }
        }
        private Operator _operator;

        #endregion

        #region Association Fixup
    
        private void FixupDefectionReport(DefectionReport previousValue)
        {
            if (previousValue != null && previousValue.OperatorDefectionReports.Contains(this))
            {
                previousValue.OperatorDefectionReports.Remove(this);
            }
    
            if (DefectionReport != null)
            {
                if (!DefectionReport.OperatorDefectionReports.Contains(this))
                {
                    DefectionReport.OperatorDefectionReports.Add(this);
                }
            }
        }
    
        private void FixupOperator(Operator previousValue)
        {
            if (previousValue != null && previousValue.OperatorDefectionReports.Contains(this))
            {
                previousValue.OperatorDefectionReports.Remove(this);
            }
    
            if (Operator != null)
            {
                if (!Operator.OperatorDefectionReports.Contains(this))
                {
                    Operator.OperatorDefectionReports.Add(this);
                }
            }
        }

        #endregion

    }
}
