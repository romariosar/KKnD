using OpenRA.Mods.Kknd.FileFormats;
using OpenRA.Mods.Kknd.FileSystem;
using OpenRA.Primitives;

namespace OpenRA.Mods.Kknd.Widgets.Widgets.Widgets
{
    public class ButtonWidget : ContainerWidget
    {
        public ButtonWidget()
        {
            var fileSystem = Game.ModData.DefaultFileSystem;
            var mobd = new Mobd((SegmentStream) fileSystem.Open("kknd2_sprites|B_Gui.mobd"), Version.KKND2);
            
            var buttonNormal = mobd.staticAnimations[3].Frames[0];
            var buttonHover = mobd.staticAnimations[4].Frames[0];
            var buttonActive = mobd.staticAnimations[5].Frames[0];
        }
    }
}