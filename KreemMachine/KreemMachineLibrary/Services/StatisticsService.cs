﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KreemMachineLibrary.Models;
using KreemMachineLibrary.DTO;

namespace KreemMachineLibrary.Services
{
    public class StatisticsService
    {
        //DataBaseContext db = Globals.db;

        #region Statistics per shift
        public ObservableCollection<ResourcesPerShiftDTO> GetResourcesPerShift(DateTime displayMonth, string cbMornnig, string cbNoon, string cbNight)
        {
            using (var db = new DataBaseContext())
            {
                var nextMonth = displayMonth.AddMonths(1);

                var result = from ss in db.ScheduledShifts
                             join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                             where ss.Date >= displayMonth && ss.Date < nextMonth && (ss.Shift.Name == cbMornnig || ss.Shift.Name == cbNoon || ss.Shift.Name == cbNight)
                             select new { SS = ss, US = us } into joined
                             group joined by new
                             {
                                 joined.SS.Date,
                                 joined.SS.Shift
                             } into g
                             select new ResourcesPerShiftDTO
                             {
                                 Date = g.Key.Date,
                                 Shift = g.Key.Shift.Name,
                                 NumberOfEmployees = g.Count(),
                                 Cost = g.Sum(us => us.US.HourlyWage * us.SS.Duration)
                             };

                return new ObservableCollection<ResourcesPerShiftDTO>(result);
            }
        }

        public ObservableCollection<ResourcesPerShiftDTO> GetResourcesPerShift(DateTime displayMonth)
        {
            return GetResourcesPerShift(displayMonth, "Morning", "Noon", "Night");
        }
        #endregion

        #region Statistics per month
        public ObservableCollection<ResourcesPerMonthDTO> GetResourcesPerMonth(DateTime displayYear)
        {
            using (var db = new DataBaseContext())
            {
                var nextYear = displayYear.AddYears(1);

                var result = from ss in db.ScheduledShifts
                             join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                             where ss.Date >= displayYear && ss.Date <= nextYear
                             select new { SS = ss, US = us } into joined
                             group joined by new
                             {
                                 joined.SS.Date.Month
                             } into g
                             select new ResourcesPerMonthDTO
                             {
                                 Month = g.FirstOrDefault().SS.Date,
                                 NumberOfEmployees = g.Count(),
                                 Cost = g.Sum(us => us.US.HourlyWage * us.SS.Duration)
                             };


                return new ObservableCollection<ResourcesPerMonthDTO>(result);
            }
        }
        #endregion

        #region Employee statistics
        public ObservableCollection<ResourcesPerEmployeeDTO> GetResourcesPerEmployeeDate(DateTime fromDate, DateTime toDate)
        {
            using (var db = new DataBaseContext())
            {
                var result = from ss in db.ScheduledShifts
                             join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                             where ss.Date >= fromDate && ss.Date < toDate
                             select new { SS = ss, US = us } into joined
                             group joined by new
                             {
                                 user = joined.US.User,
                             } into g
                             select new ResourcesPerEmployeeDTO
                             {
                                 EmployeeName = g.Key.user.FirstName + " " + g.Key.user.LastName,
                                 NumberOfScheduledShifts = g.Count(),
                                 HoursWorked = g.Sum(y => y.SS.Duration),
                                 Cost = g.Sum(x => x.US.HourlyWage * x.SS.Duration),
                             };

                return new ObservableCollection<ResourcesPerEmployeeDTO>(result);
            }
        }
        #endregion

        #region Stock statistics

        public ObservableCollection<StockStatisticsPriceDTO> GetStockStatisticsPrice(DateTime fromDate, DateTime toDate, Department category)
        {
            using (var db = new DataBaseContext())
            {
                var result = from products in db.Products
                             join productSale in db.ProductSales on products.Id equals productSale.ProductId
                             where productSale.Timestamp >= fromDate && productSale.Timestamp <= toDate && products.DepartmentId == category.Id
                             select new { p = products, ps = productSale } into joined
                             group joined by new
                             {
                                 product = joined.p.Name,
                                 totalPrice = joined.p.SellPrice * joined.ps.Quantity
                             } into g
                             select new StockStatisticsPriceDTO
                             {
                                 item = g.Key.product,
                                 price = g.Key.totalPrice,
                             };

                return new ObservableCollection<StockStatisticsPriceDTO>(result);
            }
        }

