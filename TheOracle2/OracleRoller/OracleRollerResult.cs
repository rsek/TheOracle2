﻿using TheOracle2.DataClassesNext;

namespace TheOracle2;

public class OracleRollerResult
{
    public OracleRollerResult()
    {
        ChildResults = new List<OracleRollerResult>();
        FollowUpTables = new List<Oracle>();
    }

    public ITableResult TableResult { get; private set; }
    public int? Roll { get; private set; }

    public OracleRollerResult SetRollResult(int? roll, ITableResult chanceTable)
    {
        TableResult = chanceTable;
        Roll = roll;

        return this;
    }

    public List<OracleRollerResult> ChildResults { get; set; }
    public List<Oracle> FollowUpTables { get; internal set; }
}

public interface IRollResult
{
    int Chance { get; set; }
    string Description { get; set; }
    string Image { get; set; }
    public Oracle Oracle { get; set; }
}
