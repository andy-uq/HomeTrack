using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        //
        // GET: /Admin/Details/5

        public ActionResult DeleteTransactions()
        {
        	int tCount = 0;

			using ( var session = _documentStore.OpenSession() )
			{
				foreach (var t in session.Query<RavenStore.Documents.Transaction>())
				{
					session.Delete(t);
					tCount++;
				}

				foreach ( var a in session.Query<RavenStore.Documents.Account>() )
				{
					a.Balance = 0M;
				}

				session.SaveChanges();
			}

        	return Content(string.Format("{0} transactions deleted", tCount));
        }
    }
}
