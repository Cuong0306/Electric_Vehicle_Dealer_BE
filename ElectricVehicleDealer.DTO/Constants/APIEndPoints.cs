using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Constants
{
    public static class APIEndPoints
    {
        public const string Base = "api";
        public static class Agreements
        {
            public const string BaseAgreements = $"{Base}/agreements";
            public const string Create = $"{Base}/createAgreements";
            public const string GetAll = $"{BaseAgreements}/getAllAgreements";
            public const string GetById = $"{BaseAgreements}/getByIdAgreements{{id}}";
            public const string Update = $"{BaseAgreements}/updateAgreements";
            public const string Delete = $"{BaseAgreements}/deleteAgreements/{{id}}";
            
        }
    }
}