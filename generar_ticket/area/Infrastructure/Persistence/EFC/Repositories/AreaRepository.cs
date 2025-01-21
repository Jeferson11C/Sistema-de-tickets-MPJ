using generar_ticket.area.Domain.Model.Aggregates;
using generar_ticket.area.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace generar_ticket.area.Infrastructure.Persistence.EFC.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private readonly AppDbContext _context;

        public AreaRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Area?> GetAllAreas()
        {
            return _context.Areas.ToList();
        }

        public Area? GetAreaById(int id)
        {
            return _context.Areas.FirstOrDefault(area => area.Id == id);
        }

        public void AddArea(Area? area)
        {
            _context.Areas.Add(area);
            _context.SaveChanges();
        }
    }
}