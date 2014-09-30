﻿using System;
using System.Globalization;
using System.Linq;
using Soheil.Core.ViewModels.Reports;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class WarehouseReportDataService
    {
        public WarehouseReportData GetWarehouseReport(DateTime startDate, DateTime endDate)
        {
            var result = new WarehouseReportData();

            using (var context = new SoheilEdmContext())
            {
                var warehouseRepository = new Repository<Warehouse>(context);
                var transactionRepository = new Repository<WarehouseTransaction>(context);
                var rawMaterialRepository = new Repository<RawMaterial>(context);
                var unitRepository = new Repository<UnitSet>(context);
                var productRepository = new Repository<Product>(context);

                var transactionList = transactionRepository.Find(t => t.TransactionDateTime >= startDate && t.TransactionDateTime <= endDate).ToArray();
                var warehouseList = warehouseRepository.GetAll();
                var materialList = rawMaterialRepository.GetAll();
                var unitList = unitRepository.GetAll();
                var productList = productRepository.GetAll();

                var matStorageQuery = from material in materialList
                                      from transaction in transactionList.Where(t => material != null && t.RawMaterial != null && material.Id == t.RawMaterial.Id).DefaultIfEmpty()
                                      from warehouse in warehouseList.Where(w => transaction != null && transaction.DestWarehouse != null && w.Id == transaction.DestWarehouse.Id).DefaultIfEmpty()
                                      from unit in unitList.Where(ug => material != null && material.BaseUnit != null && ug.Id == material.BaseUnit.Id).DefaultIfEmpty()
                                      let matCode = material == null ? string.Empty : material.Code
                                      let matName = material == null ? string.Empty : material.Name
                                      let warehCode = warehouse == null ? string.Empty : warehouse.Code
                                      let warehName = warehouse == null ? string.Empty : warehouse.Name
                                      let unitCode = unit == null ? string.Empty : unit.Code
                                      let unitName = unit == null ? string.Empty : unit.Description
                                      let inventory = material == null ? 0 : material.Inventory
                                      select new { matCode, matName, warehCode, warehName, unitCode, unitName, inventory };

                var productDischargeQuery = from product in productList
                               from transaction in transactionList.Where(t => product != null && t.Product != null && product.Id == t.Product.Id).DefaultIfEmpty()
                               from warehouse in warehouseList.Where(w => transaction != null && transaction.DestWarehouse != null && w.Id == transaction.DestWarehouse.Id).DefaultIfEmpty()
                               let prdId = product == null ? -1 : product.Id
                               let prdCode = product == null ? string.Empty : product.Code
                               let prdName = product == null ? string.Empty : product.Name
                               let warehCode = warehouse == null ? string.Empty : warehouse.Code
                               let warehName = warehouse == null ? string.Empty : warehouse.Name
                               let inventory = product == null ? 0 : product.Inventory
                               let price = transaction == null ? 0 : transaction.Price
                               let qty = transaction == null ? 0 : transaction.Quantity
                               let fee = price * qty
                               select new { prdId, prdCode, prdName, warehCode, warehName, inventory, price, qty, fee };


                result.Title = DateTime.Now.ToShortDateString();

                var matStrList = matStorageQuery.ToList();
                if (matStrList.Any())
                {
                    result.TotalStorage = matStrList.Sum(record => record.inventory);
                    foreach (var line in matStrList)
                    {
                        var matReport = new MaterialInventoryReportData
                        {
                            Material = line.matCode + "-" + line.matName,
                            Warehouse = line.warehCode + "-" + line.warehName,
                            Unit = line.unitCode + "-" + line.unitName,
                            Inventory = line.inventory.ToString(CultureInfo.InvariantCulture)
                        };
                        result.MaterialItems.Add(matReport);
                    }
                }

                var prdDisList = productDischargeQuery.ToList();
                if (prdDisList.Any())
                {
                    result.TotalDischarge = prdDisList.Sum(record => record.inventory);
                    result.TotalSale = prdDisList.Sum(record => record.price);
                    foreach (var line in prdDisList)
                    {
                        var prdReport = new ProductInventoryReportData
                        {
                            Product = line.prdCode + "-" + line.prdName,
                            Warehouse = line.warehCode + "-" + line.warehName,
                            Price = line.price.ToString(CultureInfo.InvariantCulture),
                            Fee = line.fee.ToString(CultureInfo.InvariantCulture),
                            Inventory = line.inventory.ToString(CultureInfo.InvariantCulture)
                        };
                        result.ProductItems.Add(prdReport);
                    }
                }
            }
            return result;
        }
    }
}