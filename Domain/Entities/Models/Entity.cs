using System.Collections;
using Entities.LinkModels;
using System.Dynamic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Entities.Models;

public class Entity : DynamicObject, IXmlSerializable, IDictionary<string, object>
{
    private const string Root = "Entity";
    private readonly IDictionary<string, object> _expando;

    public Entity()
    {
        _expando = new ExpandoObject()!;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (_expando.TryGetValue(binder.Name, out var value))
        {
            result = value;

            return true;
        }

        return base.TryGetMember(binder, out result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        if (value == null)
        {
            return false;
        }

        _expando[binder.Name] = value;

        return true;
    }

    public XmlSchema GetSchema()
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        reader.ReadStartElement(Root);

        while (!reader.Name.Equals(Root))
        {
            var name = reader.Name;

            reader.MoveToAttribute("type");
            var typeContent = reader.ReadContentAsString();
            var underlyingType = Type.GetType(typeContent);

            reader.MoveToContent();
            _expando[name] = reader.ReadElementContentAs(underlyingType!, null!);
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        foreach (var key in _expando.Keys)
        {
            var value = _expando[key];

            WriteLinksToXml(key, value, writer);
        }
    }

    private static void WriteLinksToXml(string key, object value, XmlWriter writer)
    {
        writer.WriteStartElement(key);

        if (value.GetType() == typeof(List<Link>))
        {
            foreach (var val in (List<Link>) value)
            {
                writer.WriteStartElement(nameof(Link));

                if (val.Href != null)
                {
                    WriteLinksToXml(nameof(val.Href), val.Href, writer);
                }

                if (val.Method != null)
                {
                    WriteLinksToXml(nameof(val.Method), val.Method, writer);
                }

                if (val.Rel != null)
                {
                    WriteLinksToXml(nameof(val.Rel), val.Rel, writer);
                }

                writer.WriteEndElement();
            }
        }
        else
        {
            writer.WriteString(value.ToString());
        }

        writer.WriteEndElement();
    }

    public void Add(string key, object value)
    {
        _expando.Add(key, value);
    }

    public bool ContainsKey(string key)
    {
        return _expando.ContainsKey(key);
    }

    public ICollection<string> Keys => _expando.Keys;

    public bool Remove(string key)
    {
        return _expando.Remove(key);
    }

    public bool TryGetValue(string key, out object value)
    {
        return _expando.TryGetValue(key, out value!);
    }

    public ICollection<object> Values => _expando.Values;

    public object this[string key]
    {
        get => _expando[key];
        set => _expando[key] = value;
    }

    public void Add(KeyValuePair<string, object> item)
    {
        _expando.Add(item);
    }

    public void Clear()
    {
        _expando.Clear();
    }

    public bool Contains(KeyValuePair<string, object> item)
    {
        return _expando.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        _expando.CopyTo(array, arrayIndex);
    }

    public int Count => _expando.Count;

    public bool IsReadOnly => _expando.IsReadOnly;

    public bool Remove(KeyValuePair<string, object> item)
    {
        return _expando.Remove(item);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return _expando.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
