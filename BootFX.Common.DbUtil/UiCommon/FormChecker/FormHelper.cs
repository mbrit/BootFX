// BootFX - Application framework for .NET applications
// 
// File: FormHelper.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using WF = System.Windows.Forms;
using System.Collections;
using BootFX.Common.Data;
using BootFX.Common.Entities;

namespace BootFX.Common.UI
{
    /// <summary>
    /// Web form helpers.
    /// </summary>
    public static class FormHelper
    {
        //private FormHelper()
        //{
        //}

        ///// <summary>
        ///// Resets a list selection.
        ///// </summary>
        ///// <param name="list"></param>
        //public static void ClearListSelection(ListControl list)
        //{
        //    if (list == null)
        //        throw new ArgumentNullException("list");

        //    foreach (ListItem item in list.Items)
        //        item.Selected = false;
        //}

        ///// <summary>
        ///// Selects the entity from the list.
        ///// </summary>
        ///// <param name="list"></param>
        ///// <param name="entity"></param>
        //public static bool SelectListItemByValue(ListControl list, int value)
        //{
        //    return SelectListItemByValue(list, value.ToString());
        //}

        ///// <summary>
        ///// Selects the entity from the list.
        ///// </summary>
        ///// <param name="list"></param>
        ///// <param name="entity"></param>
        //public static bool SelectListItemByValue(ListControl list, string value)
        //{
        //    if (list == null)
        //        throw new ArgumentNullException("list");

        //    // nothing...
        //    ClearListSelection(list);

        //    // something...
        //    foreach (ListItem item in list.Items)
        //    {
        //        if (string.Compare(item.Value, value, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
        //        {
        //            item.Selected = true;
        //            return true;
        //        }
        //    }

        //    // nope...
        //    return false;
        //}

        ///// <summary>
        ///// Gets a control with the given ID from the given DataGridItem.
        ///// </summary>
        ///// <param name="item"></param>
        ///// <param name="id"></param>
        ///// <param name="controlType">The type of control that's expected, or null if no type assertion should be performed.</param>
        ///// <returns></returns>
        //public static Control GetControl(DataListItem item, string id, Type controlType)
        //{
        //    if (item == null)
        //        throw new ArgumentNullException("item");
        //    return GetControlInternal(item, id, controlType);
        //}

        ///// <summary>
        ///// Gets a control with the given ID from the given DataGridItem.
        ///// </summary>
        ///// <param name="item"></param>
        ///// <param name="id"></param>
        ///// <param name="controlType">The type of control that's expected, or null if no type assertion should be performed.</param>
        ///// <returns></returns>
        //public static Control GetControl(RepeaterItem item, string id, Type controlType)
        //{
        //    if (item == null)
        //        throw new ArgumentNullException("item");
        //    return GetControlInternal(item, id, controlType);
        //}

        ///// <summary>
        ///// Gets a control with the given ID from the given DataGridItem.
        ///// </summary>
        ///// <param name="item"></param>
        ///// <param name="id"></param>
        ///// <param name="controlType">The type of control that's expected, or null if no type assertion should be performed.</param>
        ///// <returns></returns>
        //public static Control GetControl(DataGridItem item, string id, Type controlType)
        //{
        //    if (item == null)
        //        throw new ArgumentNullException("item");
        //    return GetControlInternal(item, id, controlType);
        //}

        ///// <summary>
        ///// Gets a control with the given ID fro the given DataGridItem.
        ///// </summary>
        ///// <param name="item"></param>
        ///// <param name="id"></param>
        ///// <param name="controlType">The type of control that's expected, or null if no type assertion should be performed.</param>
        ///// <returns></returns>
        //private static Control GetControlInternal(Control item, string id, Type controlType)
        //{
        //    if (item == null)
        //        throw new ArgumentNullException("item");

        //    // mbr - 09-02-2006 - if the ID is blank, assume control is what we actually want.
        //    if (id == null || id.Length == 0)
        //        return item;

        //    // mbr - 21-03-2006 - this can probably use INamingContainer, but too nervous to make a change like that right now.			
        //    Control control = null;
        //    if (item is DataGridItem)
        //        control = ((DataGridItem)item).FindControl(id);
        //    else if (item is RepeaterItem)
        //        control = ((RepeaterItem)item).FindControl(id);
        //    else if (item is DataListItem)
        //        control = ((DataListItem)item).FindControl(id);
        //    else
        //        throw new NotSupportedException(string.Format("Cannot handle '{0}'.", item));

        //    // what happened?
        //    if (control != null)
        //    {
        //        if (controlType == null)
        //            return control;
        //        else
        //        {
        //            Type foundType = control.GetType();
        //            if (!(controlType.IsAssignableFrom(foundType)))
        //                throw new InvalidOperationException(string.Format("Cannot convert '{0}' ('{1}') to '{2}'.", foundType, control.ID, controlType));
        //            return control;
        //        }
        //    }
        //    else
        //        throw new InvalidOperationException(string.Format("Failed to find control '{0}' on DataGridItem.", id));
        //}

