using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HomeTrack.RavenStore;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;

namespace HomeTrack.Web.Controllers
{
    public class AdminController : Controller
    {
    	private readonly IDocumentStore _documentStore;

    	public AdminController(IDocumentStore documentStore)
    	{
    		_documentStore = documentStore;
    	}

    	//
        // GET: /Admin/

        public ActionResult Index()
        {
			var indexes = _documentStore.DatabaseCommands.GetIndexNames(0, int.MaxValue);
            return Content(string.Join("<br />", indexes));
        }

        public ActionResult IndexDefinition(string name)
        {
			var index = _documentStore.DatabaseCommands.GetIndex(name);
            return Content(index.ToJson());
        }


        //
        // GET: /Admin/Details/5

        public ActionResult Reset()
        {
        	using ( var session = _documentStore.OpenSession() )
			{
				_documentStore.DatabaseCommands.DeleteByIndex("Transactions", new IndexQuery());
				_documentStore.DatabaseCommands.DeleteByIndex("ImportsByImportType", new IndexQuery());

				foreach ( var a in session.Query<RavenStore.Documents.Account>() )
				{
					a.Balance = 0M;
				}

				session.SaveChanges();
			}

        	return Content("Database has been reset");
        }
    }
}