        public ObservableCollection<StockStatisticsAmountDTO> GetStockStatisticsAmount(DateTime fromDate, DateTime toDate, Department category)
        {
            using (var db = new DataBaseContext())
            {
                var result = from products in db.Products
                             join productSale in db.ProductSales on products.Id equals productSale.ProductId
                             where productSale.Timestamp >= fromDate && productSale.Timestamp <= toDate && products.DepartmentId == category.Id
                             select new { p = products, ps = productSale } into joined
                             group joined by new
                             {
                                 product = joined.p,
                                 prodduct_sale = joined.ps,
                             } into g
                             select new StockStatisticsAmountDTO
                             {
                                 item = g.Key.product.Name,
                                 amount = g.Sum(x => x.ps.Quantity),
                             };

                return new ObservableCollection<StockStatisticsAmountDTO>(result);
            }
        }

        public float CalculateProfit(Product product)
        {
            using (var db = new DataBaseContext())
            {
                return db.Products.Where(p => p.Id == product.Id)
                                  .Select(p => (p.SellPrice - p.BuyCost) * p.Quantity)
                                  .FirstOrDefault();
            }
        }

        public string GetMostSellingProduct()
        {
            using (var db = new DataBaseContext())
            {
                var query = db.ProductSales?
                    .GroupBy(p => p.Id)
                    .Select(g => new { SoldProduct = g.FirstOrDefault() })
                    .OrderByDescending(g => g.SoldProduct.Quantity)
                    .FirstOrDefault()
                    .SoldProduct.Product.Name;
                return query;
            }
        }

        public string GetLeastSellingProduct()
        {
            using (var db = new DataBaseContext())
            {
                var query = db.ProductSales?
                    .GroupBy(p => p.Id)
                    .Select(g => new { SoldProduct = g.FirstOrDefault() })
                    .OrderBy(g => g.SoldProduct.Quantity)
                    .FirstOrDefault()
                    .SoldProduct.Product.Name;
                return query;
            }
        }

        public string GetMostProfitableProduct()
        {
            using (var db = new DataBaseContext())
            {
                var query = db.ProductSales?
                    .GroupBy(p => p.ProductId)
                    .Select(g => new { Product = g.FirstOrDefault().Product, Profit = g.Sum(p => (p.Product.SellPrice - p.Product.BuyCost) * p.Quantity) })
                    .OrderByDescending(g => g.Profit)
                    .ToList();



                return query.FirstOrDefault()?.Product?.Name;

            }
        }

        public string GetLeastProfitableProduct()
        {
            using (var db = new DataBaseContext())
            {

                var query = db.ProductSales?
                    .GroupBy(p => p.ProductId)
                    .Select(g => new { Product = g.FirstOrDefault().Product, Profit = g.Sum(p => (p.Product.SellPrice - p.Product.BuyCost) * p.Quantity) })
                    .OrderBy(g => g.Profit)
                    .ToList();



                return query.FirstOrDefault()?.Product?.Name;
            }
        }

        public ObservableCollection<ProductBoughtForAMonthDTO> GetQuantityBoughtThisMonth(DateTime fromDate, DateTime toDate)
        {
            using (var db = new DataBaseContext())
            {
                var query = from p in db.Products
                            join rq in db.RestockRequests on p.Id equals rq.ProductId
                            join rs in db.RestockStages on rq.ProductId equals rs.RequestId
                            where rs.Date >= fromDate && rs.Date <= toDate
                            select new
                            {
                                Product = p,
                                RestockRequest = rq,
                                RestockStage = rs
                            } into joined
                            group joined by new { joined.RestockStage.Date.Month } into grouping
                            select new ProductBoughtForAMonthDTO
                            {
                                Month = grouping.FirstOrDefault().RestockStage.Date,
                                QuantityBought = grouping.FirstOrDefault().Product.Quantity
                            };

                return new ObservableCollection<ProductBoughtForAMonthDTO>(query);
            }
        }

