namespace WorkoutTracker.MAUI.Components
{
    public class FieldSetItemStylesModule : IStylesModule
    {
        public void Configure(SharpCssConfigurator configurator)
        {
            configurator.RegisterStyles<FieldSetItemStyles>(new
            {
                InputColumn = new StyleSet
                {
                    Display = "flex",
                    JustifyContent = "flex-start"
                },
                Row = new StyleSet
                {
                    PaddingBottom = 10
                }
            });
        }
    }
}
