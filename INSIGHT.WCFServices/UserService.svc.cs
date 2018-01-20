using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using INSIGHT.Component;
using INSIGHT.Entities;

namespace INSIGHT.WCFServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "UserService" in code, svc and config file together.
    public class UserService : IUserServiceSC
    {
        public Dictionary<long, IList<UserAppRole>> GetAppRoleForAnUserListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                UserBC UserBC = new UserBC();
                return UserBC.GetAppRoleForAnUserListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public Dictionary<long, IList<UserAppRole>> GetRoleUsersListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                UserBC UserBC = new UserBC();
                return UserBC.GetRoleUsersListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public long CreateOrUpdateUserAppRole(Entities.UserAppRole approle)
        {
            try
            {
                UserBC AppRoleBC = new UserBC();
                AppRoleBC.CreateOrUpdateUserAppRole(approle);
                return approle.Id;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally { }
        }

        public long CreateOrUpdateApplication(Entities.Application application)
        {
            try
            {
                UserBC ApplicationBC = new UserBC();
                ApplicationBC.CreateOrUpdateApplication(application);
                return application.Id;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally { }
        }

        public Dictionary<long, IList<Entities.Application>> GetApplicationListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                UserBC AppBC = new UserBC();
                return AppBC.GetApplicationListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public long CreateOrUpdateRole(Entities.Role role)
        {
            try
            {
                UserBC RoleBC = new UserBC();
                RoleBC.CreateOrUpdateRole(role);
                return role.Id;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally { }
        }

        public void CreateOrUpdateUser(User User)
        {
            try
            {
                UserBC UserBC = new UserBC();
                UserBC.CreateOrUpdateUser(User);
                return;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally { }
        }

        public void ModifyUser(User User)
        {
            try
            {
                UserBC UserBC = new UserBC();
                UserBC.ModifyUser(User);
                return;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally { }
        }
        public User GetUserByUserId(string userId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    UserBC UserBC = new UserBC();
                    return UserBC.GetUserByUserId(userId);
                }
                else throw new Exception("User Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        public User GetUserByEmailId(string emailId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(emailId))
                {
                    UserBC UserBC = new UserBC();
                    return UserBC.GetUserByEmailId(emailId);
                }
                else throw new Exception("User EmailId is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        public bool ChangePassword(string userId, string oldPassword, string newPassword)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(oldPassword) && !string.IsNullOrWhiteSpace(newPassword))
                {
                    UserBC UserBC = new UserBC();
                    return UserBC.ChangePassword(userId, oldPassword, newPassword);
                }
                else { throw new Exception("User/oldPassword/newPassword is required and it cannot be null.."); }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public Dictionary<long, IList<Entities.Role>> GetRoleListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                UserBC RoleBC = new UserBC();
                return RoleBC.GetRoleListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public long CreateOrUpdateBranch(Entities.Branch branch)
        {
            try
            {
                UserBC BranchBC = new UserBC();
                BranchBC.CreateOrUpdateBranch(branch);
                return branch.Id;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally { }
        }

        public Dictionary<long, IList<Entities.Branch>> GetBranchListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                UserBC BranchBC = new UserBC();
                return BranchBC.GetBranchListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, IList<UserAppRole>> GetAppRoleForAnUserListWithPagingAndCriteriaWithLikeSearch(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                try
                {
                    UserBC UserBC = new UserBC();
                    return UserBC.GetAppRoleForAnUserListWithPagingAndCriteriaWithLikeSearch(page, pageSize, sortby, sortType, criteria);
                }
                catch (Exception)
                {
                    throw;
                }
                finally { }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, IList<User>> GetUserListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                UserBC UserBC = new UserBC();
                return UserBC.GetUserListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<User>> GetUserListWithPagingAndCriteriaLikeSearch(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                UserBC UserBC = new UserBC();
                return UserBC.GetUserListWithPagingAndCriteriaLikeSearch(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void CreateOrUpdateSession(Session s)
        {
            try
            {
                UserBC UserBC = new UserBC();
                UserBC.CreateOrUpdateSession(s);
                return;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally { }
        }
        public void UpdateSession(Session s)
        {
            try
            {
                UserBC UserBC = new UserBC();
                UserBC.UpdateSession(s);
                return;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally { }
        }
        public Dictionary<long, IList<Session>> GetSessionListWithPaging(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria)
        {
            try
            {
                UserBC UserBC = new UserBC();
                return UserBC.GetSessionListWithPaging(page, pageSize, sortType, sortBy, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public Dictionary<long, IList<UserAppRole>> GetPerformerListWithCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                try
                {
                    UserBC UserBC = new UserBC();
                    return UserBC.GetPerformerListWithCriteria(page, pageSize, sortby, sortType, criteria);
                }
                catch (Exception)
                {
                    throw;
                }
                finally { }
            }
            catch (Exception)
            {
                throw;
            }
        }
    
    }
}
