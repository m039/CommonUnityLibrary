using System;
using System.Collections.Generic;

namespace m039.Common {
    public static class Log {
        public enum LogLevel {
            Info, Warning, Error
        }

        public delegate string DescribeDelegate<T>(T sender);

        public delegate void PrintDelegate(LogLevel ll, string message);

        public static ILogger Get(object sender) {
            return Logger.Get(sender);
        }

        public static ILogger Get<T>() {
            return Get(typeof(T));
        }

        public static ILogger Get(Type sender) {
            return Logger.Get(sender);
        }

        public static ILogger Get(string sender) {
            return Logger.Get(sender);
        }

        public static void Info(object sender, string message) {
            Call(Get(sender), LogLevel.Info, message);
       }

        public static void Info<T>(string message) {
            Call(Get(typeof(T)), LogLevel.Info, message);
        }

        public static void Info(Type sender, string message) {
            Call(Get(sender), LogLevel.Info, message);
        }

        public static void Info(string sender, string message) {
            Call(Get(sender), LogLevel.Info, message);
        }

        public static void Error(object sender, string message) {
            Call(Get(sender), LogLevel.Error, message);
        }

        public static void Error<T>(string message) {
            Call(Get(typeof(T)), LogLevel.Error, message);
        }

        public static void Error(Type sender, string message) {
            Call(Get(sender), LogLevel.Error, message);
        }

        public static void Error(string sender, string message) {
            Call(Get(sender), LogLevel.Error, message);
        }

        public static void Warning(object sender, string message) {
            Call(Get(sender), LogLevel.Warning, message);
        }

        public static void Warning<T>(string message) {
            Call(Get(typeof(T)), LogLevel.Warning, message);
        }

        public static void Warning(Type sender, string message) {
            Call(Get(sender), LogLevel.Warning, message);
        }

        public static void Warning(string sender, string message) {
            Call(Get(sender), LogLevel.Warning, message);
        }

        static void Call(ILogger logger, LogLevel level, string message) {
            using (logger) {
                switch (level) {
                    case LogLevel.Info:
                        logger.Info(message);
                        break;
                    case LogLevel.Error:
                        logger.Error(message);
                        break;
                    case LogLevel.Warning:
                        logger.Warning(message);
                        break;
                }
            }
        }
        
        public static void RegisterSenderDescriber<T>(DescribeDelegate<T> describer) {
            Logger.RegisterSenderDescriber(describer);
        }

        public interface ILogger: IDisposable
        {
            void Info(string message);

            void Error(string message);

            void Warning(string message);

            ILogger SetPrinter(PrintDelegate printer);

            ILogger SetEnabled(bool enabled);
        }

        class Logger : ILogger {
            delegate string DescribeDelegate(object sender);

            static Dictionary<Type, DescribeDelegate> s_SenderDescribersByType;

            static readonly List<Logger> s_Pool = new();

            object _senderObject;

            Type _senderType;

            string _senderName;

            PrintDelegate _printer;

            bool _enabled;

            private Logger() {
            }

            public static Logger Get(object sender) {
                var logger = GetInternal();
                logger._senderObject = sender;
                return logger;
            }

            public static Logger Get(Type sender) {
                var logger = GetInternal();
                logger._senderType = sender;
                return logger;
            }

            public static Logger Get(string sender) {
                var logger = GetInternal();
                logger._senderName = sender;
                return logger;
            }

            static Logger GetInternal() {
                Logger logger;

                if (s_Pool.Count > 0) {
                    logger = s_Pool[s_Pool.Count - 1];
                    s_Pool.RemoveAt(s_Pool.Count - 1);
                } else {
                    logger = new Logger();
                }

                logger.Init();
                return logger;
            }

            static void ReleaseInternal(Logger logger) {
                if (!s_Pool.Contains(logger)) {
                    s_Pool.Add(logger);
                }
            }

            void Init() {
                _senderObject = null;
                _senderType = null;
                _senderName = null;
                _enabled = true;
                _printer = DefaultPrinter;
            }

            public void Dispose() {
                ReleaseInternal(this);
            }

            public void Info(string message) {
                Log(message, LogLevel.Info);
            }

            public void Error(string message) {
                Log(message, LogLevel.Error);
            }

            public void Warning(string message) {
                Log(message, LogLevel.Warning);
            }

            public ILogger SetPrinter(PrintDelegate printer) {
                _printer = printer;
                return this;
            }

            public ILogger SetEnabled(bool enabled)
            {
                _enabled = enabled;
                return this;
            }

            void Log(string message, LogLevel level) {
                if (!_enabled)
                    return;

                if (_senderObject != null) {
                    DescribeSender(_senderObject, ref message);
                    _senderType = _senderObject.GetType();
                }

                if (_senderType != null) {
                    _senderName = _senderType.Name;
                }

                _printer?.Invoke(
                    level, 
                    string.IsNullOrEmpty(_senderName)? message : $"[{_senderName}] {message}"
                );
            }

            static void DefaultPrinter(LogLevel level, string message) {
                Action<string> command = null;

                switch (level) {
                    case LogLevel.Info:
                        command = UnityEngine.Debug.Log;
                        break;
                    case LogLevel.Error:
                        command = UnityEngine.Debug.LogError;
                        break;
                    case LogLevel.Warning:
                        command = UnityEngine.Debug.LogWarning;
                        break;
                }

                command?.Invoke(message);
            }

            static void DescribeSender(object sender, ref string message)
            {
                CheckDescribers();

                var senderType = sender.GetType();
                foreach (var entry in s_SenderDescribersByType)
                {
                    if (senderType.IsSubclassOf(entry.Key))
                    {
                        message += entry.Value.Invoke(sender);
                        return;
                    }
                }
            }

            static void CheckDescribers()
            {
                if (s_SenderDescribersByType != null)
                    return;

                s_SenderDescribersByType = new Dictionary<Type, DescribeDelegate>();

                RegisterSenderDescriber<UnityEngine.MonoBehaviour>(sender =>
                {
                    var path = sender.gameObject.scene.path;
                    if (!string.IsNullOrEmpty(path))
                        path += "/";

                    return $"\nat {path}{sender.transform.GetPath()}";
                });

                RegisterSenderDescriber<UnityEngine.ScriptableObject>(sender => $"\nat {sender.name}");
            }

            public static void RegisterSenderDescriber<T>(DescribeDelegate<T> describer)
            {
                s_SenderDescribersByType.Add(typeof(T), sender => describer.Invoke((T)sender));
            }
        }
    }
}
