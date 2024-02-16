# blazor-virtual-waterfall
This is a virtual waterfall component packaged with blazor.

### Quick start.
1. Add the nuget package `Blazor.Virtual.Waterfall` to project.
    ```bash
    dotnet add package Blazor.Virtual.Waterfall --version 1.0.2
    ```
2. Add style and script references.
    ```html
    ...
    <link rel="stylesheet" href="_content/Blazor.Virtual.Waterfall/Blazor.Virtual.Waterfall.css" />
    ...
    <script src="_content/Blazor.Virtual.Waterfall/Blazor.Virtual.Waterfall.js" type="module"></script>
    ...

    ```
3. Sample code:
    ```razor
    <VirtualWaterfall TItem="Item"
                    Layout="layout"
                    ItemsSource="items">
        <ItemTemplate>
            <div style="height:100%;background-color:#@(context.Color);opacity:0.3;">
                <div>@(context.Color)</div>
            </div>
        </ItemTemplate>
    </VirtualWaterfall>
    @code {
        private List<Item> items => Enumerable.Range(0, 5000).Select(o => new Item { Color = this.GetColor(), Height = this.random.Next(200, 600) }).ToList();
        private Random random = new Random();
        private Blazor.Virtual.Waterfall.ILayout<Item> layout = new DefaultLayout<Item>
        {
            Spacing = 8,
            HeightCalulater = (o, _) => o.Height,
        };
        private string GetColor()
        {
            return random.Next(16777215).ToString("X2").PadLeft(6, '0');
        }

        public class Item
        {
            public int Height { get; set; }

            public string Color { get; set; }
        }
    }
    ```