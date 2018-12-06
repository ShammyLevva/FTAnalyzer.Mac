using AppKit;
using CoreGraphics;

namespace FTAnalyzer.ViewControllers
{
    public interface IPrintView 
    {
        NSView PrintView { get; }
        NSView ScrollView { get; }
        CGRect Bounds { get; }
    }
}
