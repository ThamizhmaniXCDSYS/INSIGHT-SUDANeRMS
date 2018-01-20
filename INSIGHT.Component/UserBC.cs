using System;
using System.Collections.Generic;
using PersistenceFactory;
using INSIGHT.Entities;
using CustomAuthentication;
using System.Collections;
using System.Reflection;

namespace INSIGHT.Component
{
    public class UserBC
    {
        PersistenceServiceFactory PSF = null;
        public UserBC()
        {
            List<String> Assembly = new List<String>();
            Assembly.Add("INSIGHT.Entities");
            Assembly.Add("INSIGHT.Entities.TicketingSystem");
            PSF = new PersistenceServiceFactory(Assembly);
        }
        public Dictionary<long, IList<UserAppRole>> GetAppRoleForAnUserListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<UserAppRole>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<UserAppRole>> GetRoleUsersListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<UserAppRole>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool DeleteUserAppRole(long id)
        {
            try
            {
                UserAppRole UserAppRole = PSF.Get<UserAppRole>(id);
                PSF.Delete<UserAppRole>(UserAppRole);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool DeleteUserAppRole(long[] id)
        {
            try
            {

                IList<UserAppRole> UserAppRole = PSF.GetListByIds<UserAppRole>(id);
                PSF.DeleteAll<UserAppRole>(UserAppRole);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long CreateOrUpdateUserAppRole(UserAppRole userapprole)
        {
            try
            {
                if (userapprole != null)
                    PSF.SaveOrUpdate<UserAppRole>(userapprole);
                else { throw new Exception("userapprole is required and it cannot be null.."); }
                return userapprole.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<Application>> GetApplicationListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<Application>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long CreateOrUpdateApplication(Application app)
        {
            try
            {
                if (app != null)
                    PSF.SaveOrUpdate<Application>(app);
                else { throw new Exception("Application is required and it cannot be null.."); }
                return app.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<Role>> GetRoleListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<Role>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long CreateOrUpdateRole(Role role)
        {
            try
            {
                if (role != null)
                    PSF.SaveOrUpdate<Role>(role);
                else { throw new Exception("Application is required and it cannot be null.."); }
                return role.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<Branch>> GetBranchListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<Branch>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long CreateOrUpdateBranch(Branch branch)
        {
            try
            {
                if (branch != null)
                    PSF.SaveOrUpdate<Branch>(branch);
                else { throw new Exception("Application is required and it cannot be null.."); }
                return branch.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void CreateOrUpdateUser(User User)
        {
            try
            {
                if (User != null)
                {

                    //check whether the user exists already or not
                    User Userexists = PSF.Get<User>("UserId", User.UserId.Trim());

                    if (Userexists == null)
                    {
                        PSF.Save<User>(User);
                    }
                    else if (Userexists != null && Userexists.Id == User.Id)
                    {
                        PSF.SaveOrUpdate<User>(User);
                    }
                    else
                    {
                        throw new Exception("User already exists for " + User.UserId.Trim() + ".");
                    }

                }
                else { PSF.Save<User>(User); }
                return;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ModifyUser(User User)
        {
            try
            {
                if (User != null)
                {
                    //check whether the user exists already or not
                    User Userexists = PSF.Get<User>("UserId", User.UserId.Trim());

                    if (Userexists == null)
                    {
                        PSF.Save<User>(User);
                    }
                    //else if (Userexists != null && Userexists.Id == User.Id)
                    //{
                    //    PSF.SaveOrUpdate<User>(User);
                    //}
                    else if (Userexists != null)
                    {
                        Userexists.ModifiedDate = DateTime.Now;
                        Userexists.CreatedDate = Userexists.CreatedDate == null ? DateTime.Now : Userexists.CreatedDate;
                        if(!string.IsNullOrEmpty(User.EmailId))
                        Userexists.EmailId = User.EmailId;
                        // if(User.IsActive)
                        if (User.IsActive == true || User.IsActive == false) 
                        Userexists.IsActive = User.IsActive;
                        if (!string.IsNullOrEmpty(User.UserName))
                            Userexists.UserName = User.UserName;
                        if (!string.IsNullOrEmpty(User.UserType))
                            Userexists.UserType = User.UserType;

                        PSF.SaveOrUpdate<User>(Userexists);
                    }
                    else
                    {
                        throw new Exception("User already exists for " + User.UserId.Trim() + ".");
                    }
                }
                else { PSF.Save<User>(User); }
                return;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public User GetUserByUserId(string userId)
        {
            try
            {
                return PSF.Get<User>("UserId", userId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public User GetUserByEmailId(string emailId)
        {
            try
            {
                return PSF.Get<User>("EmailId", emailId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool ChangePassword(string userId, string oldPassword, string newPassword)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(oldPassword) && !string.IsNullOrWhiteSpace(newPassword))
                {

                    //Get the user 
                    User Userexists = PSF.Get<User>("UserId", userId.Trim());
                    if (Userexists != null)
                    {
                        PassworAuth PA = new PassworAuth();
                        //encode and save the password
                        Userexists.Password = PA.base64Encode(newPassword);
                        PSF.Update<User>(Userexists);
                    }
                    else throw new Exception("No User found for " + userId.Trim() + "");
                }
                else { throw new Exception("User/oldPassword/newPassword is required and it cannot be null.."); }
                return true;
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
                return PSF.GetListWithSearchCriteriaCount<UserAppRole>(page, pageSize, sortby, sortType, criteria);
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
                return PSF.GetListWithEQSearchCriteriaCount<User>(page, pageSize, sortType, sortby, criteria);
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
                return PSF.GetListWithExactSearchCriteriaCount<User>(page, pageSize, sortType, sortby, criteria);
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
                if (s != null)
                {
                    PSF.Save<Session>(s);

                    //check whether the user exists already or not
                    //Session Userexists = PSF.Get<Session>("UserId", s.UserId.Trim());

                    //if (Userexists == null)
                    //{
                    //    PSF.Save<Session>(s);
                    //}
                    //else if (Userexists != null && Userexists.Id == s.Id)
                    //{

                    //    PSF.SaveOrUpdate<Session>(s);
                    //}

                    //else
                    //{
                    //    throw new Exception("User already exists for " + s.UserId.Trim() + ".");
                    //}

                }
                else { PSF.Save<Session>(s); }
                return;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateSession(Session s)
        {
            try
            {
                if (s != null)
                {
                    PSF.SaveOrUpdate(s);
                }
                else
                {
                    return;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, IList<Session>> GetSessionListWithPaging(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria)
        {
            try
            {
                Dictionary<long, IList<Session>> retValue = new Dictionary<long, IList<Session>>();
                return PSF.GetListWithEQSearchCriteriaCount<Session>(page, pageSize, sortType, sortBy, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<UserAppRole>> GetPerformerListWithCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<UserAppRole>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }





        

    }
}
