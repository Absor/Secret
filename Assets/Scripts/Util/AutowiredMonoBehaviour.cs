using UnityEngine;
using System.Collections;
using System.Reflection;

public class AutowiredMonoBehaviour : MonoBehaviour
{
    protected virtual void Awake()
    {
        FieldInfo[] fieldInfos = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            if (fieldInfo.GetCustomAttributes(typeof(Autowired), false).Length > 0) {
                object found = FindObjectOfType(fieldInfo.FieldType);
                if (found != null)
                {
                    fieldInfo.SetValue(this, found);
                }
                else
                {
                    Debug.LogError("Did not find " + fieldInfo.FieldType + " for " + GetType().Name + ".", this);
                }
            }
        }
    }
}
