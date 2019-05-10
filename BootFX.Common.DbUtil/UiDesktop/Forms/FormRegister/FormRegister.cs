// BootFX - Application framework for .NET applications
// 
// File: FormRegister.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using System.Collections;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Holds a collection of forms.
	/// </summary>
	public class FormRegister : IEnumerable
	{
		/// <summary>
		/// Raised when a form has been closed.
		/// </summary>
		public event FormEventHandler FormClosed;
		
		/// <summary>
		/// Raised when a form has been activated.
		/// </summary>
		public event FormEventHandler FormActivated;
		
		/// <summary>
		/// Raised when a form has been activated.
		/// </summary>
		public event FormEventHandler FormMdiChildActivated;
		
		/// <summary>
		/// Private field to support <c>Forms</c> property.
		/// </summary>
		private FormCollection _forms = new FormCollection();
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public FormRegister()
		{
		}

		/// <summary>
		/// Shows the singleton form of the given type, creating one if it is not found.
		/// </summary>
		/// <param name="type"></param>
		public Form GetFormOfType(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// defer...
			return this.GetFormOfType(type, new object[] {});
		}

		/// <summary>
		/// Shows the singleton form of the given type, creating one if it is not found.
		/// </summary>
		/// <param name="type"></param>
		public Form GetFormOfType(Type type, object[] constructorArguments)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			// find it...
			foreach(Form form in this.Forms)
			{
				if(type.IsAssignableFrom(form.GetType()))
					return form;
			}

			// create one...
			return (Form)Activator.CreateInstance(type, constructorArguments);
		}

		/// <summary>
		/// Returns true if the register contains this form.
		/// </summary>
		/// <param name="form"></param>
		/// <returns></returns>
		public bool Contains(Form form)
		{
			if(form == null)
				throw new ArgumentNullException("form");
			
			// return...
			return this.Forms.Contains(form);
		}

		/// <summary>
		/// Adds a form to the collection.
		/// </summary>
		/// <param name="form"></param>
		public void AddForm(Form form)
		{
			if(form == null)
				throw new ArgumentNullException("form");

			// check...
			if(this.Contains(form))
				throw new InvalidOperationException(string.Format("Register already contains '{0}'.", form));
			
			// add...
			this.Forms.Add(form);

			// sub...
			this.SubscribeForm(form);
		}

		/// <summary>
		/// Subscribes the form.
		/// </summary>
		/// <param name="form"></param>
		private void SubscribeForm(Form form)
		{
			if(form == null)
				throw new ArgumentNullException("form");

			// sub...
			form.HandleDestroyed += new EventHandler(form_HandleDestroyed);			
			form.Activated += new EventHandler(form_Activated);
			form.MdiChildActivate += new EventHandler(form_MdiChildActivate);
		}

		/// <summary>
		/// Subscribes the form.
		/// </summary>
		/// <param name="form"></param>
		private void UnsubscribeForm(Form form)
		{
			if(form == null)
				throw new ArgumentNullException("form");

			// sub...
			form.HandleDestroyed -= new EventHandler(form_HandleDestroyed);
			form.Activated -= new EventHandler(form_Activated);
			form.MdiChildActivate -= new EventHandler(form_MdiChildActivate);
		}

		/// <summary>
		/// Gets a collection of Form objects.
		/// </summary>
		private FormCollection Forms
		{
			get
			{
				return _forms;
			}
		}

		private void form_HandleDestroyed(object sender, EventArgs e)
		{
			Form form = (Form)sender;
			if(form == null)
				throw new InvalidOperationException("form is null.");

			// remove it...
			int index = this.Forms.IndexOf(form);
			if(index != -1)
				this.Forms.RemoveAt(index);

			// unsub...
			this.UnsubscribeForm(form);

			// notifiy...
			this.OnFormClosed(new FormEventArgs(form));
		}

		/// <summary>
		/// Raises the <c>FormClosed</c> event.
		/// </summary>
		protected virtual void OnFormClosed(FormEventArgs e)
		{
			// raise...
			if(FormClosed != null)
				FormClosed(this, e);
		}

		/// <summary>
		/// Gets the number of forms.
		/// </summary>
		public int Count
		{
			get
			{
				return this.Forms.Count;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.Forms.GetEnumerator();
		}

		private void form_Activated(object sender, EventArgs e)
		{
			this.OnFormClosed(new FormEventArgs((Form)sender));
		}

		/// <summary>
		/// Raises the <c>FormActivated</c> event.
		/// </summary>
		protected virtual void OnFormActivated(FormEventArgs e)
		{
			// raise...
			if(FormActivated != null)
				FormActivated(this, e);
		}

		private void form_MdiChildActivate(object sender, EventArgs e)
		{
			this.OnFormMdiChildActivated(new FormEventArgs((Form)sender));
		}
	
		/// <summary>
		/// Raises the <c>FormActivated</c> event.
		/// </summary>
		protected virtual void OnFormMdiChildActivated(FormEventArgs e)
		{
			// raise...
			if(FormMdiChildActivated != null)
				FormMdiChildActivated(this, e);
		}

		/// <summary>
		/// Gets the forms.
		/// </summary>
		/// <returns></returns>
		public Form[] GetForms()
		{
			return this.Forms.ToArray();
		}
	}
}
