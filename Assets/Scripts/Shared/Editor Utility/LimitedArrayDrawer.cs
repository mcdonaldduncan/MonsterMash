using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LimitedArrayAttribute))]
public class LimitedArrayDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        LimitedArrayAttribute attribute = (LimitedArrayAttribute)base.attribute;
        int maxItems = attribute.maxItems;

        if (property.isArray && property.propertyType == SerializedPropertyType.Generic)
        {
            int itemCount = Mathf.Min(property.arraySize, maxItems);
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PropertyField(position, property, label);

            if (itemCount > maxItems)
            {
                EditorGUI.HelpBox(position, "This array can have a maximum of " + maxItems + " items.", MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "This attribute is only applicable to arrays.");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        LimitedArrayAttribute attribute = (LimitedArrayAttribute)base.attribute;
        int maxItems = attribute.maxItems;

        if (property.isArray)
        {
            int itemCount = Mathf.Min(property.arraySize, maxItems);
            float itemHeight = EditorGUI.GetPropertyHeight(property, label, true);
            float height = itemHeight * itemCount;

            if (itemCount > maxItems)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            return height;
        }
        else
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}

public class LimitedArrayAttribute : PropertyAttribute
{
    public int maxItems;

    public LimitedArrayAttribute(int maxItems)
    {
        this.maxItems = maxItems;
    }
}
