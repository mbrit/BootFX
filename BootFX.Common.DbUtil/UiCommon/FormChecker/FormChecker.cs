//// BootFX - Application framework for .NET applications
//// 
//// File: FormChecker.cs
//// Build: 5.0.61009.900
//// 
//// An open source project by Matthew Reynolds (@mbrit).  
//// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
//// Elixia Solutions Limited.  All Rights Reserved.
////
//// Licensed under the MIT license.

//using System;
//using System.Web;
//using WF = System.Windows.Forms;
//using System.Text;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.ComponentModel;
//using BootFX.Common;
//using BootFX.Common.Data;
//using BootFX.Common.Entities;
//using BootFX.Common.Management;

//namespace BootFX.Common.UI
//{
//	/// <summary>
//	/// Defines a class that can be used to check form input.
//	/// </summary>
//	public class FormChecker : IUserMessages, IErrorCollector
//	{
//		/// <summary>
//		/// Private field to support <see cref="Errors"/> property.
//		/// </summary>
//		private StringCollection _errors;

//		public FormChecker()
//		{
//			this.Reset();
//		}
		
//		/// <summary>
//		/// Constructor.
//		/// </summary>
//		public FormChecker(Control baseControl) : this()
//		{
//			this.Initialize(baseControl);
//		}

//		/// <summary>
//		/// Constructor.
//		/// </summary>
//		public FormChecker(Page basePage) : this((Control)basePage)
//		{
//		}

//		public FormChecker(WF.Control control) : this()
//		{
//			this.Initialize(control);
//		}

//		private void Initialize(object control)
//		{
//			if(control == null)
//				throw new ArgumentNullException("control");
			
//			// we don't actually do anything with this?
//		}

//		/// <summary>
//		/// Gets an entity from a list.
//		/// </summary>
//		/// <param name="list"></param>
//		/// <param name="caption"></param>
//		/// <param name="required"></param>
//		/// <returns></returns>
//		public object GetEntity(DataGridItem item, string id, Type entityType, bool required)
//		{
//			if(item == null)
//				throw new ArgumentNullException("item");
//			if(id == null)
//				throw new ArgumentNullException("id");
//			if(id.Length == 0)
//				throw new ArgumentOutOfRangeException("'id' is zero-length.");
			
//			// find...
//			Control control = FormHelper.GetControl(item, id, typeof(Control));
//			if(control == null)
//				throw new InvalidOperationException("control is null.");

//			// defer...
//			return GetEntity(control, entityType, required);
//		}

//		/// <summary>
//		/// Gets an entity from a list.
//		/// </summary>
//		/// <param name="list"></param>
//		/// <param name="caption"></param>
//		/// <param name="required"></param>
//		/// <returns></returns>
//		public object GetEntity(Control control, Type entityType, bool required)
//		{
//			return this.GetEntity(new ControlReference(control, null), entityType, required);
//		}

//		/// <summary>
//		/// Gets an entity from a list.
//		/// </summary>
//		/// <param name="list"></param>
//		/// <param name="caption"></param>
//		/// <param name="required"></param>
//		/// <returns></returns>
//		public object GetEntity(ControlReference reference, Type entityType, bool required)
//		{
//			if(reference == null)
//				throw new ArgumentNullException("reference");
			
//			// defer...
//			object entity = FormHelper.GetEntity(reference, entityType);
//			if(entity != null)
//				return entity;
//			else
//			{
//				if(required)
//					this.AddError(string.Format("You must select an item for '{0}'.", reference.Caption));

//				// nothing...
//				return null;
//			}
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public object GetValue(RepeaterItem item, string id, Type type, object defaultValue, bool required)
//		{
//			return this.GetValue(new ControlReference(item, id, null), type, defaultValue, required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public object GetValue(DataGridItem item, string id, Type type, object defaultValue, bool required)
//		{
//			return this.GetValue(new ControlReference(item, id, null), type, defaultValue, required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public object GetValue(Control control, Type type, object defaultValue, bool required)
//		{
//			return this.GetValue(new ControlReference(control, null), type, defaultValue, required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public object GetValue(ControlReference reference, Type type, object defaultValue, bool required)
//		{
//			if(reference == null)
//				throw new ArgumentNullException("reference");
//			if(type == null)
//				throw new ArgumentNullException("type");

