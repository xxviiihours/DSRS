using DSRS.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Contracts;

public interface ICurrentUserService
{
    PlayerId Id { get; }
}
