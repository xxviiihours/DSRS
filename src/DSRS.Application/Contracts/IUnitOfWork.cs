using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Contracts;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