        public ObservableCollection<ProductBoughtForAMonthDTO> GetExpensesThisMonth(DateTime fromDate, DateTime toDate)
        {
            using (var db = new DataBaseContext())
            {
                var query = from p in db.Products
                            join rq in db.RestockRequests on p.Id equals rq.ProductId
                            join rs in db.RestockStages on rq.ProductId equals rs.RequestId
                            where rs.Date >= fromDate && rs.Date <= toDate
                            select new
                            {
                                Product = p,
                                RestockRequest = rq,
                                RestockStage = rs
                            } into joined
                            group joined by new { joined.RestockStage.Date.Month } into grouping
                            select new ProductBoughtForAMonthDTO
                            {
                                Month = grouping.FirstOrDefault().RestockStage.Date,
                                Cost = grouping.Sum(s => s.Product.BuyCost)
                            };

                return new ObservableCollection<ProductBoughtForAMonthDTO>(query);
            }
        }

        public ObservableCollection<ProductSoldForAMonthDTO> GetQuantitySoldThisMonth(DateTime fromDate, DateTime toDate)
        {
            using (var db = new DataBaseContext())
            {
                var query = from p in db.Products
                            join rq in db.RestockRequests on p.Id equals rq.ProductId
                            join rs in db.RestockStages on rq.ProductId equals rs.RequestId
                            where rs.Date >= fromDate && rs.Date <= toDate
                            select new
                            {
                                Product = p,
                                RestockRequest = rq,
                                RestockStage = rs
                            } into joined
                            group joined by new { joined.RestockStage.Date.Month } into grouping
                            select new ProductSoldForAMonthDTO
                            {
                                Month = grouping.FirstOrDefault().RestockStage.Date,
                                QuantitySold = grouping.FirstOrDefault().Product.Quantity
                            };

                return new ObservableCollection<ProductSoldForAMonthDTO>(query);
            }
        }

        public ObservableCollection<ProductSoldForAMonthDTO> GetProfitThisMonth(DateTime fromDate, DateTime toDate)
        {
            using (var db = new DataBaseContext())
            {
                var query = from p in db.Products
                            join rq in db.RestockRequests on p.Id equals rq.ProductId
                            join rs in db.RestockStages on rq.ProductId equals rs.RequestId
                            join ps in db.ProductSales on p.Id equals ps.ProductId
                            where rs.Date >= fromDate && rs.Date <= toDate
                            select new
                            {
                                Product = p,
                                RestockRequest = rq,
                                RestockStage = rs,
                                SoldProduct = ps
                            } into joined
                            group joined by new { joined.RestockStage.Date.Month } into grouping
                            select new ProductSoldForAMonthDTO
                            {
                                Month = grouping.FirstOrDefault().RestockStage.Date,
                                Profit = grouping.Sum(p => p.Product.SellPrice * p.SoldProduct.Quantity)
                            };

                return new ObservableCollection<ProductSoldForAMonthDTO>(query);
            }
        }
        public ObservableCollection<StockStatisticsPriceDTO> GetIncomeThisMonth(DateTime startDate, DateTime endDate, Department department)
        {
            using (var db = new DataBaseContext())
            {
                var query = from d in db.Departments
                            join p in db.Products on d.Id equals p.DepartmentId
                            join ps in db.ProductSales on p.Id equals ps.ProductId
                            where ps.Timestamp >= startDate && ps.Timestamp <= endDate && p.DepartmentId == department.Id
                            select new
                            {
                                SoldProduct = ps
                            } into joined
                            group joined by joined.SoldProduct.Product.Id into newGrouping
                            select new StockStatisticsPriceDTO
                            {
                                item = newGrouping.FirstOrDefault().SoldProduct.Product.Name,
                                price = newGrouping.Sum(p => (p.SoldProduct.Product.SellPrice - p.SoldProduct.Product.BuyCost) * p.SoldProduct.Quantity)
                            };

                return new ObservableCollection<StockStatisticsPriceDTO>(query);
            }
        }

        public ObservableCollection<StockStatisticsAmountDTO> GetAmountSoldThisMonth(DateTime startDate, DateTime endDate, Department department)
        {
            using (var db = new DataBaseContext())
            {
                var query = from d in db.Departments
                            join p in db.Products on d.Id equals p.DepartmentId
                            join ps in db.ProductSales on p.Id equals ps.ProductId
                            where ps.Timestamp >= startDate && ps.Timestamp <= endDate && p.DepartmentId == department.Id
                            select new
                            {
                                SoldProduct = ps
                            } into joined
                            group joined by joined.SoldProduct.Product.Id into newGrouping
                            select new StockStatisticsAmountDTO
                            {
                                item = newGrouping.FirstOrDefault().SoldProduct.Product.Name,
                                amount = newGrouping.Sum(p => p.SoldProduct.Quantity)
                            };
                return new ObservableCollection<StockStatisticsAmountDTO>(query);
            }
        }

