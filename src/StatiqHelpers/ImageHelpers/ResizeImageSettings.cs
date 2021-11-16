using System.ComponentModel;
using Spectre.Console.Cli;
using Statiq.App;

namespace StatiqHelpers.ImageHelpers
{
    public class ResizeImageSettings : EngineCommandSettings
    {
        [CommandArgument(0, "<width>")]
        [Description("Specify the target width")]
        public int Width { get; set; }

        [CommandArgument(0, "<height>")]
        [Description("Specify the target height")]
        public int Height { get; set; }

        [CommandOption("-c")]
        [Description("Compress all files, not just uncommitted files")]
        public bool AllFiles { get; set; }
    }
}
