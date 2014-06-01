using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Soheil.Common
{
    public class CustomDataGridDocumentPaginator : DocumentPaginator
    {
        #region Private Members

        private readonly DataGrid _documentSource;

        private readonly Collection<ColumnDefinition> _tableColumnDefinitions;

        private double _avgRowHeight;

        private double _availableHeight;

        private int _rowsPerPage;

        private int _pageCount;

        #endregion

        #region Constructor

        public CustomDataGridDocumentPaginator(DataGrid documentSource, string documentTitle, Size pageSize, Thickness pageMargin)
        {
            _tableColumnDefinitions = new Collection<ColumnDefinition>();
            _documentSource = documentSource;

            _tableColumnDefinitions = new Collection<ColumnDefinition>();
            _documentSource = documentSource;

            DocumentTitle = documentTitle;
            PageSize = pageSize;
            PageMargin = pageMargin;

            if (_documentSource != null)
                MeasureElements();
        }

        #endregion

        #region Public Properties

        #region Styling

        public Style AlternatingRowBorderStyle { get; set; }

        public Style DocumentHeaderTextStyle { get; set; }

        public Style DocumentFooterTextStyle { get; set; }

        public Style TableCellTextStyle { get; set; }

        public Style TableHeaderTextStyle { get; set; }

        public Style TableHeaderBorderStyle { get; set; }

        public Style GridContainerStyle { get; set; }

        #endregion

        public string DocumentTitle { get; set; }

        public Thickness PageMargin { get; set; }

        public override sealed Size PageSize { get; set; }

        public override bool IsPageCountValid
        {
            get { return true; }
        }

        public override int PageCount
        {
            get { return _pageCount; }
        }

        public override IDocumentPaginatorSource Source
        {
            get { return null; }
        }

        #endregion

        #region Public Methods

        public override DocumentPage GetPage(int pageNumber)
        {
            DocumentPage page = null;
            var itemsSource = new List<object>();

            ICollectionView viewSource = CollectionViewSource.GetDefaultView(_documentSource.ItemsSource);

            if (viewSource != null)
            {
                itemsSource.AddRange(viewSource.Cast<object>());
            }

            {
                int rowIndex = 1;
                int startPos = pageNumber * _rowsPerPage;
                int endPos = startPos + _rowsPerPage;

                //Create a new grid
                var tableGrid = CreateTable(true) as Grid;

                for (int index = startPos; index < endPos && index < itemsSource.Count; index++)
                {
                    Console.WriteLine(@"Adding: " + index);

                    if (rowIndex > 0)
                    {
                        object item = itemsSource[index];
                        int columnIndex = 0;

                        if (_documentSource.Columns != null)
                        {
                            foreach (DataGridColumn column in _documentSource.Columns)
                            {
                                if (column.Visibility == Visibility.Visible)
                                {
                                    AddTableCell(tableGrid, column, item, columnIndex, rowIndex);
                                    columnIndex++;
                                }
                            }
                        }

                        if (AlternatingRowBorderStyle != null && rowIndex % 2 == 0)
                        {
                            var alernatingRowBorder = new Border {Style = AlternatingRowBorderStyle};
                            alernatingRowBorder.SetValue(Grid.RowProperty, rowIndex);
                            alernatingRowBorder.SetValue(Grid.ColumnSpanProperty, columnIndex);
                            alernatingRowBorder.SetValue(Panel.ZIndexProperty, -1);
                            tableGrid.Children.Add(alernatingRowBorder);
                        }
                    }

                    rowIndex++;
                }

                page = ConstructPage(tableGrid, pageNumber);
            }

            return page;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This function measures the heights of the page header, page footer and grid header and the first row in the grid
        /// in order to work out how manage pages might be required.
        /// </summary>
        private void MeasureElements()
        {
            double allocatedSpace = 0;

            //Measure the page header
            var pageHeader = new ContentControl {Content = CreateDocumentHeader()};
            allocatedSpace = MeasureHeight(pageHeader);

            //Measure the page footer
            var pageFooter = new ContentControl {Content = CreateDocumentFooter(0)};
            allocatedSpace += MeasureHeight(pageFooter);

            //Measure the table header
            var tableHeader = new ContentControl {Content = CreateTable(false)};
            allocatedSpace += MeasureHeight(tableHeader);

            //Include any margins
            allocatedSpace += PageMargin.Bottom + PageMargin.Top;

            //Work out how much space we need to display the grid
            _availableHeight = PageSize.Height - allocatedSpace;

            //Calculate the height of the first row
            _avgRowHeight = MeasureHeight(CreateTempRow());

            //Calculate how many rows we can fit on each page
            double rowsPerPage = Math.Floor(_availableHeight / _avgRowHeight);

            if (!double.IsInfinity(rowsPerPage))
                _rowsPerPage = Convert.ToInt32(rowsPerPage);

            //Count the rows in the document source
            double rowCount = CountRows(_documentSource.ItemsSource);

            //Calculate the nuber of pages that we will need
            if (rowCount > 0)
                _pageCount = Convert.ToInt32(Math.Ceiling(rowCount / rowsPerPage));
        }

        /// <summary>
        /// This method constructs the document page (visual) to print
        /// </summary>
        private DocumentPage ConstructPage(Grid content, int pageNumber)
        {
            if (content == null)
                return null;

            //Build the page inc header and footer
            var pageGrid = new Grid();
            pageGrid.FlowDirection = FlowDirection.RightToLeft;

            //Header row
            AddGridRow(pageGrid, GridLength.Auto);

            //Content row
            AddGridRow(pageGrid, new GridLength(1.0d, GridUnitType.Star));

            //Footer row
            AddGridRow(pageGrid, GridLength.Auto);

            var pageHeader = new ContentControl {Content = CreateDocumentHeader()};
            pageGrid.Children.Add(pageHeader);

            content.SetValue(Grid.RowProperty, 1);
            pageGrid.Children.Add(content);

            var pageFooter = new ContentControl {Content = CreateDocumentFooter(pageNumber + 1)};
            pageFooter.SetValue(Grid.RowProperty, 2);

            pageGrid.Children.Add(pageFooter);

            double width = PageSize.Width - (PageMargin.Left + PageMargin.Right);
            double height = PageSize.Height - (PageMargin.Top + PageMargin.Bottom);

            pageGrid.Measure(new Size(width, height));
            pageGrid.Arrange(new Rect(PageMargin.Left, PageMargin.Top, width, height));

            return new DocumentPage(pageGrid);
        }

        /// <summary>
        /// Creates a default header for the document; containing the doc title
        /// </summary>
        private object CreateDocumentHeader()
        {
            var headerBorder = new Border();
            var titleText = new TextBlock
            {
                Style = DocumentHeaderTextStyle,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Text = DocumentTitle,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            headerBorder.Child = titleText;

            return headerBorder;
        }

        /// <summary>
        /// Creates a default page footer consisting of datetime and page number
        /// </summary>
        private object CreateDocumentFooter(int pageNumber)
        {
            var footerGrid = new Grid {Margin = new Thickness(0, 10, 0, 0)};

            var colDefinition = new ColumnDefinition {Width = new GridLength(0.5d, GridUnitType.Star)};

            var dateTimeText = new TextBlock
            {
                Style = DocumentFooterTextStyle,
                Text = DateTime.Now.ToString("dd-MMM-yyy HH:mm")
            };

            footerGrid.Children.Add(dateTimeText);

            var pageNumberText = new TextBlock
            {
                Style = DocumentFooterTextStyle,
                Text = "Page " + pageNumber + " of " + PageCount
            };
            pageNumberText.SetValue(Grid.ColumnProperty, 1);
            pageNumberText.HorizontalAlignment = HorizontalAlignment.Right;

            footerGrid.Children.Add(pageNumberText);

            return footerGrid;
        }

        /// <summary>
        /// Counts the number of rows in the document source
        /// </summary>
        /// <param name="itemsSource"></param>
        /// <returns></returns>
        private double CountRows(IEnumerable itemsSource)
        {
            int count = 0;

            if (itemsSource != null)
            {
                count += itemsSource.Cast<object>().Count();
            }

            return count;
        }

        /// <summary>
        /// The following function creates a temp table with a single row so that it can be measured and used to 
        /// calculate the totla number of req'd pages
        /// </summary>
        /// <returns></returns>
        private Grid CreateTempRow()
        {
            var tableRow = new Grid();

            if (_documentSource != null)
            {
                foreach (ColumnDefinition colDefinition in _tableColumnDefinitions)
                {
                    var copy = XamlReader.Parse(XamlWriter.Save(colDefinition)) as ColumnDefinition;
                    tableRow.ColumnDefinitions.Add(copy);
                }

                foreach (object item in _documentSource.ItemsSource)
                {
                    int columnIndex = 0;
                    if (_documentSource.Columns != null)
                    {
                        foreach (DataGridColumn column in _documentSource.Columns)
                        {
                            if (column.Visibility == Visibility.Visible)
                            {
                                AddTableCell(tableRow, column, item, columnIndex, 0);
                                columnIndex++;
                            }
                        }
                    }

                    //We only want to measure teh first row
                    break;
                }
            }

            return tableRow;
        }

        /// <summary>
        /// This function counts the number of rows in the document
        /// </summary>
        private object CreateTable(bool createRowDefinitions)
        {
            if (_documentSource == null)
                return null;

            var table = new Grid {Style = GridContainerStyle};

            int columnIndex = 0;


            if (_documentSource.Columns != null)
            {
                double totalColumnWidth = _documentSource.Columns.Sum(column => column.Visibility == Visibility.Visible ? column.Width.Value : 0);

                foreach (DataGridColumn column in _documentSource.Columns)
                {
                    if (column.Visibility == Visibility.Visible)
                    {
                        AddTableColumn(table, totalColumnWidth, columnIndex, column);
                        columnIndex++;
                    }
                }
            }

            if (TableHeaderBorderStyle != null)
            {
                var headerBackground = new Border {Style = TableHeaderBorderStyle};
                headerBackground.SetValue(Grid.ColumnSpanProperty, columnIndex);
                headerBackground.SetValue(Panel.ZIndexProperty, -1);

                table.Children.Add(headerBackground);
            }

            if (createRowDefinitions)
            {
                for (int i = 0; i <= _rowsPerPage; i++)
                    table.RowDefinitions.Add(new RowDefinition());
            }

            return table;

        }

        /// <summary>
        /// Measures the height of an element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private double MeasureHeight(FrameworkElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.Measure(PageSize);
            return element.DesiredSize.Height;
        }

        /// <summary>
        /// Adds a column to a grid
        /// </summary>
        /// <param name="grid">Grid to add the column to</param>
        /// <param name="totalColumnWidth"></param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="column">Source column defintition which will be used to calculate the width of the column</param>
        private void AddTableColumn(Grid grid, double totalColumnWidth, int columnIndex, DataGridColumn column)
        {
            double proportion = column.Width.Value / (PageSize.Width - (PageMargin.Left + PageMargin.Right));

            var colDefinition = new ColumnDefinition {Width = new GridLength(proportion, GridUnitType.Star)};

            grid.ColumnDefinitions.Add(colDefinition);

            var text = new TextBlock
            {
                Style = TableHeaderTextStyle,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Text = column.Header.ToString()
            };
            
            text.SetValue(Grid.ColumnProperty, columnIndex);

            grid.Children.Add(text);
            _tableColumnDefinitions.Add(colDefinition);
        }

        /// <summary>
        /// Adds a cell to a grid
        /// </summary>
        /// <param name="grid">Grid to add teh cell to</param>
        /// <param name="column">Source column definition which contains binding info</param>
        /// <param name="item">The binding source</param>
        /// <param name="columnIndex">Column index</param>
        /// <param name="rowIndex">Row index</param>
        private void AddTableCell(Grid grid, DataGridColumn column, object item, int columnIndex, int rowIndex)
        {
            if (column is DataGridTemplateColumn)
            {
                var templateColumn = column as DataGridTemplateColumn;
                var contentControl = new ContentControl
                {
                    Focusable = true,
                    ContentTemplate = templateColumn.CellTemplate,
                    Content = item
                };

                contentControl.SetValue(Grid.ColumnProperty, columnIndex);
                contentControl.SetValue(Grid.RowProperty, rowIndex);

                grid.Children.Add(contentControl);
            }
            else if (column is DataGridTextColumn)
            {
                var textColumn = column as DataGridTextColumn;
                var text = new TextBlock
                {
                    Text = "Text",
                    Style = TableCellTextStyle,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    DataContext = item
                };

                var binding = textColumn.Binding as Binding;

                //if (!string.IsNullOrEmpty(column.DisplayFormat))
                //binding.StringFormat = column.DisplayFormat;

                text.SetBinding(TextBlock.TextProperty, binding);

                text.SetValue(Grid.ColumnProperty, columnIndex);
                text.SetValue(Grid.RowProperty, rowIndex);

                grid.Children.Add(text);
            }
        }

        /// <summary>
        /// Adds a row to a grid
        /// </summary>
        private void AddGridRow(Grid grid, GridLength rowHeight)
        {
            if (grid == null)
                return;

            var rowDef = new RowDefinition {Height = rowHeight};

            grid.RowDefinitions.Add(rowDef);
        }

        #endregion

    }
}
