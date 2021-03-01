using System;
using System.Collections.Generic;

namespace Assets.PatternDesigner.Scripts.Util
{
    /// <summary>
    ///     Handles different events. Maps a class like Button, Toggle or SteamVR-Input-Action with a given listener function.
    ///     Will check, if the class will be null and don't thrwo an exception.
    /// </summary>
    /// <typeparam name="T">Eventholder like Button</typeparam>
    /// <typeparam name="U">Listener</typeparam>
    public class EventHandler<T, U>
    {
        private readonly Dictionary<T, U> actions = new Dictionary<T, U>();

        private readonly Action<T, U> addListener;
        private readonly Action<T, U> removeListener;

        public EventHandler(Action<T, U> addListener, Action<T, U> removeListener, params KeyValuePair<T, U>[] actions)
        {
            this.addListener = addListener;
            this.removeListener = removeListener;

            foreach (var a in actions)
                if (a.Key != null && a.Value != null)
                    this.actions.Add(a.Key, a.Value);
        }

        public void RegisterActions()
        {
            foreach (var action in actions)
                if (action.Key != null && action.Value != null)
                    addListener(action.Key, action.Value);
        }

        public void UnregisterActions()
        {
            foreach (var action in actions)
                if (action.Key != null && action.Value != null)
                    removeListener(action.Key, action.Value);
        }

        public static KeyValuePair<T, U> newKeyValuePair(T t, U u)
        {
            return new KeyValuePair<T, U>(t, u);
        }
    }
}