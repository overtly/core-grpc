using Overt.GrpcExample.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overt.GrpcExample.Application.Constracts
{
    public interface IUserService
    {
        UserEntity DoSomething();

        bool DoSomethingWithTrans();
    }
}
