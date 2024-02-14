namespace Blazor.Virtual.Waterfall;

using System.Collections.Generic;

public class RenderData<TItem>
{
    public Style SpacerBeofreStyle { get; set; }

    public Style SpacerAfterStyle { get; set; }

    public Style HeighterStyle { get; set; }

    public List<VirtualWaterfallItem<TItem>> RenderItems { get; set; }
}
