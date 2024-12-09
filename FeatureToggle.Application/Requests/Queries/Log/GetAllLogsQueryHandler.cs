using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureToggle.Application.DTOs;
using FeatureToggle.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;


using CsvHelper;
using System.IO;
using System.Globalization;
using System;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace FeatureToggle.Application.Requests.Queries.Log
{
    public class GetAllLogsQueryHandler(FeatureManagementContext featureManagementContext) : IRequestHandler<GetAllLogsQuery, FileContentResult>
    {
        public async Task<FileContentResult> Handle(GetAllLogsQuery request, CancellationToken cancellationToken)
        {
            List<LogDTO> query = await featureManagementContext.Logs.Include(u => u.User)
               .Select(x => new LogDTO
               {
                   LogId = x.Id,
                   UserName = x.User.UserName,
                   FeatureId = x.FeatureId,
                   FeatureName = x.FeatureName,
                   BusinessId = x.BusinessId,
                   BusinessName = x.BusinessName,
                   Time = x.Time,
                   Action = x.Action,

               })
               .OrderByDescending(x => x.Time)
            .ToListAsync(cancellationToken);

            
            using (MemoryStream memoryStream = new())
            {
                using (StreamWriter streamWriter = new(memoryStream))
                using (CsvWriter csvWriter = new(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(query);
                    streamWriter.Flush();
                }

                FileContentResult result = new(memoryStream.ToArray(), "text/csv");
                return result;
            }


        }

    }
}
