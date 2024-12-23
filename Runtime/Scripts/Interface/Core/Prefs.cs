using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Trackman
{
#if UNITY_EDITOR
    using UnityPrefs = UnityEditor.EditorPrefs;
#else
    using UnityPrefs = UnityEngine.PlayerPrefs;
#endif
    public abstract class Prefs : IPrefs
    {
        #region Properties
        protected abstract string ScopePrefix { get; }
        public string Prefix { get; set; } = string.Empty;

        ICollection<string> IDictionary<string, object>.Keys => throw new NotImplementedException();
        ICollection<object> IDictionary<string, object>.Values => throw new NotImplementedException();
        int ICollection<KeyValuePair<string, object>>.Count => throw new NotImplementedException();
        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;
        public object this[string key] { get => Read<object>(key, null); set => Write(key, value); }
        #endregion

        #region Methods
        public bool Contains(string path) => PrefsBackend.Current.HasKey(GetFullPath(path));
        public void Write(string path, object value)
        {
            if (value is bool boolValue) PrefsBackend.Current.SetInt(GetFullPath(path), boolValue ? 1 : 0);
            else if (value is float floatValue) PrefsBackend.Current.SetFloat(GetFullPath(path), floatValue);
            else if (value is int intValue) PrefsBackend.Current.SetInt(GetFullPath(path), intValue);
            else if (value is string stringValue) PrefsBackend.Current.SetString(GetFullPath(path), stringValue);
            else PrefsBackend.Current.SetString(GetFullPath(path), DebugUtility.GetString(value));
        }
        public T Read<T>(string path, T defaultValue)
        {
            try
            {
                if (defaultValue is bool boolValue) return (T) (object) (PrefsBackend.Current.GetInt(GetFullPath(path), boolValue ? 1 : 0) == 1);
                if (defaultValue is float floatValue) return (T) (object) PrefsBackend.Current.GetFloat(GetFullPath(path), floatValue);
                if (defaultValue is int intValue) return (T) (object) PrefsBackend.Current.GetInt(GetFullPath(path), intValue);
                if (defaultValue is string stringValue) return (T) (object) PrefsBackend.Current.GetString(GetFullPath(path), stringValue);

                if (defaultValue == null)
                {
                    string stringVal = PrefsBackend.Current.GetString(GetFullPath(path), "");
                    if (stringVal.NullOrEmpty()) return default;
                    if (bool.TryParse(stringVal, out boolValue)) return (T) (object) boolValue;
                    if (int.TryParse(stringVal, out intValue)) return (T) (object) intValue;
                    if (float.TryParse(stringVal, out floatValue)) return (T) (object) floatValue;

                    return (T) (object) stringVal;
                }

                return DebugUtility.FromString<T>(PrefsBackend.Current.GetString(GetFullPath(path), DebugUtility.GetString(defaultValue)));
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            return defaultValue;
        }
        #endregion

        #region Dicitonary Methods
        public void Add(string key, object value) => Write(key, value);
        public bool ContainsKey(string key) => Contains(key);
        public bool Remove(string key)
        {
            bool contains = Contains(key);
            if (contains) PrefsBackend.Current.DeleteKey(GetFullPath(key));
            return contains;
        }
        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            bool contains = Contains(key);
            if (contains) value = this[key];
            else value = default;
            return contains;
        }
        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item) => Add(item.Key, item.Value);
        void ICollection<KeyValuePair<string, object>>.Clear() => throw new NotSupportedException();
        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item) => Contains(item.Key);
        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => throw new NotSupportedException();
        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item) => Remove(item.Key);
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => throw new NotSupportedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
        #endregion

        #region Support Methods
        string GetFullPath(string path) => $"{ScopePrefix}.Prefs.{(Prefix.NotNullOrEmpty() ? Path.Combine(Prefix, path) : path)}";
        #endregion
    }

    public class ProductPrefs : Prefs
    {
        #region Properties
        protected override string ScopePrefix { get; }
        #endregion

        #region Constructors
        public ProductPrefs(Scene scene)
        {
#if UNITY_EDITOR
            if (!scene.IsValid()) throw new ArgumentException();

            UnityEditor.PackageManager.PackageInfo scenePackageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(scene.path);
            ScopePrefix = scenePackageInfo?.displayName.Split('.')[^1] ?? Application.productName.Replace(" ", "");
#else
            ScopePrefix = Application.productName;
#endif
        }
        #endregion
    }

    public class ScenePrefs : Prefs
    {
        #region Properties
        protected override string ScopePrefix { get; }
        #endregion

        #region Constructors
        public ScenePrefs(Scene scene)
        {
#if UNITY_EDITOR
            if (scene.IsValid() && !string.IsNullOrEmpty(scene.path) && UnityEditor.PackageManager.PackageInfo.FindForAssetPath(scene.path) is { } scenePackageInfo)
            {
                ScopePrefix = Path.Combine(scenePackageInfo.displayName.Split('.')[^1], scene.path);
                return;
            }
#endif
            ScopePrefix = scene.IsValid() ? scene.path : Application.productName;
        }
        #endregion
    }

    public class ProjectPrefs : Prefs
    {
        #region Fields
        static readonly Lazy<string> productName = new(() => Application.productName.Replace(" ", ""));
        #endregion

        #region Properties
        protected override string ScopePrefix => productName.Value;
        #endregion
    }

    public class GlobalPrefs : Prefs
    {
        #region Properties
        protected override string ScopePrefix => "Shared";
        #endregion
    }

    /// <summary>
    /// Implements the same methods as <see cref="PlayerPrefs"/>
    /// </summary>
    public abstract class PrefsBackend
    {
        #region Properties
        public static PrefsBackend Current => Custom ?? real;
        public static PrefsBackend Custom { get; set; }
        static PrefsBackend real = new PrefsBackendReal();
        #endregion

        #region Methods
        public abstract bool HasKey(string path);
        public abstract void SetInt(string path, int value);
        public abstract int GetInt(string path, int defaultValue);
        public abstract void SetFloat(string path, float value);
        public abstract float GetFloat(string path, float defaultValue);
        public abstract void SetString(string path, string value);
        public abstract string GetString(string path, string defaultValue);
        public abstract void DeleteKey(string path);
        public abstract void DeleteAll();
        #endregion
    }

    public class PrefsBackendReal : PrefsBackend
    {
        #region Methods
        public override bool HasKey(string path) => UnityPrefs.HasKey(path);
        public override void SetInt(string path, int value) => UnityPrefs.SetInt(path, value);
        public override int GetInt(string path, int defaultValue) => UnityPrefs.GetInt(path, defaultValue);
        public override void SetFloat(string path, float value) => UnityPrefs.SetFloat(path, value);
        public override float GetFloat(string path, float defaultValue) => UnityPrefs.GetFloat(path, defaultValue);
        public override void SetString(string path, string value) => UnityPrefs.SetString(path, value);
        public override string GetString(string path, string defaultValue) => UnityPrefs.GetString(path, defaultValue);
        public override void DeleteKey(string path) => UnityPrefs.DeleteKey(path);
        public override void DeleteAll() => UnityPrefs.DeleteAll();
        #endregion
    }

    public class PrefsBackendInMemory : PrefsBackend
    {
        #region Fields
        readonly Dictionary<string, object> data = new();
        #endregion

        #region Methods
        public override bool HasKey(string path) => data.ContainsKey(path);
        public override void SetInt(string path, int value) => data[path] = value;
        public override int GetInt(string path, int defaultValue) => data.TryGetValue(path, out object value) ? (int) value : defaultValue;
        public override void SetFloat(string path, float value) => data[path] = value;
        public override float GetFloat(string path, float defaultValue) => data.TryGetValue(path, out object value) ? (float) value : defaultValue;
        public override void SetString(string path, string value) => data[path] = value;
        public override string GetString(string path, string defaultValue) => data.TryGetValue(path, out object value) ? (string) value : defaultValue;
        public override void DeleteKey(string path) => data.Remove(path);
        public override void DeleteAll() => data.Clear();
        #endregion
    }
}