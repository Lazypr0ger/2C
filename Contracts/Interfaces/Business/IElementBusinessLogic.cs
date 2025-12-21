using Contracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Interfaces.Business
{
    public interface IElementBusinessLogic
    {
        List<ElementDto> GetAll();
        ElementDto GetById(int id);
        void Create(ElementDto elementDto);
        void Update(ElementDto elementDto);
        void Delete(int id);
    }
}
