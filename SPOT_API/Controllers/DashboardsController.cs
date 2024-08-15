using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using CarTradePro.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Models;
using SPOT_API.Persistence;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public DashboardsController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/Dashboards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dashboard>>> GetDashboards()
        {
            return await _context.Dashboards.ToListAsync();
        }

        // GET: api/Dashboards/5
        [HttpGet("Top")]
        public async Task<ActionResult<DashboardDto>> GetDashboardByCurrentTenant()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();


            //var dashboard = await _context.Dashboards
            //    //.Where(c=> c.TenantId == user.TenantId)

            //    .FirstOrDefaultAsync();

            //if (dashboard == null)
            //{
            DashboardDto dashboard = null;
            try
            {

                var lastYearSales = await _context.Stocks
                    .Where(s => s.Sale.SaleDateTime.Year == DateTime.Now.Year - 1)
                    .GroupBy(s => 1)
                    .Select(g => new SalesData
                    {
                        Total = g.Count(),
                        Amount = g.Sum(s => s.Sale.SaleAmount)
                    })
                    .FirstOrDefaultAsync();

                var thisYearSales = await _context.Stocks
                    .Where(s => s.Sale.SaleDateTime.Year == DateTime.Now.Year)
                    .GroupBy(s => 1)
                    .Select(g => new SalesData
                    {
                        Total = g.Count(),
                        Amount = g.Sum(s => s.Sale.SaleAmount)
                    })
                    .FirstOrDefaultAsync();


                var stockDataStatuses = await _context.Stocks
                    .Include(c=> c.StockStatusHistories)
                    .ThenInclude(c=> c.StockStatus)
                    .ToListAsync();

                var stockData = stockDataStatuses.
                    GroupBy(c => c.LatestStockStatus.StockStatus.Name)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                var stockStatus = new StockStatusData
                {
                    Available = stockData.FirstOrDefault(s => s.Status == "Available")?.Count ?? 0,
                    InProgress = stockData.FirstOrDefault(s => s.Status != "Draft" && s.Status != "Available")?.Count ?? 0
                };



                var topSellingModels = await _context.Stocks
                    .Where(s => s.Sale.SaleDateTime.Year == DateTime.Now.Year)
                    .Include(c=> c.Vehicle)
                    .ThenInclude(c=> c.Brand)
                    .GroupBy(s => s.Vehicle.Brand.Name)
                    .OrderByDescending(g => g.Count())
                    .Select(g => new TopSellingModelData
                    {
                        Name = g.Key,
                        UnitsSold = g.Count()
                    })
                    .Take(10) // Get top 5 selling models
                    .ToListAsync();

                var lastYearTopSellingModels = await _context.Stocks
                    .Where(s => s.Sale.SaleDateTime.Year == DateTime.Now.Year - 1)
                    .Include(c => c.Vehicle)
                    .ThenInclude(c => c.Brand)
                    .GroupBy(s => s.Vehicle.Brand.Name)
                    .OrderByDescending(g => g.Count())
                    .Select(g => new TopSellingModelData
                    {
                        Name = g.Key,
                        UnitsSold = g.Count()
                    })
                    .Take(10) // Get top 5 selling models
                    .ToListAsync();

                dashboard = new DashboardDto
                {
                    LastYearSales = lastYearSales,
                    ThisYearSales = thisYearSales,
                    StockStatus = stockStatus,
                    TopSellingModels = topSellingModels,
                    LastYearTopSellingModels = lastYearTopSellingModels
                };



                //var stocks = _context.Stocks
                //    .Include(c => c.StockStatusHistories.OrderByDescending(c => c.DateTime))
                //    .ThenInclude(c => c.StockStatus)
                //    .ToList();

                //int totalStockAvailable = stocks.Where(p => p.LatestStockStatus.StockStatus.Name == "Available").ToList().Count();
                //int totalStockInProcess = stocks.Where(p => 
                //        p.LatestStockStatus.StockStatus.Name != "Draft" &&
                //        p.LatestStockStatus.StockStatus.Name != "Available" &&
                //        p.LatestStockStatus.StockStatus.Name != "Cancelled" &&
                //        p.LatestStockStatus.StockStatus.Name != "Sold"
                //        ).ToList().Count();
                //int totalTotalStockSoldThisYear = stocks.Where(p => 
                //    p.LatestStockStatus.StockStatus.Name == "Sold" && 
                //    p.LatestStockStatus.DateTime.Year == DateTime.Now.Year).ToList().Count();

                //int totalTotalStockSoldLastYear = stocks.Where(p =>
                //    p.LatestStockStatus.StockStatus.Name == "Sold" &&
                //    p.LatestStockStatus.DateTime.Year == DateTime.Now.AddYears(-1).Year).ToList().Count();

                //dashboard = new Dashboard
                //{
                //    Date = DateTime.Now.Date,
                //    ThisYear = DateTime.Now.Year,
                //    LastYear = DateTime.Now.Year - 1,
                //    TotalStockAvailable = totalStockAvailable ,
                //    TotalStockInProcess = totalStockInProcess,
                //    TotalTotalStockSoldLastYear = totalTotalStockSoldLastYear,
                //    TotalTotalStockSoldThisYear = totalTotalStockSoldThisYear,
                //};
            }
            catch (Exception ex)
            {

                throw;
            }
               

                //await _context.Dashboards.AddAsync(dashboard);
                //await _context.SaveChangesAsync();

            //}

            return dashboard;
        }


        // GET: api/Dashboards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Dashboard>> GetDashboard(Guid id)
        {
            var dashboard = await _context.Dashboards.FindAsync(id);

            if (dashboard == null)
            {
                return NotFound();
            }

            return dashboard;
        }

        // PUT: api/Dashboards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDashboard(Guid id, Dashboard dashboard)
        {
            if (id != dashboard.Id)
            {
                return BadRequest();
            }

            _context.Entry(dashboard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DashboardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Dashboards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Dashboard>> PostDashboard(Dashboard dashboard)
        {
            _context.Dashboards.Add(dashboard);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDashboard", new { id = dashboard.Id }, dashboard);
        }

        // DELETE: api/Dashboards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDashboard(Guid id)
        {
            var dashboard = await _context.Dashboards.FindAsync(id);
            if (dashboard == null)
            {
                return NotFound();
            }

            _context.Dashboards.Remove(dashboard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DashboardExists(Guid id)
        {
            return _context.Dashboards.Any(e => e.Id == id);
        }
    }
}
