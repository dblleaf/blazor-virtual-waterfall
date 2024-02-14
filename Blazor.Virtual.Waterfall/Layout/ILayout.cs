namespace Blazor.Virtual.Waterfall;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ILayout<TItem>
{
    bool OnContentWidthChange(float width);

    void UpdateItems(IEnumerable<TItem> itemsSource);

    VirtualWaterfallItem<TItem> ToVirtualWaterfallItem(TItem item);

    void AddVirtualWaterfallItem(TItem item);

    void Render(
        float scrollTop,
        float scrollHeight,
        ValueTask? loadMoreTask = null);

    void ReLayout();

    float Spacing { get; set; }

    Style SpacerBeforeStyle { get; }

    Style SpacerAfterStyle { get; }

    Style HeighterStyle { get; }

    List<VirtualWaterfallItem<TItem>> RenderItems { get; }
}
