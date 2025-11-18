using ElectricVehicleDealer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Intergations.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerateQuotePdf(Quote quote);
    }
}
