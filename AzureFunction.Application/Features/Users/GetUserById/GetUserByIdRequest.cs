using AzureFunction.Application.Base.Interfaces;
using AzureFunction.Domain.Entities.Response;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunction.Application.Features.Users.GetUserById
{
    public class GetUserByIdRequest : IReturn<IResult<User?>>
    {
        public int Id { get; set; }
    }
}