        ///// <summary>
        ///// Gets a control with the given ID fro the given DataGridItem.
        ///// </summary>
        ///// <param name="item"></param>
        ///// <param name="id"></param>
        ///// <param name="controlType">The type of control that's expected, or null if no type assertion should be performed.</param>
        ///// <param name="throwIfNotFound"></param>
        ///// <returns></returns>
        //[Obsolete("Call the version that does not take throw if not found.", false)]
        //public static Control GetControl(DataGridItem item, string id, Type controlType, bool throwIfNotFound)
        //{
        //    return GetControl(item, id, controlType);
        //}

        //public static Label GetLabel(DataGridItem item, string labelId)
        //{
        //    return (Label)GetControl(item, labelId, typeof(Label));
        //}

        //[Obsolete("Use the version of this method that does not have 'throwIfNotFound'.")]
        //public static Label GetLabel(DataGridItem item, string labelId, bool throwIfNotFound)
        //{
        //    return GetLabel(item, labelId);
        //}

        ///// <summary>
        ///// Sets the text on a label.
        ///// </summary>
        ///// <param name="item"></param>
        ///// <param name="labelId"></param>
        ///// <param name="text"></param>
        //[Obsolete("Use SetControlText instead.")]
        //public static void SetLabelText(DataGridItem item, string labelId, string text)
        //{
        //    SetControlText(item, labelId, text);
        //}

        ///// <summary>
        ///// Sets the text on a label.
        ///// </summary>
        ///// <param name="item"></param>
        ///// <param name="labelId"></param>
        ///// <param name="text"></param>
        //[Obsolete("Use SetControlText instead.")]
        //public static void SetLabelText(DataGridItem item, string labelId, string text, bool throwIfNotFound)
        //{
        //    SetLabelText(item, labelId, text);
        //}

        //public static void SetControlText(DataListItem item, string id, string text)
        //{
        //    if (item == null)
        //        throw new ArgumentNullException("item");
        //    SetControlTextInternal(item, id, text);
        //}

        //public static void SetControlText(RepeaterItem item, string id, string text)
        //{
        //    if (item == null)
        //        throw new ArgumentNullException("item");
        //    SetControlTextInternal(item, id, text);
        //}

        //public static void SetControlText(DataGridItem item, string id, string text)
        //{
        //    if (item == null)
        //        throw new ArgumentNullException("item");
        //    SetControlTextInternal(item, id, text);
        //}

        //private static void SetControlTextInternal(Control item, string id, string text)
        //{
        //    if (item == null)
        //        throw new ArgumentNullException("item");

        //    // get it...
        //    Control control = GetControlInternal(item, id, typeof(Control));
        //    if (control == null)
        //        throw new InvalidOperationException("control is null.");

        //    // textbox?
        //    if (control is TextBox)
        //        ((TextBox)control).Text = text;
        //    else if (control is Label)
        //        ((Label)control).Text = text;
        //    else
        //        throw new NotSupportedException(string.Format("Control '{0}' is not supported.", control));
        //}

        ///// <summary>
        ///// Gets an enumeration value.
        ///// </summary>
        ///// <param name="controll"></param>
        ///// <param name="enumType"></param>
        ///// <param name="required"></param>
        ///// <returns></returns>
        //public static object GetEnumerationValue(RepeaterItem item, string id, Type enumType)
        //{
        //    bool found = false;
        //    return GetEnumerationValue(item, id, enumType, ref found);
        //}

        ///// <summary>
        ///// Gets an enumeration value.
        ///// </summary>
        ///// <param name="controll"></param>
        ///// <param name="enumType"></param>
        ///// <param name="required"></param>
        ///// <returns></returns>
        //public static object GetEnumerationValue(DataGridItem item, string id, Type enumType)
        //{
        //    bool found = false;
        //    return GetEnumerationValue(item, id, enumType, ref found);
        //}

        ///// <summary>
        ///// Gets an enumeration value.
        ///// </summary>
        ///// <param name="controll"></param>
        ///// <param name="enumType"></param>
        ///// <param name="required"></param>
        ///// <returns></returns>
        //public static object GetEnumerationValue(Control control, Type enumType)
        //{
        //    bool found = false;
        //    return GetEnumerationValue(control, enumType, ref found);
        //}

        ///// <summary>
        ///// Gets an enumeration value.
        ///// </summary>
        ///// <param name="controll"></param>
        ///// <param name="enumType"></param>
        ///// <param name="required"></param>
        ///// <returns></returns>
        //public static object GetEnumerationValue(RepeaterItem item, string id, Type enumType, ref bool found)
        //{
        //    return GetEnumerationValue(new ControlReference(item, id, null), enumType, ref found);
        //}

        ///// <summary>
        ///// Gets an enumeration value.
        ///// </summary>
        ///// <param name="controll"></param>
        ///// <param name="enumType"></param>
        ///// <param name="required"></param>
        ///// <returns></returns>
        //public static object GetEnumerationValue(DataGridItem item, string id, Type enumType, ref bool found)
        //{
        //    return GetEnumerationValue(new ControlReference(item, id, null), enumType, ref found);
        //}

