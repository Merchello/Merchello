using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    internal static class SettingsMappingExtensions
    {

        #region CountryDisplay

        internal static CountryDisplay ToCountryDisplay(this ICountry country)
        {
            AutoMapper.Mapper.CreateMap<ICountry, CountryDisplay>();
            AutoMapper.Mapper.CreateMap<IProvince, ProvinceDisplay>();

            return AutoMapper.Mapper.Map<CountryDisplay>(country);
        }

        #endregion
    }
}
