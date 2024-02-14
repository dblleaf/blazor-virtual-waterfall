namespace Blazor.Virtual.Waterfall;

internal interface IVirtualWaterfallJsCallbacks
{
    void OnContentWidthChange(float contentWidth, bool firstCallback = false);

    void OnSpacerBeforeVisible(float scrollTop, float scrollHeight, float clientHeight);

    void OnSpacerAfterVisible(float scrollTop, float scrollHeight, float clientHeight);
}
