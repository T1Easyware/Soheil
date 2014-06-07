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

using System.Windows.Documents;
using Soheil.Controls.Printing.Interfaces;

namespace Soheil.Controls.Printing.Document
{
    /// <summary>
    /// Class for fillable table row values
    /// </summary>
    public class TableRowForDynamicHeader : TableRow, ITableRowForDynamicHeader
    {
        private string _tableName = null;
        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TableRowForDynamicHeader()
        {
        }
    }
}