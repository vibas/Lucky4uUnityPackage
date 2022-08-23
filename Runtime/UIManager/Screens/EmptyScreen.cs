namespace UIFramework
{
    public class EmptyScreen : UIScreen
    {
        public override void OnSetup()
        {
            // Run one-time setup operations here.
        }

        public override void OnPush(Data data)
        {
            // Be sure to call PushFinished to signal the end of the push.
            PushFinished();
        }

        public override void OnPop()
        {
            // Be sure to call PopFinished to signal the end of the pop.
            PopFinished();
        }

        public override void OnFocus()
        {
        }

        public override void OnFocusLost()
        {
        }
    }
}