using HireHuntBackend.Common;
using HireHuntBackend.Context;
using HireHuntBackend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.ServiceModel.Syndication;
using System.Xml;
using static HireHuntBackend.Common.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HireHuntBackend.Services
{
    public class ExternalJobsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _remoteOkUrl;
        private readonly string _weWorkRemotelyUrl;
        private readonly string _arbeitNowUrl;
        private readonly string _remotiveUrl;
        private readonly DbContextHireHunt _dbContext;
        public ExternalJobsService(HttpClient httpClient,IConfiguration config, DbContextHireHunt dbContext)
        {
            _httpClient = httpClient;
            _remoteOkUrl = config["ExternalJobsUrl:RemoteOk"];
            _weWorkRemotelyUrl = config["ExternalJobsUrl:WeWorkRemotely"];
            _arbeitNowUrl = config["ExternalJobsUrl:ArbeitNow"];
            _remotiveUrl = config["ExternalJobsUrl:Remotive"];
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
                ?.ReadElementExtensions<string>("region", "")
                ?.FirstOrDefault();
                jobs.Add(new JobPost
                {
                    Title = item.Title.Text.Split(':')[1].Trim(),
                    Url = item.Links?.FirstOrDefault()?.Uri.ToString(),
                    Description = item.Summary.Text,
                    PostedDate = item.PublishDate.DateTime,
                    Source = ExternalJobs.WeWorkRemotely.ToString(),
                    Location = region,
                    Company= item.Title.Text.Split(':')[0].Trim(),
                    Category = item.Categories?.FirstOrDefault()?.Name,
                }); 

            }
            var remoteOkjobs = await _httpClient.GetStringAsync(_remoteOkUrl);
            var root = JsonConvert.DeserializeObject<List<RemoteOkJob>>(remoteOkjobs);
            if (root != null)
            {
                foreach (var item in root.Skip(1))
                {
                    jobs.Add(new JobPost
                    {
                        Title = item.position,
                        Url = item.url,
                        Description = item.description,
                        PostedDate = DateTime.TryParse(item.date, out var date) ? date :null,
                        Source = ExternalJobs.RemoteOk.ToString(),
                        Location = item.location,
                        Company = item.company,
                        Category = item.tags?.FirstOrDefault()

                    });
                }
            }

            var arbeitNowJobs=await _httpClient.GetStringAsync(_arbeitNowUrl);
            var arbeitnowJobRes = JsonConvert.DeserializeObject<ArbeitnowResponse>(arbeitNowJobs);
            var arbeitnowJobResData = arbeitnowJobRes.Data;
            foreach (var item in arbeitnowJobResData)
            {
                jobs.Add(new JobPost
                {
                    Title = item.Title,
                    Url = item.Url,
                    Description = item.Description,
                    PostedDate =DateTime.TryParse(item.Created_At,out var date ) ? date : null,
                    Source = ExternalJobs.ArbeitNow.ToString(),
                    Location = item.Location,
                    Company = item.Company_Name,
                    Category = item.Tags?.FirstOrDefault()

                });
            }
            var remotiveJobs=await _httpClient.GetStringAsync(_remotiveUrl);
            var remotiveJobsRes = JsonConvert.DeserializeObject<RemotiveJobResponse>(remotiveJobs);
            var remotiveJobsResData = remotiveJobsRes.Jobs;
            foreach (var item in remotiveJobsResData)
            {
                jobs.Add(new JobPost
                {
                    Title = item.Title,
                    Url = item.Url,
                    Description = item.Description,
                    PostedDate = DateTime.TryParse(item.Publication_Date, out var date) ? date : null,
                    Source = ExternalJobs.Remotive.ToString(),
                    Company = item.Company_Name,
                    Category = item.Category
                });
            }
            foreach(var job in jobs)
            {
                var existingJob=_dbContext.JobPosts.FirstOrDefault(x=>x.Url == job.Url);
                if(existingJob == null)
                {
                    await _dbContext.JobPosts.AddRangeAsync(jobs);
                }
                else
                {
                    existingJob.Title = job.Title;
                    existingJob.Description=job.Description;
                    existingJob.PostedDate =job.PostedDate;
                    existingJob.Source = job.Source;
                    existingJob.Company = job.Company;
                    existingJob.Category = job.Category;


                }
            }

            await _dbContext.SaveChangesAsync();

            return jobs;
        }


    }
}
