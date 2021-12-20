using Fly.Domain.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Domain.Entities
{
    public class Category : EntityStringKey
    {
        public Category(string name, string description, 
            long version, bool isDeleted = false, DateTime? lastModifiedDate = null, string modifiedBy = "1", string? id = null)
            : base(version, isDeleted, lastModifiedDate, modifiedBy, id)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}
