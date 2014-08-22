using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Soheil.Core.ViewModels.SetupTime;


namespace Soheil.Core.ViewModels
{
    public class UnitRelation : DependencyObject
    {
        public UnitRelation(Model.UnitConversion model)
        {
            if (model == null) return;
            Id = model.Id;
            Factor = model.Factor;
        }

        public int Id { get; private set; }
        //Name Dependency Property
        public long Factor
        {
            get { return (long) GetValue(FactorProperty); }
            set { SetValue(FactorProperty, value); }
        }

        public static readonly DependencyProperty FactorProperty =
            DependencyProperty.Register("Factor", typeof (long), typeof (UnitRelation), new PropertyMetadata(null));


        //ColumnHeaders Observable Collection
        private ObservableCollection<Cell> _columnHeaders = new ObservableCollection<Cell>();

        public ObservableCollection<Cell> ColumnHeaders
        {
            get { return _columnHeaders; }
        }

        //RowHeaders Observable Collection
        private ObservableCollection<Cell> _rowHeaders = new ObservableCollection<Cell>();

        public ObservableCollection<Cell> RowHeaders
        {
            get { return _rowHeaders; }
        }

        //ColumnPGList Observable Collection
        private ObservableCollection<SetupTime.ProductGroup> _columnPGList =
            new ObservableCollection<SetupTime.ProductGroup>();

        public ObservableCollection<SetupTime.ProductGroup> ColumnPGList
        {
            get { return _columnPGList; }
        }

        //RowPGList Observable Collection
        private ObservableCollection<SetupTime.ProductGroup> _rowPGList =
            new ObservableCollection<SetupTime.ProductGroup>();

        public ObservableCollection<SetupTime.ProductGroup> RowPGList
        {
            get { return _rowPGList; }
        }

        //ChangeoverCells Observable Collection
        private ObservableCollection<Cell> _changeoverCells = new ObservableCollection<Cell>();

        public ObservableCollection<Cell> ChangeoverCells
        {
            get { return _changeoverCells; }
        }

        //TotalVisualColumns Dependency Property
        public int TotalVisualColumns
        {
            get { return (int) GetValue(TotalVisualColumnsProperty); }
            set { SetValue(TotalVisualColumnsProperty, value); }
        }

        public static readonly DependencyProperty TotalVisualColumnsProperty =
            DependencyProperty.Register("TotalVisualColumns", typeof (int), typeof (SetupTimeTableVm),
                new UIPropertyMetadata(0));

        //TotalVisualRows Dependency Property
        public int TotalVisualRows
        {
            get { return (int) GetValue(TotalVisualRowsProperty); }
            set { SetValue(TotalVisualRowsProperty, value); }
        }

        public static readonly DependencyProperty TotalVisualRowsProperty =
            DependencyProperty.Register("TotalVisualRows", typeof (int), typeof (SetupTimeTableVm),
                new UIPropertyMetadata(0));

        public void Reload()
        {
            var pgModelList = new DataServices.ProductGroupDataService(null).GetActivesRecursive(Id);

            #region Reload Sync

            int col = 0;
            int row = 0;
            foreach (var pgModel in pgModelList)
            {
                if (!pgModel.Products.Any()) continue;

                //    //Add ProductGroup To ColumnHeaders
                //    var cpg = new SetupTime.ProductGroup(pgModel, this, isRowHeader: false);
                //    cpg.RowIndex = 0;
                //    cpg.ColumnIndex = col++;//pg itself is a column
                //    ColumnHeaders.Add(cpg);
                //    ColumnPGList.Add(cpg);
                //    foreach (var p in cpg.Products)
                //    {
                //        foreach (var r in p.Reworks)
                //        {
                //            //Add Rework To ColumnHeaders
                //            r.RowIndex = 0;
                //            r.ColumnIndex = col;
                //            ColumnHeaders.Add(r);
                //            //...
                //            col++;
                //        }
                //    }

                //    //Add ProductGroup To RowHeaders
                //    var rpg = new SetupTime.ProductGroup(pgModel, this, isRowHeader: true);
                //    rpg.RowIndex = row++;//pg itself is a row
                //    rpg.ColumnIndex = 0;
                //    RowHeaders.Add(rpg);
                //    RowPGList.Add(rpg);
                //    foreach (var p in rpg.Products)
                //    {
                //        foreach (var r in p.Reworks)
                //        {
                //            //Add Rework to RowHeaders
                //            r.RowIndex = row;
                //            r.ColumnIndex = 0;
                //            RowHeaders.Add(r);
                //            //Add Checkbox
                //            r.Checkbox.RowIndex = row;
                //            r.Checkbox.ColumnIndex = 1;
                //            RowHeaders.Add(r.Checkbox);
                //            //Add Warmup
                //            r.Warmup.RowIndex = row;
                //            r.Warmup.ColumnIndex = 2;
                //            RowHeaders.Add(r.Warmup);

                //            //...
                //            row++;
                //        }
                //    }
                //}

                //				*********
                //use of Context in a non DataService class
                //This is to speed up the reading of changeovers
                //				*********
                using (var context = new Dal.SoheilEdmContext())
                {
                    //Add ChangeoverCells
                    foreach (var rpg in RowPGList)
                    {
                        foreach (var cpg in ColumnPGList)
                        {
                            ChangeoverCells.Add(Cell.Blank(rpg, cpg));
                            foreach (var cp in cpg.Products)
                                foreach (var cr in cp.Reworks)
                                    ChangeoverCells.Add(Cell.Blank(rpg, cr, columnColor: cp.Color));
                        }
                        foreach (var rp in rpg.Products)
                            foreach (var rr in rp.Reworks)
                                foreach (var cpg in ColumnPGList)
                                {
                                    ChangeoverCells.Add(Cell.Blank(rr, cpg, rowColor: rr.Product.Color));
                                    foreach (var cp in cpg.Products)
                                        foreach (var cr in cp.Reworks)
                                            if (cr.IsValid)
                                                ChangeoverCells.Add(new ChangeoverCell(rr, cr, this.Id, context));
                                            else
                                                ChangeoverCells.Add(Cell.Blank(rr, cr, rowColor: rr.Product.Color,
                                                    columnColor: cr.Product.Color));
                                }
                    }
                }
                //				*********
                TotalVisualColumns = col;
                TotalVisualRows = row;

                #endregion
            }
        }
    }
}