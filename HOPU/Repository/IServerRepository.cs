using System.Collections.Generic;
using System.Linq;


namespace HOPU
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetUnifiedTestList();
    }
}
