using System.Drawing;

namespace OpenRA.Mods.Kknd.Widgets.Widgets.Widgets
{
    public class ContainerWidget : Widget
    {
        public Rectangle ContentBounds;

        public IContainerStyle ContainerStyle;
        public ILayout Layout;

        public override void Draw()
        {
            if (ContainerStyle != null)
                ContainerStyle.Draw();

            if (Layout != null)
                Layout.Draw();
        }

        public override void Resize(Rectangle renderArea)
        {
            base.Resize(renderArea);

            if (ContainerStyle != null)
            {
                ContainerStyle.Resize(Bounds);
                ContentBounds = ContainerStyle.GetContentBounds(Bounds);
            }
            else
                ContentBounds = new Rectangle(Bounds.Location, Bounds.Size);
            
            if (Layout != null)
                Layout.AlignChildren(ContentBounds);
        }
    }
}