using Sodao.GrpcExample.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sodao.GrpcExample.Application.Constracts
{
    public interface IUserService
    {
        UserEntity DoSomething();

        bool DoSomethingWithTrans();
    }
}
