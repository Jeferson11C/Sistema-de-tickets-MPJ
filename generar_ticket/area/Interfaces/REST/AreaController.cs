using generar_ticket.area.Domain.Model.Aggregates;
using generar_ticket.area.Interfaces.REST.Resources;
using generar_ticket.area.Interfaces.REST.Transform;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace generar_ticket.area.Interfaces.REST
{
    [ApiController]
    [Route("api/[controller]")]
    public class AreaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AreaResourcesFromEntityAssembler _areaResourcesAssembler;
        private readonly CreateAreaCommandFromResourcesAssembler _createAreaAssembler;

        public AreaController(
            AppDbContext context,
            AreaResourcesFromEntityAssembler areaResourcesAssembler,
            CreateAreaCommandFromResourcesAssembler createAreaAssembler)
        {
            _context = context;
            _areaResourcesAssembler = areaResourcesAssembler;
            _createAreaAssembler = createAreaAssembler;
        }

        [HttpGet("{id}")]
        public ActionResult<AreaResources> GetArea(int id)
        {
            // Retrieve the Area entity from the database
            Area area = _context.Areas.Find(id);
            if (area == null)
            {
                return NotFound();
            }

            var resource = _areaResourcesAssembler.ToResource(area);
            return Ok(resource);
        }

        [HttpGet]
        public ActionResult<IEnumerable<AreaResources>> GetAllAreas()
        {
            // Retrieve all Area entities from the database
            var areas = _context.Areas.ToList();
            var resources = areas.Select(a => _areaResourcesAssembler.ToResource(a)).ToList();
            return Ok(resources);
        }

        [HttpPost]
        public ActionResult<AreaResources> CreateArea([FromBody] CreatedAreaResources resource)
        {
            var area = _createAreaAssembler.ToEntity(resource);

            // Save the Area entity to the database
            _context.Areas.Add(area);
            _context.SaveChanges();

            var createdResource = _areaResourcesAssembler.ToResource(area);
            return CreatedAtAction(nameof(GetArea), new { id = createdResource.Id }, createdResource);
        }
    }
}