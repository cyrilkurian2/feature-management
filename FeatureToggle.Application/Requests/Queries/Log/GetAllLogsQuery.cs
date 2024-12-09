using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureToggle.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FeatureToggle.Application.Requests.Queries.Log
{
    public class GetAllLogsQuery : IRequest<FileContentResult>
    {

    }
}
