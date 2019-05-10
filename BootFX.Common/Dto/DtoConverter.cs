// BootFX - Application framework for .NET applications
// 
// File: DtoConverter.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Reflection;
using BootFX.Common.Data;

namespace BootFX.Common.Dto
{
    public class DtoConverter //: JsonConverter
    {
        public void WriteJson(IJsonNetWriter writer, object value, IJsonNetSerializer serializer)
        {
            var dto = (IDtoBase)value;

            // walk...
            var values = new Dictionary<string, object>();
            foreach (var field in dto.Type.Fields)
            {
                var walk = dto[field];
                if (walk != null && walk.GetType().IsEnum)
                    walk = walk.ToString();

                // set...
                values[field.JsonName] = walk;
            }

            // customer...
            var preloader = DtoCachePreloader.BoundPreloader;
            foreach (var link in dto.Type.Links)
            {
                if (preloader != null && !(dto.IsLoaded(link)))
                {
                    var parentId = dto.GetValue<long>(link.ReferenceField);
                    if (parentId != 0)
                    {
                        // get it...
                        var parent = (IDtoCapable)preloader.GetItem(link.Link.ParentEntityType, parentId);
                        if (parent != null)
                        {
                            var parentDto = parent.ToDto();
                            dto.SetLink(link, parentDto);
                        }
                    }
                }

                // get...
                values[link.JsonName] = dto.GetLink(link);
            }

            // send...
            serializer.Serialize(writer, values);
        }

        public object ReadJson(IJsonNetReader reader, Type objectType, object existingValue, IJsonNetSerializer serializer)
        {
            return this.ReadJsonInternal(reader, objectType, existingValue, serializer, 0);
        }

        private object ReadJsonInternal(IJsonNetReader reader, Type objectType, object existingValue, IJsonNetSerializer serializer, int depth)
        {
            // mbr - 2016-07-07 - if we get null, quit...
            if (reader.TokenType == JsonNetTokenShim.Null)
                return null;

            string readingName = null;

