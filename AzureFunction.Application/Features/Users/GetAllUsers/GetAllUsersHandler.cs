using AzureFunction.Application.Base.Interfaces;
using AzureFunction.Application.Base;
using AzureFunction.Domain.Entities.Constants;
using AzureFunction.Domain.Entities.Exceptions;
using Bogus;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureFunction.Domain.Entities.Response;
using AzureFunction.Application.Features.Users.CreateUsers;

namespace AzureFunction.Application.Features.Users.GetAllUsers
{
    public class GetAllUsersHandler
    {
        
        public async Task<IResult<List<User>>> Handle(GetAllUsersRequest request, CancellationToken token = default)
        {
            try
            {
                if(CreateUsersHandler.users is not null && CreateUsersHandler.users.Count() > 0)
                {
                    List<User> users = CreateUsersHandler.users;

                    if (request is not null && !string.IsNullOrWhiteSpace(request.filter))
                        return Result<List<User>>.Success(
                                users.Where(p => string.IsNullOrWhiteSpace(request.filter) || p.Name.Contains(request.filter)).ToList(),
                                    SystemMessageSettingNumbers.Success
                                    );
                  

                    return Result<List<User>>.Success(
                                CreateUsersHandler.users,
                                    SystemMessageSettingNumbers.Success
                                    );
                }
               
                return Result<List<User>>.Success(
                                new(),
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
