using System.Reflection;
using UnityEngine;

namespace m039.Common
{
    public class CommonMonoBehaviour : MonoBehaviour
    {
        protected virtual void OnValidate()
        {
            CheckFields();
        }

        void CheckFields()
        {
            var type = GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.IsNotSerialized)
                    continue;

                if (!field.IsPublic && field.GetCustomAttribute<SerializeField>() == null)
                    continue;

                if (field.GetCustomAttribute<NotRequiredFieldAttribute>() != null)
                    continue;

                if (field.GetCustomAttribute<InjectAttribute>() != null)
                    continue;

                var value = field.GetValue(this);
                if (value == null || value.Equals(null))
                {
                    var hierarchy = transform.GetPath();
                    var message =
                        $"Field <b>{field.Name}</b> not set in component <b>{type.Name}</b> with hierarchy <b>{hierarchy}</b>";

                    Log.Error(this, message);
                }
            }
        }
    }
}
