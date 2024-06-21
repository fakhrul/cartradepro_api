using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SPOT_API.Persistence;
using System;
using System.Linq;

namespace Performance
{
    class Program
    {
        private const int MISSING_USER_IN_MINUTE = 30;
        static void Main(string[] args)
        {
            try
            {

                IConfiguration _config = new ConfigurationBuilder()
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                 .Build();

                string resultString = "Entry written at {0}";
                resultString = string.Format(resultString, DateTime.Now.ToLongTimeString());
                var cs = _config.GetConnectionString("SpotDB");
                var options = new DbContextOptionsBuilder<SpotDBContext>()
                    .UseNpgsql(_config.GetConnectionString("SpotDB")).Options;

                WriteLog(string.Format("Spot Performance has been started at " + DateTime.Now.ToString()));

                using (var dbContext = new SpotDBContext(options))
                {

                    try
                    {
                        ProcessCalculatePerformance(dbContext);
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message);
                    }

                    try
                    {
                        ProcessFindMissingUser(dbContext);
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message);
                    }

                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ProcessFindMissingUser(SpotDBContext dbContext)
        {
            WriteLog(string.Format("----> Processing Missing User"));

            var currentDateTime = DateTime.Now;

            var schedules = dbContext.Schedules
                .Where(c => c.EndDateTime > currentDateTime)
                .Where(c => c.StartDateTime.AddMinutes(MISSING_USER_IN_MINUTE) < currentDateTime)
                .Include(c => c.Profile)
                .ToList();

            foreach (var schedule in schedules)
            {
                WriteLog(string.Format("Found active schedule [{0}]", schedule.Name));

                var activities = dbContext.Activities
                    .Where(c => c.AreaId == schedule.AreaId)
                    .Where(c => c.ProfileId == schedule.ProfileId)
                    .Where(c => c.StartDateTime <= schedule.EndDateTime)
                    .Where(c => c.EndDateTime >= schedule.StartDateTime)
                    .Include(c => c.Profile)
                    .ToList();

                if (activities.Count == 0)
                {
                    WriteLog(string.Format("No activity found for schedule [{0}].", schedule.Name));

                    var locationLog = dbContext.LocationLogs.Where(c => c.DeviceId == schedule.Profile.DeviceId).FirstOrDefault();
                    if (locationLog == null)
                    {
                        WriteLog(string.Format("Cant find location log. Create Missing User"));

                        SPOT_API.Models.MissingUser missingUser = new SPOT_API.Models.MissingUser
                        {
                            DateTime = currentDateTime,
                            ProfileId = schedule.ProfileId,
                            ScheduleId = schedule.Id,
                            TenantId = schedule.Profile.TenantId.Value,
                        };
                        dbContext.MissingUsers.Add(missingUser);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        var area = dbContext.Areas.Where(c => c.FingerPrintCode == locationLog.LastKnownLocation).FirstOrDefault();
                        if(area == null)
                        {
                            WriteLog(string.Format("Invalid area for this user. Create Missing User"));

                            SPOT_API.Models.MissingUser missingUser = new SPOT_API.Models.MissingUser
                            {
                                DateTime = currentDateTime,
                                ProfileId = schedule.ProfileId,
                                ScheduleId = schedule.Id,
                                TenantId = schedule.Profile.TenantId.Value,
                            };
                            dbContext.MissingUsers.Add(missingUser);
                            dbContext.SaveChanges();
                        }
                        else 
                        {
                            if (area.Id != schedule.AreaId)
                            {
                                WriteLog(string.Format("User is in other area [{0}]. Create Missing User", area.Name));

                                SPOT_API.Models.MissingUser missingUser = new SPOT_API.Models.MissingUser
                                {
                                    DateTime = currentDateTime,
                                    ProfileId = schedule.ProfileId,
                                    ScheduleId = schedule.Id,
                                    TenantId = schedule.Profile.TenantId.Value,
                                };
                                dbContext.MissingUsers.Add(missingUser);
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                if(currentDateTime.Subtract( locationLog.LastKnownLocationUpdateOn).TotalMinutes > 30)
                                {
                                    WriteLog(string.Format("User is in the area [{0}], but last update more than 30 minutes. Create Missing User", area.Name));

                                    SPOT_API.Models.MissingUser missingUser = new SPOT_API.Models.MissingUser
                                    {
                                        DateTime = currentDateTime,
                                        ProfileId = schedule.ProfileId,
                                        ScheduleId = schedule.Id,
                                        TenantId = schedule.Profile.TenantId.Value,
                                    };
                                    dbContext.MissingUsers.Add(missingUser);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }
                }
                else
                {
                    WriteLog(string.Format("Activity found, do nothing."));

                }

            }

        }


        private static void WriteLog(string msg)
        {
            Console.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), msg));
        }

        private static void ProcessCalculatePerformance(SpotDBContext dbContext)
        {
            WriteLog(string.Format("----> Processing Performance"));

            var currentDateTime = DateTime.Now;

            var schedules = dbContext.Schedules
                .Where(c => c.IsEvaluated == false)
                .Where(c => c.EndDateTime < currentDateTime)
                .ToList();

            foreach (var schedule in schedules)
            {
                WriteLog(string.Format("Found active schedule [{0}]", schedule.Name));

                var activities = dbContext.Activities
                    .Where(c => c.AreaId == schedule.AreaId)
                    .Where(c => c.ProfileId == schedule.ProfileId)
                    .Where(c => c.StartDateTime <= schedule.EndDateTime)
                    .Where(c => c.EndDateTime >= schedule.StartDateTime)
                    .ToList();


                var timeSpanOverTimeBegin = TimeSpan.Zero;
                var timeSpanOverTimeEnd = TimeSpan.Zero;

                var timeSpanUnderTimeBegin = TimeSpan.Zero;
                var timeSpanUnderTimeEnd = TimeSpan.Zero;

                var timeSpanNormal = TimeSpan.Zero;
                var timeSpanUnderTimeMiddle = TimeSpan.Zero;

                foreach (var activity in activities)
                {
                    /*
                     S  |<----->|
                     A  |<----->|

                     S  |<----->|
                     A   |<--->|
                     */
                    if (activity.StartDateTime >= schedule.StartDateTime && activity.EndDateTime <= schedule.EndDateTime)
                    {
                        timeSpanUnderTimeBegin += activity.StartDateTime.Subtract(schedule.StartDateTime);
                        timeSpanNormal += activity.EndDateTime.Subtract(activity.StartDateTime);
                        timeSpanUnderTimeEnd += schedule.EndDateTime.Subtract(activity.EndDateTime);
                    }

                    /*
                     S    |<----->|
                     A  |<--------->|
                     */
                    if (activity.StartDateTime < schedule.StartDateTime && activity.EndDateTime > schedule.EndDateTime)
                    {
                        timeSpanOverTimeBegin += schedule.StartDateTime.Subtract(activity.StartDateTime);
                        timeSpanNormal += schedule.EndDateTime.Subtract(schedule.StartDateTime);
                        timeSpanOverTimeEnd += activity.EndDateTime.Subtract(schedule.EndDateTime);
                    }


                    /*
                     S    |<----->|
                     A  |<----->|
                     */
                    if (activity.StartDateTime < schedule.StartDateTime && activity.EndDateTime <= schedule.EndDateTime)
                    {
                        timeSpanOverTimeBegin += schedule.StartDateTime.Subtract(activity.StartDateTime);
                        timeSpanNormal += activity.EndDateTime.Subtract(schedule.StartDateTime);
                        timeSpanUnderTimeEnd += schedule.EndDateTime.Subtract(activity.EndDateTime);
                    }

                    /*
                     S  |<----->|
                     A    |<----->|
                     */
                    if (activity.StartDateTime >= schedule.StartDateTime && activity.EndDateTime >= schedule.EndDateTime)
                    {
                        timeSpanUnderTimeBegin += activity.StartDateTime.Subtract(schedule.StartDateTime);
                        timeSpanNormal += schedule.EndDateTime.Subtract(activity.StartDateTime);
                        timeSpanOverTimeEnd += activity.EndDateTime.Subtract(schedule.EndDateTime);
                    }

                    activity.IsEvaluated = true;
                }

                var scheduledTimeSpan = schedule.EndDateTime.Subtract(schedule.StartDateTime);
                timeSpanUnderTimeMiddle = scheduledTimeSpan - timeSpanNormal - timeSpanUnderTimeBegin - timeSpanUnderTimeEnd;

                schedule.IsEvaluated = true;

                var perf = new SPOT_API.Models.Performance
                {
                    ProfileId = schedule.ProfileId,
                    Date = schedule.StartDateTime.Date,
                    ScheduleId = schedule.Id,
                    ScheduledTimeInMinutes = scheduledTimeSpan.TotalMinutes,
                    OverTimeBeginInMinutes = timeSpanOverTimeBegin.TotalMinutes,
                    OverTimeEndInMinutes = timeSpanOverTimeEnd.TotalMinutes,
                    UnderTimeBeginInMunites = timeSpanUnderTimeBegin.TotalMinutes,
                    UnderTimeEndInMinutes = timeSpanUnderTimeEnd.TotalMinutes,
                    NormalTimeInMinutes = timeSpanNormal.TotalMinutes,
                    UnderTimeMiddleInMinutes = timeSpanUnderTimeMiddle.TotalMinutes,
                };
                dbContext.Performances.Add(perf);
            }

            dbContext.SaveChanges();
        }
    }
}
