using AutoMapper;
using OrderPaymentSystem.Domain.Dto.Report;
using OrderPaymentSystem.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Mapping
{
    public class ReportMapping: Profile
    {
        public ReportMapping()
        {
            CreateMap<Report, ReportDto>().ReverseMap();
        }
    }
}
