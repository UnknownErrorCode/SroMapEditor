using OpenTK.Windowing.Desktop;

namespace SimpleGridFly
{
    internal class IGameWindow : GameWindow
    {
        public IGameWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }
    }
}