        public ObservableCollection<StockStatisticsDTO> StockStatistics(DateTime startDate, DateTime endDate, Department department)
        {

            using (var db = new DataBaseContext())
            {
                var query = from d in db.Departments
                            join p in db.Products on d.Id equals p.DepartmentId
                            join ps in db.ProductSales on p.Id equals ps.ProductId
                            where ps.Timestamp >= startDate && ps.Timestamp <= endDate && p.DepartmentId == department.Id
                            select new
                            {
                                SoldProduct = ps
                            } into joined
                            group joined by joined.SoldProduct.Product.Id into newGrouping
                            select new StockStatisticsDTO
                            {
                                Item = newGrouping.FirstOrDefault().SoldProduct.Product.Name,
                                Amount = newGrouping.Sum(p => p.SoldProduct.Product.Quantity),
                                BuyPrice = newGrouping.FirstOrDefault().SoldProduct.Product.BuyCost,
                                Income = newGrouping.Sum(p => (p.SoldProduct.Quantity * p.SoldProduct.Product.BuyCost)),
                                AmountSold = newGrouping.Sum(p => p.SoldProduct.Quantity),
                                Profit = newGrouping.Sum(p => (p.SoldProduct.Product.SellPrice - p.SoldProduct.Product.BuyCost) * p.SoldProduct.Quantity),
                            };

                return new ObservableCollection<StockStatisticsDTO>(query);
            }
        }
        #endregion

        #region Department 

        public ObservableCollection<DepartmentDTO> AllDepartmentStatistics() {

            using (var db = new DataBaseContext())
            {
                var query = from d in db.Departments
                            join p in db.Products on d.Id equals p.DepartmentId
                            join u in db.Users on d.Id equals u.DepartmentId
                            select new
                            {
                                Department = d,
                                User = u,
                                Product = p,
                            } into joined
                            group joined by joined.Department into newGrouping
                            select new DepartmentDTO
                            {
                                Department = newGrouping.Key.Name,

                            };


                return new ObservableCollection<DepartmentDTO>(query);

            }
        
        }

        public ObservableCollection<StatisticksPerDepartmentDTO> StatisticsPerDepartment(DateTime fromDate, DateTime toDate, Department department) {
            using (var db = new DataBaseContext())
            {
                var query = from d in db.Departments
                            join u in db.Users on d.Id equals u.DepartmentId
                            join us in db.UserScheduledShifts on u.Id equals us.UserId
                            join ss in db.ScheduledShifts on us.ScheduledShiftId equals ss.Id
                            where ss.Date >= fromDate && ss.Date < toDate && u.DepartmentId == department.Id
                            select new
                            {
                                U = u,
                                SS = ss,
                                US = us
                            } into joined
                            group joined by joined.U into newGrouping
                            select new StatisticksPerDepartmentDTO
                            {
                                Name = newGrouping.Key.FirstName + " " + newGrouping.Key.LastName,
                                HoursWorked = newGrouping.Sum(y => y.SS.Duration),
                                SumWage = newGrouping.Sum(x => x.US.HourlyWage * x.SS.Duration),

                            };

                return new ObservableCollection<StatisticksPerDepartmentDTO>(query);
            }
        }
/*        public ObservableCollection<ResourcesPerEmployeeDTO> GetResourcesPerEmployeeDate(DateTime fromDate, DateTime toDate)
        {
            using (var db = new DataBaseContext())
            {
                var result = from ss in db.ScheduledShifts
                             join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                             where ss.Date >= fromDate && ss.Date < toDate
                             select new { SS = ss, US = us } into joined
                             group joined by new
                             {
                                 user = joined.US.User,
                             } into g
                             select new ResourcesPerEmployeeDTO
                             {
                                 EmployeeName = g.Key.user.FirstName + " " + g.Key.user.LastName,
                                 NumberOfScheduledShifts = g.Count(),
                                 HoursWorked = g.Sum(y => y.SS.Duration),
                                 Cost = g.Sum(x => x.US.HourlyWage)
                             };

                return new ObservableCollection<ResourcesPerEmployeeDTO>(result);
            }
        }*/

        #endregion



    }
}
