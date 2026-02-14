using System;

namespace DSRS.Domain.Common;

public interface IAuditableEntity
{
    public DateTime CreatedAt { get; }
    public DateTime LastModified { get; }
    void SetCreated(DateTime now);
    void SetModified(DateTime now);
}
