﻿/************************************************************************
 * Copyright: Hans Wolff
 *
 * License:  This software abides by the LGPL license terms. For further
 *           licensing information please see the top level LICENSE.txt 
 *           file found in the root directory of CodeReason Reports.
 *
 * Author:   Hans Wolff
 *
 ************************************************************************/

using Soheil.Core.Printing.Interfaces;

namespace Soheil.Core.Printing.Document
{
    /// <summary>
    /// Contains a single report context value that is to be displayed on the report
    /// </summary>
    public class InlineContextValue : InlinePropertyValue, IAggregateValue, IInlineContextValue, IInlinePropertyValue
    {
        private string _aggregateGroup = null;
        /// <summary>
        /// Gets or sets the aggregate group
        /// </summary>
        public string AggregateGroup
        {
            get { return _aggregateGroup; }
            set { _aggregateGroup = value; }
        }
    }
}
