using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistenceFactory;
using INSIGHT.Entities;


namespace INSIGHT.Component
{
    public class MenuBC
    {
        PersistenceServiceFactory PSF = null;
        public MenuBC()
        {
            List<String> Assembly = new List<String>();
            Assembly.Add("INSIGHT.Entities");
            Assembly.Add("INSIGHT.Entities.TicketingSystem");
            PSF = new PersistenceServiceFactory(Assembly);
        }
        public Menu GetMenuItemsById(long Id)
        {
            try
            {
                if (Id > 0)
                    return PSF.Get<Menu>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<Menu> GetMenusById(long[] Ids)
        {
            try
            {
                if (Ids != null && Ids.Count() > 0)
                    return PSF.GetListByIds<Menu>(Ids);
                else { throw new Exception("Id is required and it cannot be 0"); }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<Menu>> GetMenuListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<Menu>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
