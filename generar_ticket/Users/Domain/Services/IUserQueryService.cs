using System.Collections.Generic;
using System.Threading.Tasks;
using generar_ticket.Users.Domain.Model.Aggregate;
using generar_ticket.Users.Domain.Model.Queries;


namespace generar_ticket.Users.Domain.Services
{
    public interface IUserQueryService
    {
        Task<IEnumerable<User>> Handle(GetAllUsersQuery query);
        Task<User> Handle(GetUserByIdQuery query);
        Task<IEnumerable<User>> Handle(GetUsersByRoleQuery query);
        Task<IEnumerable<User>> Handle(GetUsersByAreaQuery query);

    }
}