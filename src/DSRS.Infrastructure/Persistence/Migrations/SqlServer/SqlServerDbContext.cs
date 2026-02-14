using DSRS.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSRS.Infrastructure.Persistence.Migrations.SqlServer;

public sealed class SqlServerDbContext(DbContextOptions<SqlServerDbContext> options,
     IDateTime dateTimeService) : AppDbContext(options, dateTimeService)
{
}
