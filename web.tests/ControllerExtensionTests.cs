using System;
using System.Web.Mvc;
using HomeTrack.Web.Controllers;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace HomeTrack.Web.Tests
{
	[TestFixture]
	public class ControllerExtensionTests
	{
		[Test]	 
		public void ModelErrorIntoJson()
		{
			var modelState = new ModelStateDictionary();
			var jsonResult = ControllerExtensions.ToJson(modelState);

			var data = jsonResult.Data;
			Assert.That(data, Has.Property("Tag").EqualTo("ValidationError"));
			Assert.That(data, Has.Property("State").Empty);
		}
		
		[Test]	 
		public void SimpleModelErrorIntoJson()
		{
			var modelState = new ModelStateDictionary();
			modelState.Add("FirstName", new ModelState { Errors = { "First name is a required field" } });

			var jsonResult = ControllerExtensions.ToJson(modelState);

			var data = jsonResult.Data;
			Assert.That(data, Has.Property("State").Not.Empty);
			Assert.That(data, Has.Property("State").With.Some.Property("Name").EqualTo("FirstName"));
			Assert.That(data, Has.Property("State").With.Some.Property("Errors").With.Some.EqualTo("First name is a required field"));
		}
		
		[Test]	 
		public void ModelErrorWithExceptionIntoJson()
		{
			var modelState = new ModelStateDictionary();
			modelState.Add("FirstName", new ModelState { Errors = { new InvalidOperationException("Cannot operate") } });
			
			var jsonResult = ControllerExtensions.ToJson(modelState);

			var data = jsonResult.Data;
			Assert.That(data, Has.Property("State").Not.Empty);
			Assert.That(data, Has.Property("State").With.Some.Property("Name").EqualTo("FirstName"));
			Assert.That(data, Has.Property("State").With.Some.Property("Errors").With.Some.EqualTo("Cannot operate"));
		}
	}
}