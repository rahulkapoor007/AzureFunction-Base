using AzureFunction.Application.Base.Interfaces;
using AzureFunction.Domain.Entities.Response;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunction.Application.Features.Users.GetAllUsers
{
    public class GetAllUsersRequest : IReturn<IResult<List<User>>>
    {
        public string filter { get; set; }
    }
}
