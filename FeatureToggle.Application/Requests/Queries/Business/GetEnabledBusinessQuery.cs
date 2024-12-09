﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureToggle.Application.DTOs;
using MediatR;

namespace FeatureToggle.Application.Requests.Queries.Business
{
    public class GetEnabledBusinessQuery : IRequest<List<GetBusinessDTO>>
    {
        public int FeatureId { get; set; }
    }
}
