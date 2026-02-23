using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Players.Get;

public record GetPlayerByNameCommand(string Name) : ICommand<Result<PlayerDto>> { }
