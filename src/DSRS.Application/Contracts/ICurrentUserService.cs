using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Contracts;

public interface ICurrentUserService
{
    Guid Id { get; }
}
