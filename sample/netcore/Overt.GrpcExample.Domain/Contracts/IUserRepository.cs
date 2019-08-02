using Overt.Core.Data;
using Overt.GrpcExample.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overt.GrpcExample.Domain.Contracts
{
    public interface IUserRepository : IBaseRepository<UserEntity>
    {
        /// <summary>
        /// 做什么东西
        /// </summary>
        /// <returns></returns>
        UserEntity DoSomething();
    }
}
