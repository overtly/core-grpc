using Sodao.GrpcExample.Application.Constracts;
using Sodao.GrpcExample.Domain.Contracts;
using Sodao.GrpcExample.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sodao.GrpcExample.Application.Services
{
    public class UserService : IUserService
    {
        IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserEntity DoSomething()
        {
            return _userRepository.DoSomething();
        }

        public bool DoSomethingWithTrans()
        {
            using (var transaction = _userRepository.BeginTransaction())
            {
                var entity = new UserEntity()
                {
                    AddTime = DateTime.Now,
                    IsSex = false,
                    UserName = DateTime.Now.ToString("yyyyMMddHHmmss")
                };
                var addRes = _userRepository.Add(entity, true);
                _userRepository.Set(() => new { RealName = $"R_{DateTime.Now.ToString("yyyyMMddHHmmss")}" }, oo => oo.UserId == entity.UserId);
                transaction.Commit();
            }
            return true;
        }
    }
}
