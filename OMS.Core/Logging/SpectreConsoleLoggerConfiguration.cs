using System;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace OMS.Core.Logging;


public class SpectreConsoleLoggerConfiguration
{
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
    public int EventId { get; set; } = 0;
    public Action<IAnsiConsole>? ConsoleConfiguration { get; set; }
    public AnsiConsoleSettings? ConsoleSettings {get;set;} = null;
    public bool IncludePrefix {get;set;} = true;
    public bool IncludeEventId {get;set;} = false;
}
