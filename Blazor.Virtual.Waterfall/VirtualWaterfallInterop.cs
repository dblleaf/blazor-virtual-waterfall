namespace Blazor.Virtual.Waterfall;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

internal class VirtualWaterfallInterop : IAsyncDisposable
{
    private const string JsFunctionsPrefix = "VirtualWaterfall";

    private readonly IJSRuntime jsRuntime;

    private readonly IVirtualWaterfallJsCallbacks owner;

    private readonly DotNetObjectReference<VirtualWaterfallInterop> dotNetObject;

    public VirtualWaterfallInterop(IVirtualWaterfallJsCallbacks owner, IJSRuntime jsRuntime)
    {
        this.owner = owner;
        this.jsRuntime = jsRuntime;
        this.dotNetObject = DotNetObjectReference.Create(this);
    }

    public async Task InitializeAsync(ElementReference spacerBefore, ElementReference spacerAfter)
    {
        await this.jsRuntime.InvokeVoidAsync($"{JsFunctionsPrefix}.init", this.dotNetObject, spacerBefore, spacerAfter);
    }

    public async Task ScrollTopAsync()
    {
        await this.ScrollToAsync(0);
    }

    public async Task ScrollToAsync(float top)
    {
        await this.jsRuntime.InvokeVoidAsync($"{JsFunctionsPrefix}.scrollTo", this.dotNetObject, top);
    }

    public async ValueTask DisposeAsync()
    {
        await this.jsRuntime.InvokeVoidAsync($"{JsFunctionsPrefix}.dispose", this.dotNetObject);
    }

    [JSInvokable]
    public void OnContentWidthChange(float contentWidth, bool firstCallback = false)
    {
        this.owner.OnContentWidthChange(contentWidth, firstCallback);
    }

    [JSInvokable]
    public void OnSpacerBeforeVisible(float scrollTop, float scrollHeight, float clientHeight)
    {
        this.owner.OnSpacerBeforeVisible(scrollTop, scrollHeight, clientHeight);
    }

    [JSInvokable]
    public void OnSpacerAfterVisible(float scrollTop, float scrollHeight, float clientHeight)
    {
        this.owner.OnSpacerAfterVisible(scrollTop, scrollHeight, clientHeight);
    }
}
