﻿using UnityEngine;
using System.Collections.Generic;
using Handler = System.Action<object, object>;
using SenderTable = System.Collections.Generic.Dictionary<object, System.Collections.Generic.List<System.Action<object, object>>>;

public class NotificationCenter {
    #region Properties
    /// <summary>
    /// The dictionary "key" (string) represents a notificationName property to be observed
    /// The dictionary "value" (SenderTable) maps between sender and observer sub tables
    /// </summary>
    private Dictionary<string, SenderTable> _table = new Dictionary<string, SenderTable>();
    private HashSet<List<Handler>> _invoking = new HashSet<List<Handler>>();
    #endregion

    #region Singleton Pattern
    public readonly static NotificationCenter instance = new NotificationCenter();
    private NotificationCenter() { }
    #endregion

    #region Public
    public void AddObserver(Handler handler, string notificationName) {
        AddObserver(handler, notificationName, null);
    }

    public void AddObserver(Handler handler, string notificationName, object sender) {
        if(handler == null) {
            Debug.LogError("Can't add a null event handler for notification, " + notificationName);
            return;
        }

        if(string.IsNullOrEmpty(notificationName)) {
            Debug.LogError("Can't observe an unnamed notification");
            return;
        }

        if(!_table.ContainsKey(notificationName))
            _table.Add(notificationName, new SenderTable());

        SenderTable subTable = _table[notificationName];

        object key = (sender != null) ? sender : this;

        if(!subTable.ContainsKey(key))
            subTable.Add(key, new List<Handler>());

        List<Handler> list = subTable[key];
        if(!list.Contains(handler)) {
            if(_invoking.Contains(list))
                subTable[key] = list = new List<Handler>(list);

            list.Add(handler);
        }
    }

    public void RemoveObserver(Handler handler, string notificationName) {
        RemoveObserver(handler, notificationName, null);
    }

    public void RemoveObserver(Handler handler, string notificationName, object sender) {
        if(handler == null) {
            Debug.LogError("Can't remove a null event handler for notification, " + notificationName);
            return;
        }

        if(string.IsNullOrEmpty(notificationName)) {
            Debug.LogError("A notification name is required to stop observation");
            return;
        }

        // No need to take action if we dont monitor this notification
        if(!_table.ContainsKey(notificationName))
            return;

        SenderTable subTable = _table[notificationName];
        object key = (sender != null) ? sender : this;

        if(!subTable.ContainsKey(key))
            return;

        List<Handler> list = subTable[key];
        int index = list.IndexOf(handler);
        if(index != -1) {
            if(_invoking.Contains(list))
                subTable[key] = list = new List<Handler>(list);
            list.RemoveAt(index);
        }
    }

    public void Clean() {
        string[] notKeys = new string[_table.Keys.Count];
        _table.Keys.CopyTo(notKeys, 0);

        for(int i = notKeys.Length - 1; i >= 0; --i) {
            string notificationName = notKeys[i];
            SenderTable senderTable = _table[notificationName];

            object[] senKeys = new object[senderTable.Keys.Count];
            senderTable.Keys.CopyTo(senKeys, 0);

            for(int j = senKeys.Length - 1; j >= 0; --j) {
                object sender = senKeys[j];
                List<Handler> handlers = senderTable[sender];
                if(handlers.Count == 0)
                    senderTable.Remove(sender);
            }

            if(senderTable.Count == 0)
                _table.Remove(notificationName);
        }
    }

    public void PostNotification(string notificationName) {
        PostNotification(notificationName, null);
    }

    public void PostNotification(string notificationName, object sender) {
        PostNotification(notificationName, sender, null);
    }

    public void PostNotification(string notificationName, object sender, object e) {
        if(string.IsNullOrEmpty(notificationName)) {
            Debug.LogError("A notification name is required");
            return;
        }

        // No need to take action if we dont monitor this notification
        if(!_table.ContainsKey(notificationName))
            return;

        // Post to subscribers who specified a sender to observe
        SenderTable subTable = _table[notificationName];
        if(sender != null && subTable.ContainsKey(sender)) {
            List<Handler> handlers = subTable[sender];
            _invoking.Add(handlers);
            for(int i = 0; i < handlers.Count; ++i)
                handlers[i](sender, e);
            _invoking.Remove(handlers);
        }

        // Post to subscribers who did not specify a sender to observe
        if(subTable.ContainsKey(this)) {
            List<Handler> handlers = subTable[this];
            _invoking.Add(handlers);
            for(int i = 0; i < handlers.Count; ++i)
                handlers[i](sender, e);
            _invoking.Remove(handlers);
        }
    }
    #endregion
}

public static class NotificationExtensions {
    public static void PostNotification(this object obj, string notificationName) {
        NotificationCenter.instance.PostNotification(notificationName, obj);
    }

    public static void PostNotification(this object obj, string notificationName, object e) {
        NotificationCenter.instance.PostNotification(notificationName, obj, e);
    }

    public static void AddObserver(this object obj, Handler handler, string notificationName) {
        NotificationCenter.instance.AddObserver(handler, notificationName);
    }

    public static void AddObserver(this object obj, Handler handler, string notificationName, object sender) {
        NotificationCenter.instance.AddObserver(handler, notificationName, sender);
    }

    public static void RemoveObserver(this object obj, Handler handler, string notificationName) {
        NotificationCenter.instance.RemoveObserver(handler, notificationName);
    }

    public static void RemoveObserver(this object obj, Handler handler, string notificationName, object sender) {
        NotificationCenter.instance.RemoveObserver(handler, notificationName, sender);
    }
}