using Microsoft.Extensions.Configuration;
using Sodao.Core.Data;
using Sodao.GrpcExample.Domain.Contracts;
using Sodao.GrpcExample.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Sodao.GrpcExample.Domain.Repositories
{
    public class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public UserEntity DoSomething()
        {
            var testModel = new TestValue();

            var abc = new int[] { 3, 4, 5 };
            var count = GetList(1, 10, oo => abc.Contains(oo.UserId));

            return null;

            #region 增
            var model = new UserEntity()
            {
                UserName = "abc",
                RealName = "abc",
                Password = "123456",
            };
            var addResult = Add(model, true);
            #endregion

            #region 查
            var getResult = Get(oo => oo.UserId == model.UserId, oo => new { oo.UserId, oo.UserName });

            var countResult = Count(oo => oo.UserName.Contains("test"));
            #endregion

            #region 改
            var setResult = Set(() => new { RealName = "abc" }, oo => oo.UserId == model.UserId);
            #endregion

            #region 删
            var delResult = Delete(oo => oo.UserId == model.UserId);
            #endregion

            return null;
        }
    }

    public class TestValue
    {
        public List<int> Ids { get; set; } = new List<int>() { 3, 4, 5 };
    }
}
