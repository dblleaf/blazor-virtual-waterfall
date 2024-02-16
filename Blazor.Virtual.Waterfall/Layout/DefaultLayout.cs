namespace Blazor.Virtual.Waterfall;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DefaultLayout<TItem> : ILayout<TItem>
{
    private List<float> columnsTop = new List<float>();
    private float columnWidth;
    private float contentWidth;
    private int columnCount;
    private float spacerBeforeHeight;
    private float spacerAfterTop;
    private float height;

    public float Spacing { get; set; }

    public float ItemMinWidth { get; set; } = 200;

    public int MinColumnCount { get; set; } = 1;

    public Func<TItem, float, float> HeightCalulater { get; set; }

    public Style SpacerBeforeStyle
        => Style.Create()
            .Add("top", "0")
            .Add("height", $"{this.spacerBeforeHeight}px");

    public Style SpacerAfterStyle
        => Style.Create()
            .Add("top", $"{this.spacerAfterTop}px")
            .Add("bottom", "0");

    public Style HeighterStyle
        => Style.Create()
            .Add("height", $"{this.height}px");

    public List<VirtualWaterfallItem<TItem>> RenderItems { get; private set; } = [];

    private List<VirtualWaterfallItem<TItem>> Items { get; } = [];

    public bool OnContentWidthChange(float width)
    {
        this.contentWidth = width;
        this.columnCount = this.CalColumnCount();
        this.columnWidth = this.GetColumnWidth();
        this.ReLayout();
        return true;
    }

    public VirtualWaterfallItem<TItem> ToVirtualWaterfallItem(TItem item)
    {
        var colomnIdex = this.GetColumnIndex();
        var virtualWaterfallItem = new VirtualWaterfallItem<TItem>
        {
            Data = item,
            Height = this.HeightCalulater(item, this.columnWidth),
            Width = this.columnWidth,
            Left = colomnIdex * (this.columnWidth + this.Spacing),
            Top = this.columnsTop[colomnIdex],
            Spacing = this.Spacing,
        };

        this.columnsTop[colomnIdex] = virtualWaterfallItem.Top + virtualWaterfallItem.Height + this.Spacing;
        return virtualWaterfallItem;
    }

    public void ReLayout()
    {
        this.columnsTop = Enumerable.Range(0, this.columnCount).Select(_ => 0f).ToList();
    }

    public void Render(
        float scrollTop,
        float clientHeight,
        ValueTask? loadMoreTask = null)
    {
        if (!(this.Items?.Count > 0))
        {
            return;
        }

        var startIndex = 0;
        var endIndex = this.Items.Count;
        var min = this.Items.Where(o => o.Top < scrollTop - clientHeight).LastOrDefault();
        var max = this.Items.Where(o => o.Top > scrollTop + clientHeight * 2).FirstOrDefault();
        if (min != null)
        {
            startIndex = this.Items.IndexOf(min);
        }

        if (max != null)
        {
            endIndex = this.Items.IndexOf(max);
        }

        if (endIndex >= this.Items.Count - 5)
        {
            if (loadMoreTask != null)
            {
                var task = loadMoreTask.Value;
                if (task.IsCompleted)
                {
                    this.height = this.columnsTop.Max();
                    this.Render(scrollTop, clientHeight, loadMoreTask);
                }
            }
        }

        this.RenderItems = this.Items
            .Skip(startIndex)
        .Take(endIndex - startIndex)
            .ToList();
        this.spacerBeforeHeight = this.RenderItems.FirstOrDefault()?.Top ?? 0;
        this.spacerAfterTop = this.RenderItems.LastOrDefault()?.Top + this.RenderItems.LastOrDefault()?.Height + this.Spacing ?? 0;
    }

    public void UpdateItems(IEnumerable<TItem> itemsSource)
    {
        this.Items.Clear();
        this.columnsTop = Enumerable.Range(0, this.columnCount).Select(_ => 0f).ToList();
        if (itemsSource != null)
        {
            foreach (var item in itemsSource)
            {
                this.AddVirtualWaterfallItem(item);
            }

            this.height = this.columnsTop.Max();
        }
    }

    public void AddVirtualWaterfallItem(TItem item)
    {
        var virtualWaterfallItem = this.ToVirtualWaterfallItem(item);
        this.Items.Add(virtualWaterfallItem);
    }

    private int GetColumnIndex()
    {
        return this.columnsTop.IndexOf(this.columnsTop.Min());
    }

    private float GetColumnWidth()
    {
        var spacing = (this.columnCount + 1) * this.Spacing;
        return (float)Math.Floor((this.contentWidth - spacing) / this.columnCount);
    }

    private int CalColumnCount()
    {
        var cWidth = this.contentWidth - this.Spacing * 2;
        if (cWidth > this.ItemMinWidth * 2)
        {
            var count = Convert.ToInt32(Math.Floor(cWidth / this.ItemMinWidth));

            return count;
        }

        return this.MinColumnCount;
    }
}
