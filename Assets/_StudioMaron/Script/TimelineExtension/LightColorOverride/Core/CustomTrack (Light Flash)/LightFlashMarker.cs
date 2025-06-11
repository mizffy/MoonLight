using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.ComponentModel;

namespace StudioMaron
{
    [DisplayName("Light Flash Marker")]
    public class LightFlashMarker : Marker, INotification, INotificationOptionProvider
    {
        public enum MarkerAction
        {
            Flash,
        }
        public MarkerAction action = MarkerAction.Flash;
        [Min(0.1f)] public float flashIntensity = 4;
        [Min(0.1f)] public float flashTime = 0.5f;
        public PropertyName id { get; }
        public NotificationFlags flags => 0;
    }
}
