using System;
using System.Collections.Generic;
using DotnetSpider.Core;
using DotnetSpider.Core.Monitor;
using DotnetSpider.Core.Pipeline;
using DotnetSpider.Core.Processor;
using DotnetSpider.Core.Scheduler;
using DotnetSpider.Core.Selector;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Sample
{
	public class BaseUsage
	{
		public static void Run()
		{
			IocExtension.ServiceCollection.AddSingleton<IMonitorService, NLogMonitor>();

			var site = new Site { EncodingName = "UTF-8" };
			for (int i = 1; i < 5; ++i)
			{
				site.AddStartUrl("http://" + $"www.youku.com/v_olist/c_97_g__a__sg__mt__lg__q__s_1_r_0_u_0_pt_0_av_0_ag_0_sg__pr__h__d_1_p_{i}.html");
			}

			Spider spider = Spider.Create(site, new MyPageProcessor(), new QueueDuplicateRemovedScheduler()).AddPipeline(new MyPipeline()).SetThreadNum(1);

			SpiderMonitor.Register(spider);

			spider.Run();
			Console.Read();
		}

		private class MyPipeline : BasePipeline
		{
			public override void Process(ResultItems resultItems)
			{
				foreach (YoukuVideo entry in resultItems.Results["VideoResult"])
				{
					Console.WriteLine($"{entry.Name}");
				}

				//May be you want to save to database
				// 
			}
		}

		private class MyPageProcessor : IPageProcessor
		{
			public void Process(Page page)
			{
				var totalVideoElements = page.Selectable.SelectList(Selectors.XPath("//div[@class='yk-col3']")).Nodes();
				List<YoukuVideo> results = new List<YoukuVideo>();
				foreach (var videoElement in totalVideoElements)
				{
					var video = new YoukuVideo();
					video.Name = videoElement.Select(Selectors.XPath("./div/div[4]/div[1]/a")).GetValue();
					results.Add(video);
				}
				page.AddResultItem("VideoResult", results);
			}

			public Site Site { get; set; }
		}

		public class YoukuVideo
		{
			public string Name { get; set; }
		}
	}
}