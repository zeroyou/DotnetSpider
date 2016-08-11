﻿using System;
using System.Collections.Generic;
using DotnetSpider.Core.Downloader;
using DotnetSpider.Extension;
using DotnetSpider.Extension.Configuration;
using DotnetSpider.Extension.Model;
using DotnetSpider.Extension.Model.Attribute;
using DotnetSpider.Extension.ORM;
using DotnetSpider.Core;
using DotnetSpider.Extension.Downloader;

namespace DotnetSpider.Sample
{
	public class Hao360SpiderInfoBuble : SpiderBuilder
	{
		protected override EntitySpider GetSpiderContext()
		{
			EntitySpider context = new EntitySpider(new Site())
			{
				UserId = "86Research",
				TaskGroup = "HaoBrowser",
				Identity = "HaoBrowser Hao360Spider Buble " + DateTime.Now.ToString("yyyy-MM-dd HHmmss"),
				CachedSize = 1,
				ThreadNum = 1,
				SkipWhenResultIsEmpty = true,
				Downloader = new HttpClientDownloader()
				{
					Handlers = new List<IDownloadHandler>
					{
						new SubContentHandler {
							StartString="sales[\"hotsite_yixing\"] = [",
							EndString="}}",
							StartOffset=27,
							EndOffset=0
						},
						new ReplaceContentHandler {
							NewValue="/",
							OldValue="\\/",
						},
					}
				}
			};
			context.SetScheduler(new Extension.Scheduler.RedisScheduler {
				Host = "127.0.0.1",
				Port = 6379,
				Password = "#frAiI^MtFxh3Ks&swrnVyzAtRTq%w"
			});
			context.AddEntityPipeline(new MysqlPipeline()
			{
				ConnectString = "Database='testhao';Data Source= 127.0.0.1;User ID=root;Password=root@123456;Port=4306",
			});
			context.AddStartUrl("https://hao.360.cn/");
			context.AddEntityType(typeof(UpdateHao360Info));
			return context;
		}

		[Schema("testhao", "hao360buble")]
		[TypeExtractBy(Expression = "$.data", Type = ExtractType.JsonPath, Multi = false)]
		public class UpdateHao360Info : ISpiderEntity
		{
			[StoredAs("title", DataType.String, 100)]
			[PropertyExtractBy(Expression = "$.title", Type = ExtractType.JsonPath)]
			public string Title { get; set; }

			[StoredAs("url", DataType.String, 200)]
			[PropertyExtractBy(Expression = "$.link", Type = ExtractType.JsonPath)]
			public string Url { get; set; }

			[StoredAs("run_id", DataType.Date)]
			[PropertyExtractBy(Expression = "Now", Type = ExtractType.Enviroment)]
			public DateTime RunId { get; set; }

			public string Id { get; set; }
		}

		public class Hao360
		{
			public string HId { get; set; }
			public string IsBuble { get; set; }
			public string Name { get; set; }
		}
	}
}