        ///// <summary>
        ///// Gets an enumeration value.
        ///// </summary>
        ///// <param name="controll"></param>
        ///// <param name="enumType"></param>
        ///// <param name="required"></param>
        ///// <returns></returns>
        //public static object GetEnumerationValue(Control control, Type enumType, ref bool found)
        //{
        //    return GetEnumerationValue(new ControlReference(control, null), enumType, ref found);
        //}

        ///// <summary>
        ///// Gets an enumeration value.
        ///// </summary>
        ///// <param name="controll"></param>
        ///// <param name="enumType"></param>
        ///// <param name="required"></param>
        ///// <returns></returns>
        //public static object GetEnumerationValue(ControlReference reference, Type enumType, ref bool found)
        //{
        //    if (reference == null)
        //        throw new ArgumentNullException("reference");
        //    if (enumType == null)
        //        throw new ArgumentNullException("enumType");
        //    if (!(enumType.IsEnum))
        //        throw new ArgumentException(string.Format("'{0}' is not an enumeration."));

        //    // get the value...
        //    string asString = GetStringValue(reference);
        //    if (asString == null || asString.Length == 0)
        //    {
        //        found = false;
        //        return 0;
        //    }
        //    else
        //        found = true;

        //    // have we got all numbers?
        //    bool allDigits = true;
        //    foreach (char c in asString)
        //    {
        //        if (!(char.IsDigit(c)))
        //        {
        //            allDigits = false;
        //            break;
        //        }
        //    }

        //    // digits?
        //    if (allDigits)
        //        return ConversionHelper.ToInt32(asString, Cultures.User);
        //    else
        //    {
        //        try
        //        {
        //            return Enum.Parse(enumType, asString, true);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new InvalidOperationException(string.Format("Failed to convert '{0}' to enumeration type '{1}'.", asString, enumType), ex);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Gets a string value.
        ///// </summary>
        ///// <param name="control"></param>
        ///// <returns></returns>
        //public static string GetStringValue(RepeaterItem control, string id)
        //{
        //    return GetStringValue(new ControlReference(control, id, null));
        //}

        ///// <summary>
        ///// Gets a string value.
        ///// </summary>
        ///// <param name="control"></param>
        ///// <returns></returns>
        //public static string GetStringValue(DataGridItem control, string id)
        //{
        //    return GetStringValue(new ControlReference(control, id, null));
        //}

        ///// <summary>
        ///// Gets a string value.
        ///// </summary>
        ///// <param name="control"></param>
        ///// <returns></returns>
        //public static string GetStringValue(Control control)
        //{
        //    return GetStringValue(new ControlReference(control, null));
        //}

        ///// <summary>
        ///// Gets the text from the list.
        ///// </summary>
        ///// <param name="text"></param>
        ///// <param name="required"></param>
        ///// <param name="context"></param>
        ///// <returns></returns>
        //public static string GetStringValue(ControlReference reference)
        //{
        //    if (reference == null)
        //        throw new ArgumentNullException("reference");

        //    // get...
        //    if (reference.IsWebControl)
        //        return GetStringValueWeb(reference);
        //    else
        //        return GetStringValueWf(reference);
        //}

        private static string GetStringValueWf(ControlReference reference)
        {
            // resolve...
            WF.Control useControl = reference.ResolvedWfControl;
            if (useControl == null)
                throw new InvalidOperationException("useControl is null.");

            // what is it?
            if (useControl is WF.TextBox)
            {
                string value = ((WF.TextBox)useControl).Text;
                if (value != null)
                    value = value.Trim();
                return value;
            }
            // mbr - 27-02-2007 - erred on the side of caution with this one even though
            // TextBoxBase should work - don't want to risk knock-on effects.
            else if (useControl is WF.RichTextBox)
            {
                string value = ((WF.RichTextBox)useControl).Text;
                if (value != null)
                    value = value.Trim();
                return value;
            }
            else if (useControl is WF.ComboBox)
            {
                string value = ((WF.ComboBox)useControl).Text;
                if (value != null)
                    value = value.Trim();
                return value;
            }
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", useControl));
        }

        /// <summary>
        /// Finds controls within the control tree of the given type.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="controlType"></param>
        /// <returns></returns>
        public static WF.Control[] GetControlsRecursive(WF.Control container, Type controlType)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (controlType == null)
                throw new ArgumentNullException("controlType");

            // create...
            ArrayList results = new ArrayList();
            GetControlsRecursiveInternal(container, controlType, results);

            // return...
            return (WF.Control[])results.ToArray(typeof(WF.Control));
        }

        private static void GetControlsRecursiveInternal(WF.Control container, Type controlType, ArrayList results)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (controlType == null)
                throw new ArgumentNullException("controlType");
            if (results == null)
                throw new ArgumentNullException("results");

            // if...
            if (controlType.IsAssignableFrom(container.GetType()))
                results.Add(container);

            // walk...
            foreach (WF.Control child in container.Controls)
                GetControlsRecursiveInternal(child, controlType, results);
        }
    }
}
