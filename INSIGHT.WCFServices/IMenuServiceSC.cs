using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using INSIGHT.Entities;

namespace INSIGHT.WCFServices
{
    [ServiceContract()]
    public interface IMenuServiceSC
    {
        Menu GetMenuItemsById(long Id);
        IList<Menu> GetMenusById(long[] Id);
        Dictionary<long, IList<Menu>> GetMenuListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria);
    }
}
