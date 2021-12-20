using Fly.Domain.Entities;
using Fly.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Domain.Aggreagates
{
    public interface IProductRepository : IRepository<Product>
    {
    }
}