//			// get the string value...
//			string asString = this.GetStringValue(reference, required);
//			if(asString != null && asString.Length > 0)
//			{
//				try
//				{
//					return ConversionHelper.ChangeType(asString, type, Cultures.User);
//				}
//				catch(Exception ex)
//				{
//					this.AddError(string.Format("Failed to convert '{0}' to '{1}'.  ({2})", asString, type, ex.Message));
//				}
//			}

//			// return the default if not found or couldn't convert...
//			return defaultValue;
//		}

//		/// <summary>
//		/// Gets an enumeration value.
//		/// </summary>
//		/// <param name="controll"></param>
//		/// <param name="enumType"></param>
//		/// <param name="required"></param>
//		/// <returns></returns>
//		public object GetEnumerationValue(RepeaterItem item, string id, Type enumType, bool required)
//		{
//			return this.GetEnumerationValue(new ControlReference(item, id, null), enumType, required);
//		}

//		/// <summary>
//		/// Gets an enumeration value.
//		/// </summary>
//		/// <param name="controll"></param>
//		/// <param name="enumType"></param>
//		/// <param name="required"></param>
//		/// <returns></returns>
//		public object GetEnumerationValue(DataGridItem item, string id, Type enumType, bool required)
//		{
//			return this.GetEnumerationValue(new ControlReference(item, id, null), enumType, required);
//		}

//		/// <summary>
//		/// Gets an enumeration value.
//		/// </summary>
//		/// <param name="controll"></param>
//		/// <param name="enumType"></param>
//		/// <param name="required"></param>
//		/// <returns></returns>
//		public object GetEnumerationValue(Control control, Type enumType, bool required)
//		{
//			return this.GetEnumerationValue(new ControlReference(control, null), enumType, required);
//		}

//		/// <summary>
//		/// Gets an enumeration value.
//		/// </summary>
//		/// <param name="controll"></param>
//		/// <param name="enumType"></param>
//		/// <param name="required"></param>
//		/// <returns></returns>
//		public object GetEnumerationValue(ControlReference reference, Type enumType, bool required)
//		{
//			if(reference == null)
//				throw new ArgumentNullException("reference");
//			if(enumType == null)
//				throw new ArgumentNullException("enumType");
//			if(!(enumType.IsEnum))
//				throw new ArgumentException(string.Format("'{0}' is not an enumeration."));
			
//			// get the value...
//			string asString = this.GetStringValue(reference, required);
//			if(asString == null || asString.Length == 0)
//				return 0;

//			// have we got all numbers?
//			bool allDigits = true;
//			foreach(char c in asString)
//			{
//				if(!(char.IsDigit(c)))
//				{
//					allDigits = false;
//					break;
//				}
//			}

//			// digits?
//			if(allDigits)
//				return ConversionHelper.ToInt32(asString, Cultures.User);
//			else
//			{
//				try
//				{
//					return Enum.Parse(enumType, asString, true);
//				}
//				catch(Exception ex)
//				{
//					throw new InvalidOperationException(string.Format("Failed to convert '{0}' to enumeration type '{1}'.", asString, enumType), ex);
//				}
//			}
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public decimal GetDecimalValue(DataGridItem item, string id, bool required)
//		{
//			return this.GetDecimalValue(new ControlReference(item, id, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public decimal GetDecimalValue(RepeaterItem item, string id, bool required)
//		{
//			return this.GetDecimalValue(new ControlReference(item, id, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public decimal GetDecimalValue(Control control, bool required)
//		{
//			return this.GetDecimalValue(new ControlReference(control, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public decimal GetDecimalValue(ControlReference reference, bool required)
//		{
//			if(reference == null)
//				throw new ArgumentNullException("reference");
			
