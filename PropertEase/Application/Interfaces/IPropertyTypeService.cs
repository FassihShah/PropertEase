using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPropertyTypeService
    {
        Task<List<PropertyType>> GetAllAsync();
        Task<PropertyType> GetByIdAsync(int typeId);
    }
}
