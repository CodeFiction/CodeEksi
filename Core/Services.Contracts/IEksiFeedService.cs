using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Contracts.Models;

namespace Services.Contracts
{
    public interface IEksiFeedService
    {
        Task<IList<TitleModel>> GetDebeList();
        Task<IList<TitleModel>> GetPopulerList();
    }
}