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

            DashboardDto dashboard = null;
            try
            {

                var lastYearSales = await _context.Stocks
                    .Where(s => s.Sale.SaleDateTime.Year == DateTime.Now.Year - 1)
                    .GroupBy(s => 1)
                    .Select(g => new SalesData
                    {
                        Total = g.Count(),
                        Amount = g.Sum(s => (float)s.Sale.SaleAmount)
                    })
                    .FirstOrDefaultAsync();

                var thisYearSales = await _context.Stocks
                    .Where(s => s.Sale.SaleDateTime.Year == DateTime.Now.Year)
                    .Where(s=> s.Vehicle.Brand != null)
                    .Where(s => s.Vehicle.Model != null)
                    .GroupBy(s => 1)
                    .Select(g => new SalesData
                    {
                        Total = g.Count(),
                        Amount = g.Sum(s => (float)s.Sale.SaleAmount)
                    })
                    .FirstOrDefaultAsync();


                var stockDataStatuses = await _context.Stocks
                    .Where(c => c.IsOpen == true)
                    //.Include(c=> c.StockStatusHistories)
                    //.ThenInclude(c=> c.StockStatus)
                    .ToListAsync();

                var arrivaleStateData = stockDataStatuses.
                    GroupBy(c => c.ArrivalState)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                var arrivaleStatus = new ArrivaleStatusData
                {
                    Incoming = arrivaleStateData.Where(s => s.Status == ArrivalState.Incoming ).ToList().Count ,
                    Received = arrivaleStateData.Where(s => s.Status == ArrivalState.Received).ToList().Count
                };



                var topSellingModels = await _context.Stocks
                    .Where(s => s.Sale.SaleDateTime.Year == DateTime.Now.Year)
                    .Where(c=> c.IsSold)
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


                // Add Units Registered by Month
                var unitsRegisteredByMonth = await _context.Stocks
                    .Where(s => s.RegistrationDate.Year == DateTime.Now.Year)
                    .GroupBy(s => s.RegistrationDate.Month)
                    .Select(g => new UnitsByMonth
                    {
                        Month = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                // Add Units Registered by Year
                var unitsRegisteredByYear = await _context.Stocks
                    .GroupBy(s => s.RegistrationDate.Year)
                    .Select(g => new UnitsByYear
                    {
                        Year = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();


                dashboard = new DashboardDto
                {
                    LastYearSales = lastYearSales,
                    ThisYearSales = thisYearSales,
                    ArrivalStatus = arrivaleStatus,
                    TopSellingModels = topSellingModels,
                    LastYearTopSellingModels = lastYearTopSellingModels,

                    UnitsRegisteredByMonth = unitsRegisteredByMonth,
                    UnitsRegisteredByYear = unitsRegisteredByYear,
                };

            }
            catch (Exception ex)
            {

                throw;
            }

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
