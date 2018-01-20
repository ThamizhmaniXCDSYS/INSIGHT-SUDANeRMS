using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using INSIGHT.Entities;

namespace INSIGHT.WCFServices
{
    [ServiceContract()]
    public interface IUserServiceSC
    {
        [OperationContract]
        Dictionary<long, IList<UserAppRole>> GetAppRoleForAnUserListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria);
        [OperationContract]
        long CreateOrUpdateApplication(Application application);
        [OperationContract]
        Dictionary<long, IList<Application>> GetApplicationListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria);
        [OperationContract]
        long CreateOrUpdateRole(Role role);
        [OperationContract]
        Dictionary<long, IList<Role>> GetRoleListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria);
        [OperationContract]
        long CreateOrUpdateBranch(Branch branch);
        [OperationContract]
        Dictionary<long, IList<Branch>> GetBranchListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria);
        [OperationContract]
        void CreateOrUpdateUser(User User);
        [OperationContract]
        User GetUserByUserId(string userId);
        [OperationContract]
        bool ChangePassword(string userId, string oldPassword, string newPassword);
        [OperationContract]
        Dictionary<long, IList<UserAppRole>> GetRoleUsersListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria);
        [OperationContract]
        Dictionary<long, IList<UserAppRole>> GetAppRoleForAnUserListWithPagingAndCriteriaWithLikeSearch(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria);
    }
}
