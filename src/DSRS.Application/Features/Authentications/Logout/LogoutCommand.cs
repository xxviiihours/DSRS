using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DSRS.Application.Features.Authentications.Logout;

public record LogoutCommand : ICommand<Result>
{
}
