using HireHuntBackend.Context;
using HireHuntBackend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace HireHuntBackend.Services
{
    public class ExternalJobsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _remoteOkUrl;
        private readonly string _weWorkRemotelyUrl;
        private readonly DbContextHireHunt _dbContext;
        public ExternalJobsService(HttpClient httpClient,IConfiguration config, DbContextHireHunt dbContext)
        {
            _httpClient = httpClient;
            _remoteOkUrl = config["ExternalJobsUrl:RemoteOk"];
            _weWorkRemotelyUrl = config["ExternalJobsUrl:WeWorkRemotely"];
            _dbContext = dbContext;
        }

        public async Task<string> GetRemoteOk()
        {
            var jobs = await _httpClient.GetStringAsync(_remoteOkUrl);
            return jobs;
        }

        public async Task<List<JobPost>> WeWorkRemotely()
        {

            var reader = XmlReader.Create(_weWorkRemotelyUrl);
            var feed = SyndicationFeed.Load(reader);
            List<JobPost> jobs = new List<JobPost>();
            foreach(var item in feed.Items) {
                var region = item.ElementExtensions
                .ReadElementExtensions<string>("region", "")
                .FirstOrDefault();
                jobs.Add(new JobPost
                {
                    Title = item.Title.Text.Split(':')[1].Trim(),
                    Url = item.Links.FirstOrDefault().Uri.ToString(),
                    Description = item.Summary.Text,
                    PostedDate = item.PublishDate.DateTime,
                    Source = feed.Description.Text,
                    Location = region,
                    Company= item.Title.Text.Split(':')[0].Trim(),
                    Category=item.Categories.FirstOrDefault().Name,
                }); 

            }
            await _dbContext.JobPosts.AddRangeAsync(jobs);
            await _dbContext.SaveChangesAsync();

            return jobs;
        }


    }
}
