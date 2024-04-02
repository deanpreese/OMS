namespace OMS.SharedKernel.Common;

public class ScreenColorBase
{
    public string NORMAL      = Console.IsOutputRedirected ? "" : "\x1b[39m";
    public string RED         = Console.IsOutputRedirected ? "" : "\x1b[91m";
    public string GREEN       = Console.IsOutputRedirected ? "" : "\x1b[92m";
    public string YELLOW      = Console.IsOutputRedirected ? "" : "\x1b[93m";
    public string MAGENTA     = Console.IsOutputRedirected ? "" : "\x1b[95m";
    public string CYAN        = Console.IsOutputRedirected ? "" : "\x1b[96m";
    public string GREY        = Console.IsOutputRedirected ? "" : "\x1b[97m";

}
