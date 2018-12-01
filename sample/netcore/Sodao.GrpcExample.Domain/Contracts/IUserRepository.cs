using Sodao.Core.Data;
using Sodao.GrpcExample.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sodao.GrpcExample.Domain.Contracts
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
