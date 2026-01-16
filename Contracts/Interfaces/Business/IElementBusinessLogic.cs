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
        ElementDto GetById(string id);
        decimal CalculateTotalCostElement(int countElement, ProductionDto productionDto);

        void Create(ElementDto elementDto);
        void Update(ElementDto elementDto);
        void Delete(string id);

        void Recovery(string id);
    }
}
