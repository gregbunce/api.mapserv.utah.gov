﻿using System;
using System.Linq;
using System.Web.Mvc;
using Ninject;
using Raven.Client;
using StackExchange.Redis;
using WebAPI.Common.Executors;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Dashboard.Commands.Key;
using WebAPI.Dashboard.Controllers;
using WebAPI.Dashboard.Models.ViewModels;

namespace WebAPI.Dashboard.Areas.secure.Controllers
{
    [Authorize]
    public class HomeController : RavenController
    {
        public HomeController(IDocumentStore store)
            : base(store) {}

        [Inject]
        public ConnectionMultiplexer Redis { get; set; }

        [HttpGet]
        public ActionResult Index()
        {
            var account = Account;
            if (account == null)
            {
                return Logout();
            }

            var keys = Session.Query<ApiKey, IndexKeysForUser>()
                            .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                            .Where(x => x.ApiKeyStatus == ApiKey.KeyStatus.Active &&
                                        !x.Deleted &&
                                        x.AccountId == Account.Id);

            var stats = CommandExecutor.ExecuteCommand(new TotalRedisStatsCommand(Redis.GetDatabase(), keys));

            if (account.KeyQuota.KeysUsed == 0)
            {
                var href = Url.Action("Index", new
                {
                    Controller = "KeyManagement"
                });

                Message =
                    "You haven't created any API keys yet. To start using the web API " +
                    string.Format("click <a href='{0}'>Manage manage my API keys</a> and generate one.", href);
            }

            return View(new MainViewModel(account)
                .WithApiUsage(stats)
                .WithKeyQuota(account.KeyQuota));
        }
    }
}