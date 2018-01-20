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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MenuService" in code, svc and config file together.
    public class MenuService : IMenuServiceSC
    {
        public Menu GetMenuItemsById(long Id)
        {
            try
            {
                MenuBC MenuBC = new MenuBC();
                return MenuBC.GetMenuItemsById(Id);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }

        public IList<Menu> GetMenusById(long[] Id)
        {
            try
            {
                MenuBC MenuBC = new MenuBC();
                return MenuBC.GetMenusById(Id);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }

        public Dictionary<long, IList<Menu>> GetMenuListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                MenuBC MenuBC = new MenuBC();
                return MenuBC.GetMenuListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