//			// get the string value...
//			return (decimal)this.GetValue(reference, typeof(decimal), 0M, required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public int GetInt32Value(Control control, bool required)
//		{
//			return this.GetInt32Value(new ControlReference(control, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public int GetInt32Value(DataGridItem item, string id, bool required)
//		{
//			return this.GetInt32Value(new ControlReference(item, id, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public int GetInt32Value(RepeaterItem item, string id, bool required)
//		{
//			return this.GetInt32Value(new ControlReference(item, id, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public int GetInt32Value(ControlReference reference, bool required)
//		{
//			// return...
//			return (int)this.GetValue(reference, typeof(int), 0, required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public short GetInt16Value(Control control, bool required)
//		{
//			return this.GetInt16Value(new ControlReference(control, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public short GetInt16Value(DataGridItem item, string id, bool required)
//		{
//			return this.GetInt16Value(new ControlReference(item, id, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public short GetInt16Value(RepeaterItem item, string id, bool required)
//		{
//			return this.GetInt16Value(new ControlReference(item, id, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public short GetInt16Value(ControlReference reference, bool required)
//		{
//			// return...
//			// Doesn't like casting the default value to a short (RSF)
//            return (Convert.ToInt16(this.GetValue(reference, typeof(short), 0, required)));
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public float GetFloatValue(Control control, bool required)
//		{
//			return this.GetFloatValue(new ControlReference(control, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public float GetFloatValue(DataGridItem item, string id, bool required)
//		{
//			return this.GetFloatValue(new ControlReference(item, id, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public float GetFloatValue(RepeaterItem item, string id, bool required)
//		{
//			return this.GetFloatValue(new ControlReference(item, id, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public float GetFloatValue(ControlReference reference, bool required)
//		{
//			// return...
//			return (Convert.ToSingle(this.GetValue(reference, typeof(float), 0, required)));
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public double GetDoubleValue(Control control, bool required)
//		{
//			return this.GetDoubleValue(new ControlReference(control, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public double GetDoubleValue(DataGridItem item, string id, bool required)
//		{
//			return this.GetDoubleValue(new ControlReference(item, id, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public double GetDoubleValue(RepeaterItem item, string id, bool required)
//		{
//			return this.GetDoubleValue(new ControlReference(item, id, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public double GetDoubleValue(ControlReference reference, bool required)
//		{
//			// return...
//			return (Convert.ToDouble(this.GetValue(reference, typeof(double), 0, required)));
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public byte GetByteValue(Control control, bool required)
//		{
//			return this.GetByteValue(new ControlReference(control, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public byte GetByteValue(DataGridItem item, string id, bool required)
//		{
//			return this.GetByteValue(new ControlReference(item, id, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public byte GetByteValue(RepeaterItem item, string id, bool required)
//		{
//			return this.GetByteValue(new ControlReference(item, id, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public byte GetByteValue(ControlReference reference, bool required)
//		{
//			// return...
//			// Doesn't like casting the default value to a byte (RSF)
//			return (Convert.ToByte(this.GetValue(reference, typeof(byte), 0, required)));
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public string GetStringValue(Control control, bool required)
//		{
//			return this.GetStringValue(new ControlReference(control, null), required);
//		}

//		/// <summary>
//		/// Gets the text from any control.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public string GetStringValue(ControlReference reference, bool required)
//		{
//			if(reference == null)
//				throw new ArgumentNullException("reference");
			
//			// defer...
//			string value = FormHelper.GetStringValue(reference);
//			if((value == null || value.Length == 0) && required)
//				this.AddMissingValueError(reference);

//			// return...
//			return value;
//		}

//		private void AddMissingValueError(ControlReference reference)
//		{
//			if(reference == null)
//				throw new ArgumentNullException("reference");
//			this.AddError(string.Format("You must provide a value for '{0}'.", reference.Caption));
//		}

//		/// <summary>
//		/// Gets the text from the list.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public string GetStringValue(DataGridItem item, string name, bool required)
//		{
//			Control control = FormHelper.GetControl(item, name, typeof(Control));
//			if(control == null)
//				throw new InvalidOperationException("control is null.");

//			// return...
//			return this.GetStringValue(control, required);
//		}

//		/// <summary>
//		/// Gets the text from the text box.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public DateTime GetDateTimeValue(ControlReference reference, bool required)
//		{
//			if(reference == null)
//				throw new ArgumentNullException("reference");
			
//			// return..
//			return (DateTime)this.GetValue(reference, typeof(DateTime), DateTime.MinValue, required);
//		}

//		/// <summary>
//		/// Gets the text from the text box.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public DateTime GetDateTimeValue(DataGridItem item, string id, bool required)
//		{
//			if(item == null)
//				throw new ArgumentNullException("item");
//			if(id == null)
//				throw new ArgumentNullException("id");
//			if(id.Length == 0)
//				throw new ArgumentOutOfRangeException("'id' is zero-length.");
			
//			// get...
//			Control control = FormHelper.GetControl(item, id, typeof(Control));
//			if(control == null)
//				throw new InvalidOperationException("control is null.");

//			// defer...
//			return this.GetDateTimeValue(control, required);
//		}

//		/// <summary>
//		/// Gets the text from the text box.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public DateTime GetDateTimeValue(Control control, bool required)
//		{
//			if(control == null)
//				throw new ArgumentNullException("control");
			
