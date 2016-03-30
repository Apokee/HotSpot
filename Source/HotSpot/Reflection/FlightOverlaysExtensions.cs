using System.Reflection;

namespace HotSpot.Reflection
{
    internal static class FlightOverlaysExtensions
    {
        private static readonly FieldInfo ScreenMessageField;

        static FlightOverlaysExtensions()
        {
            var field = typeof(FlightOverlays).GetField("statusMessage", BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null && field.FieldType == typeof(ScreenMessage))
            {
                ScreenMessageField = field;
            }
            else
            {
                Log.Warning("Could not find FlightOverlays ScreenMessage field");
            }
        }

        public static ScreenMessage TryGetScreenMessage(this FlightOverlays flightOverlays)
        {
            return (ScreenMessage)ScreenMessageField?.GetValue(flightOverlays);
        }
    }
}
