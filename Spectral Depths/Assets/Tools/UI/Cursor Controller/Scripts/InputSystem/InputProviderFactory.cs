namespace SlimUI.CursorControllerPro.InputSystem {
    // Construct an input provider (users can modify this to return a different input provider)
    class InputProviderFactory {
        public static IInputProvider GetInputProvider() {
            return new DefaultInputProvider();
        }
    }
}