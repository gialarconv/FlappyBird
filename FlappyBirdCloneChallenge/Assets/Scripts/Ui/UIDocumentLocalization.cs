/// src = https://gist.github.com/andrew-raphael-lukasik/72a4d3d14dd547a1d61ae9dc4c4513da
///
/// Copyright (C) 2022 Andrzej Rafał Łukasik (also known as: Andrew Raphael Lukasik)

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

[DisallowMultipleComponent]
[RequireComponent(typeof(UIDocument))]
public class UIDocumentLocalization : MonoBehaviour
{
    /// <summary> Executed after hierarchy is cloned fresh and translated. </summary>
    public event System.Action OnCompleted = () => { };

    [SerializeField] private LocalizedStringTable _table = null;
    private UIDocument _uiDocument;

    void OnEnable()
    {
        if (_uiDocument == null)
            _uiDocument = GetComponent<UIDocument>();
        _table.TableChanged += OnTableChanged;
    }

    void OnDisable()
    {
        _table.TableChanged -= OnTableChanged;
    }


    void OnTableChanged(StringTable table)
    {
        _uiDocument.rootVisualElement.Clear();
        _uiDocument.visualTreeAsset.CloneTree(_uiDocument.rootVisualElement);

        var op = _table.GetTableAsync();
        if (op.IsDone)
        {
            OnTableLoaded(op);
        }
        else
        {
            op.Completed -= OnTableLoaded;
            op.Completed += OnTableLoaded;
        }
    }

    void OnTableLoaded(AsyncOperationHandle<StringTable> op)
    {
        StringTable table = op.Result;
        LocalizeChildrenRecursively(_uiDocument.rootVisualElement, table);
        _uiDocument.rootVisualElement.MarkDirtyRepaint();
        OnCompleted();
    }

    void LocalizeChildrenRecursively(VisualElement element, StringTable table)
    {
        VisualElement.Hierarchy elementHierarchy = element.hierarchy;
        int numChildren = elementHierarchy.childCount;
        for (int i = 0; i < numChildren; i++)
        {
            VisualElement child = elementHierarchy.ElementAt(i);
            Localize(child, table);
        }
        for (int i = 0; i < numChildren; i++)
        {
            VisualElement child = elementHierarchy.ElementAt(i);
            VisualElement.Hierarchy childHierarchy = child.hierarchy;
            int numGrandChildren = childHierarchy.childCount;
            if (numGrandChildren != 0)
                LocalizeChildrenRecursively(child, table);
        }
    }

    void Localize(VisualElement next, StringTable table)
    {
        if (typeof(TextElement).IsInstanceOfType(next))
        {
            TextElement textElement = (TextElement)next;
            string key = textElement.text;
            if (!string.IsNullOrEmpty(key) && key[0] == '#')
            {
                key = key.TrimStart('#');
                StringTableEntry entry = table[key];
                if (entry != null)
                    textElement.text = entry.LocalizedValue;
                else
                    Debug.LogWarning($"No {table.LocaleIdentifier.Code} translation for key: '{key}'");
            }
        }
    }
}
