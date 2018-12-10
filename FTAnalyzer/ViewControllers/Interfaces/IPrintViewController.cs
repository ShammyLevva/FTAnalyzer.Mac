using AppKit;
using FTAnalyzer.Utilities;

namespace FTAnalyzer
{
    public interface IPrintViewController
    {
        NSView PrintView { get; }
        void PreparePrintView(CustomPrintPanel printPanel); 
    }
}
