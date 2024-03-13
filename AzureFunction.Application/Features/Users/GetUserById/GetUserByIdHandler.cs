using AzureFunction.Application.Base.Interfaces;
using AzureFunction.Application.Base;
using AzureFunction.Application.Features.Users.CreateUsers;
using AzureFunction.Application.Features.Users.GetAllUsers;
using AzureFunction.Domain.Entities.Constants;
using AzureFunction.Domain.Entities.Exceptions;
using AzureFunction.Domain.Entities.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Azure.Core;

namespace AzureFunction.Application.Features.Users.GetUserById
{
    public class GetUserByIdHandler
    {
        public async Task<IResult<User?>> Handle(GetUserByIdRequest request, CancellationToken token = default)
        {
            try
            {
                if (request != null && CreateUsersHandler.users is not null && CreateUsersHandler.users.Count() > 0)
                {
                    var user = CreateUsersHandler.users.FirstOrDefault(p => p.Id == request.Id);
                     
                    if(user != null)
                        return Result<User?>.Success(
                                    user,
                                    SystemMessageSettingNumbers.Success
                                    );
                }

                return Result<User?>.Success(
                                null,
                                SystemMessageSettingNumbers.Success,
                                System.Net.HttpStatusCode.NoContent

                                    );
            }
            catch (Exception ex)
            {
                if (ex is ApplicationErrorException) throw;
                else throw new ApplicationErrorException(ex);
            }
        }
    }
}
