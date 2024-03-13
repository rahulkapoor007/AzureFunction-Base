using AzureFunction.Application.Base.Interfaces;
using AzureFunction.Application.Base;
using AzureFunction.Application.Features.Users.GetAllUsers;
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

namespace AzureFunction.Application.Features.Users.CreateUsers
{
    public class CreateUsersHandler
    {
        public static List<User> users = new List<User>();
        public async Task<IResult<SuccessResponse>> Handle(CreateUsersRequest request, CancellationToken token = default)
        {
            try
            {
                users = Enumerable.Range(1, 100).Select(num => new User () { Id = num, Name = new Faker().Person.FullName }).ToList();

                //var categories = await _crustRepository.GetCrusts(LanguageId, token);

                return Result<SuccessResponse>.Success(
                   new SuccessResponse() { Success = true },
                   SystemMessageSettingNumbers.Success
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
