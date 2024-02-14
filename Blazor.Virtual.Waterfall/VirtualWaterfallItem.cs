namespace Blazor.Virtual.Waterfall;

public class VirtualWaterfallItem<T>
{
    public float Left { get; set; }

    public float Top { get; set; }

    public float Height { get; set; }

    public float Width { get; set; }

    public float Spacing { get; set; }

    public T Data { get; set; }

    internal Style Style =>
        Style.Create()
            .Add("width", this.Width + "px")
            .Add("height", this.Height + "px")
            .Add("transform", $"translate({this.Left}px, {this.Top}px)");
}
