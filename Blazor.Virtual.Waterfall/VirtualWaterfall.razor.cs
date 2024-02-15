namespace Blazor.Virtual.Waterfall;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class VirtualWaterfall<TItem> : IVirtualWaterfallJsCallbacks, IAsyncDisposable
{
    [Parameter]
    public IEnumerable<TItem> ItemsSource { get; set; }

    [Parameter]
    public RenderFragment<TItem> ItemTemplate { get; set; }

    [Parameter]
    public ILayout<TItem> Layout { get; set; }

    [Parameter]
    public RenderFragment PlaceHolder { get; set; }

    [Parameter]
    public Func<Task<IEnumerable<TItem>>> ItemsProvider { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private VirtualWaterfallInterop jsInterop;
    private ElementReference spacerBefore;
    private ElementReference spacerAfter;
    private bool noMore = false;
    private float scrollTop;
    private float scrollHeight;
    private float clientHeight;
    private ValueTask? loadMoreInforTask;

    public void OnContentWidthChange(float contentWidth, bool firstCallback = true)
    {
        if (this.Layout.OnContentWidthChange(contentWidth))
        {
            this.Layout.UpdateItems(this.ItemsSource);
            if (!firstCallback)
            {
                this.Layout.Render(
                    this.scrollTop,
                    this.clientHeight,
                    this.loadMoreInforTask);
                this.StateHasChanged();
            }
        }
    }

    public void OnSpacerBeforeVisible(float scrollTop, float scrollHeight, float clientHeight)
    {
        this.scrollTop = scrollTop;
        this.scrollHeight = scrollHeight;
        this.clientHeight = clientHeight;
        if (!(this.ItemsSource?.Count() > 0))
        {
            return;
        }

        this.Layout.Render(
            this.scrollTop,
            this.clientHeight,
            this.loadMoreInforTask);
        this.StateHasChanged();
    }

    public void Refresh()
    {
        this.noMore = false;
        this.Layout.ReLayout();
    }

    public void OnSpacerAfterVisible(float scrollTop, float scrollHeight, float clientHeight)
    {
        this.scrollTop = scrollTop;
        this.scrollHeight = scrollHeight;
        this.clientHeight = clientHeight;
        if (!(this.ItemsSource?.Count() > 0))
        {
            return;
        }

        this.Layout.Render(
            this.scrollTop,
            this.clientHeight,
            this.loadMoreInforTask);
        this.StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        await this.jsInterop.DisposeAsync();
    }

    protected override void OnParametersSet()
    {
        this.loadMoreInforTask = this.ItemsProvider != null ? this.LoadDataAsync() : null;
        base.OnParametersSet();
    }

    protected override async void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            this.jsInterop = new VirtualWaterfallInterop(this, this.JSRuntime);
            await this.jsInterop.InitializeAsync(this.spacerBefore, this.spacerAfter);
        }

        base.OnAfterRender(firstRender);
    }

    private async ValueTask LoadDataAsync()
    {
        if (this.noMore)
        {
            return;
        }

        var result = await this.ItemsProvider();
        if (result != null)
        {
            foreach (var item in result)
            {
                this.Layout.AddVirtualWaterfallItem(item);
            }
        }
        else
        {
            this.noMore = true;
        }
    }
}