            // walk...
            var dto = (IDtoBase)Activator.CreateInstance(objectType);
            while (true)
            {
                var didPeek = false;

                // what did we get?
                if (reader.TokenType == JsonNetTokenShim.EndObject)
                    break;
                else if (reader.TokenType == JsonNetTokenShim.PropertyName)
                    readingName = (string)reader.Value;
                else if (reader.TokenType == JsonNetTokenShim.String || reader.TokenType == JsonNetTokenShim.Boolean || reader.TokenType == JsonNetTokenShim.Integer ||
                    reader.TokenType == JsonNetTokenShim.Float || reader.TokenType == JsonNetTokenShim.Date || reader.TokenType == JsonNetTokenShim.Null)
                {
                    // field...
                    var field = dto.Type.GetFieldByJsonName(readingName, false);
                    if (field != null)
                    {
                        // set...
                        if (field.CanWrite)
                            dto[field] = reader.Value;
                    }
                    else
                    {
                        // mbr - 2015-06-17 - we might have a property that's not mapped to a field. this will happen if we
                        // use a DtoBase<> as a base type (e.g. "SaveOrderLineDto")...
                        var prop = GetAdhocProperty(dto.GetType(), readingName);
                        if (prop != null && prop.CanWrite)
                        {
                            var toSet = ConversionHelper.ChangeType(reader.Value, prop.PropertyType);
                            prop.SetValue(dto, toSet);
                        }
                    }

                    // reset...
                    readingName = null;
                }
                else if (reader.TokenType == JsonNetTokenShim.StartObject)
                {
                    if (!(string.IsNullOrEmpty(readingName)))
                    {
                        var member = dto.Type.GetMemberByJsonName(readingName, false);
                        if (member != null)
                        {
                            // what is it?
                            if (typeof(IDtoBase).IsAssignableFrom(member.ValueType))
                            {
                                // read something we know...
                                var inner = (IDtoBase)this.ReadJsonInternal(reader, member.ValueType, existingValue, serializer, depth + 1);

                                // set...
                                if (member is DtoLink)
                                    dto.SetLink((DtoLink)member, inner);
                                else
                                    throw new NotSupportedException(string.Format("Cannot handle '{0}'.", member.GetType()));
                            }
                            else
                            {
                                // read it generally...
                                //var inner = serializer.;
                                //var prop = this.GetAdhocProperty(member.ValueType, member.JsonName);
                                //if (prop != null && prop.CanWrite)
                                //    prop.SetValue(dto, inner);
                                throw new NotImplementedException("This operation has not been implemented.");
                            }
                        }
                        else
                        {
                            // drain here...
                            while (true)
                            {
                                if (!(reader.Read()))
                                    throw new InvalidOperationException("End of stream encountered whilst looking for end of object. ");

                                // stop?
                                if (reader.TokenType == JsonNetTokenShim.EndObject)
                                    break;
                            }
                        }
                    }
                }
                else if (reader.TokenType == JsonNetTokenShim.StartArray)
                {
                    // find the field...
                    var field = dto.Type.GetFieldByJsonName(readingName, false);
                    if (field != null)
                    {
                        // go next...
                        if (!(reader.Read()))
                            throw new InvalidOperationException("End of stream encountered whilst stepping into array.");

                        // get the object type...
                        Type subtype = null;
                        if (typeof(IEnumerable).IsAssignableFrom(field.DtoProperty.PropertyType))
                        {
                            if (field.DtoProperty.PropertyType.GenericTypeArguments.Length == 1)
                                subtype = field.DtoProperty.PropertyType.GenericTypeArguments[0];
                            else
                                throw new NotSupportedException(string.Format("A parameter of set length '{0}' is invalid.", field.DtoProperty.PropertyType.GenericTypeArguments.Length));

                        }
                        else
                            throw new NotSupportedException(string.Format("Cannot handle '{0}'.", field.DtoProperty.PropertyType));

                        // read...
                        var readingList = (IList)Activator.CreateInstance(field.DtoProperty.PropertyType);
                        while (true)
                        {
                            var inner = this.ReadJsonInternal(reader, subtype, existingValue, serializer, depth + 1);
                            if (inner == null)
                                throw new InvalidOperationException("'inner' is null.");

                            // add...
                            readingList.Add(inner);

                            // now what?
                            if (!(reader.Read()))
                                throw new InvalidOperationException("End of stream encountered whilst peaking array end.");

                            // what did we get?
                            if (reader.TokenType == JsonNetTokenShim.StartObject)
                            {
                            }
                            else if (reader.TokenType == JsonNetTokenShim.EndArray)
                            {
                                // set the result...
                                dto[field] = readingList;

                                // go back to the top of the loop...
                                didPeek = true;
                                break;
                            }
                            else
                                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", reader.TokenType));

                        }
                    }
                    else
                    {
                        // flush...
                        while (true)
                        {
                            if (reader.TokenType == JsonNetTokenShim.EndArray)
                                break;
                            if (!(reader.Read()))
                                throw new InvalidOperationException("End of stream encountered whilst looking for end of array.");
                        }
                    }
                }

                if (!(didPeek))
                {
                    if (!(reader.Read()))
                        break;
                }
            }

            // return...
            return dto;
        }

        private PropertyInfo GetAdhocProperty(Type type, string name)
        {
            var props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (prop.Name == name)
                    return prop;
                else
                {
                    var jsonNetAttrs = prop.GetCustomAttributes(JsonNetMetadata.JsonPropertyAttributeType, false);
                    if (jsonNetAttrs.Any())
                    {
                        var nameFromAttr = JsonNetMetadata.GetPropertyNameFromAttribute(jsonNetAttrs[0]);
                        if (nameFromAttr == name)
                            return prop;
                    }
                }
            }

            return null;
        }

        public bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