//			if(control is TextBox)
//				return this.GetDateTimeValue((TextBox)control, required);
//			else
//				throw new NotSupportedException(string.Format("Cannot handle '{0}'.", control));
//		}

//		/// <summary>
//		/// Gets the text from the text box.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public DateTime GetDateTimeValue(TextBox control, bool required)
//		{
//			string asString = this.GetStringValue(control, required);
//			if(asString != null && asString.Length > 0)
//			{
//				try
//				{
//					return ConversionHelper.ToDateTime(asString, Cultures.User);
//				}
//				catch(Exception ex)
//				{
//					this.AddError(string.Format("Could not convert '{0}' to a date/time.  ({1})", asString, ex.Message));
//					return DateTime.MinValue;
//				}
//			}
//			else
//				return DateTime.MinValue;
//		}

//		/// <summary>
//		/// Gets the text from the text box.
//		/// </summary>
//		/// <param name="text"></param>
//		/// <param name="required"></param>
//		/// <param name="context"></param>
//		/// <returns></returns>
//		public string GetStringValue(TextBox control, bool required)
//		{
//			return this.GetStringValue(new ControlReference(control, null), required);
//		}

//		/// <summary>
//		/// Throws an exception if errors occur.
//		/// </summary>
//		public void AssertNoErrors()
//		{
//			if(this.HasErrors)
//				throw new InvalidOperationException(string.Format("Validation errors have occurred:\r\n{0}", this.GetAllErrorsSeparatedByCrLf()));
//		}

//		public bool HasErrors
//		{
//			get
//			{
//				if(this.Errors.Count > 0)
//					return true;
//				else
//					return false;
//			}
//		}

//		/// <summary>
//		/// Logs an error.
//		/// </summary>
//		/// <param name="message"></param>
//		public void AddError(string message)
//		{
//			if(message == null)
//				throw new ArgumentNullException("message");
//			if(message.Length == 0)
//				throw new ArgumentOutOfRangeException("'message' is zero-length.");
			
//			// add...
//			this.Errors.Add(message);
//		}

//		/// <summary>
//		/// Gets the errors.
//		/// </summary>
//		private StringCollection Errors
//		{
//			get
//			{
//				return _errors;
//			}
//		}

//		/// <summary>
//		/// Gets all the errors.
//		/// </summary>
//		/// <returns></returns>
//		public string[] GetAllErrors()
//		{
//			string[] results = new string[this.Errors.Count];
//			for(int index = 0; index < this.Errors.Count; index++)
//				results[index] = this.Errors[index];

//			// return...
//			return results;
//		}

//		/// <summary>
//		/// Gets all the errors as a block.
//		/// </summary>
//		/// <returns></returns>
//		public string GetAllErrorsSeparatedByCrLf()
//		{
//			StringBuilder builder = new StringBuilder();
//			foreach(string error in this.Errors)
//			{
//				if(builder.Length > 0)
//					builder.Append("\r\n");
//				builder.Append(error);
//			}

//			// return...
//			return builder.ToString();
//		}

//		/// <summary>
//		/// Gets all errors separated by a BR tag and encoded ready for display.
//		/// </summary>
//		/// <returns></returns>
//		public string GetAllErrorsSeparatedByBrTag()
//		{
//			StringBuilder builder = new StringBuilder();
//			foreach(string error in this.Errors)
//			{
//				if(builder.Length > 0)
//					builder.Append("<BR />");
//				builder.Append(HttpUtility.HtmlEncode(error));
//			}

//			// return...
//			return builder.ToString();
//		}

//		/// <summary>
//		/// Gets the user messages.
//		/// </summary>
//		/// <returns></returns>
//		string[] IUserMessages.GetUserMessages()
//		{
//			return this.GetAllErrors();
//		}

//		/// <summary>
//		/// Resets the control.
//		/// </summary>
//		private void Reset()
//		{
//			_errors = new StringCollection();
//			//			_values = CollectionsUtil.CreateCaseInsensitiveHashtable();
//		}

//		public long GetInt64Value(Control control, bool required)
//		{
//			// get the string value...
//			return (int)this.GetValue(control, typeof(int), 0, required);
//		}

//        public void AddErrors(IUserMessages messages)
//        {
//            this.AddErrors(messages.GetUserMessages());
//        }

//        private void AddErrors(IEnumerable<string> errors)
//        {
//            foreach (var error in errors)
//                this.AddError(error);
//        }
//	}
//}
