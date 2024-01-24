﻿namespace ServerSide.GridUtilities.Grid;
public class ResultsDto<T> where T : class
{
    public virtual IReadOnlyList<T> Results { get; set; } = null!;

    public virtual int TotalCount { get; set; }
}
